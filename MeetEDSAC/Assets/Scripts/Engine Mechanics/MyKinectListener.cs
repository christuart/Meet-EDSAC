using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyKinectListener : MonoBehaviour, KinectGestures.GestureListenerInterface {

	public Controller mainController;
	public KinectFeedbackController kinectFeedback;

	public KinectGestures.Gestures[] gesturesToDetect = new KinectGestures.Gestures[] {
																KinectGestures.Gestures.SwipeLeft,
																KinectGestures.Gestures.SwipeRight,
																KinectGestures.Gestures.SwipeUp,
																KinectGestures.Gestures.SwipeDown,
																KinectGestures.Gestures.ZoomIn,
																KinectGestures.Gestures.ZoomOut };

	private KinectManager kinectManager;
	private long firstUserId;
	private long secondUserId;
	private int users = 0;

	private Dictionary<long,Dictionary<KinectGestures.Gestures,bool>> gestureStates;
	private Dictionary<long,Queue<KinectGestures.Gestures>> activeGestures;

	private Dictionary<long,bool> wasZooming;
	// this will say whether a zoom gesture was occurring last frame, so that we can
	// produce a zoomDelta and decide whether we are zooming in or out

	public int zoomFrames = 3;
	// this says how many frames worth of zoom data to average over
	public float zoomThreshold = 0.2f;
	// this says what the average zoom has to be to be outputted

	private Dictionary<long,Queue<float>> zoomHistory;

	private Dictionary<long,float> lastZoom;
	private Dictionary<long,float> currentZoomDelta;

	private Dictionary<long,bool> faceTrackingAvailable;
	private FacetrackingManager faceTracker;

	void Awake() {
		zoomHistory = new Dictionary<long,Queue<float>>();
		gestureStates = new Dictionary<long,Dictionary<KinectGestures.Gestures, bool>>();
		activeGestures = new Dictionary<long,Queue<KinectGestures.Gestures>>();
		wasZooming = new Dictionary<long, bool>();
		lastZoom = new Dictionary<long, float>();
		currentZoomDelta = new Dictionary<long, float>();
		faceTrackingAvailable = new Dictionary<long, bool> ();
	}

	void Start() {
		kinectManager = KinectManager.Instance;
		//kinectManager.gestureListeners.Add (this);
	}

	public void UserDetected(long userId, int userIndex)
	{
		if (users == 2) {
			kinectFeedback.AddItem(KinectFeedbackController.TOO_MANY_PLAYERS);
		} else {

			KinectManager manager = KinectManager.Instance;
			foreach (KinectGestures.Gestures g in gesturesToDetect)
				manager.DetectGesture(userId, g);

			Dictionary<KinectGestures.Gestures,bool> userGestureStates = new Dictionary<KinectGestures.Gestures, bool>();
			Queue<KinectGestures.Gestures> userActiveGestures = new Queue<KinectGestures.Gestures>();
			Queue<float> userZoomHistory = new Queue<float>();
			
			if (users == 0) {
				firstUserId = userId;
				mainController.SetFirstPlayerId(userId);
				mainController.OnNewFirstPlayer();
				users = 1;
				if (mainController.useKinect) kinectFeedback.AddItem(KinectFeedbackController.SINGLE_PLAYER);
			} else if (users == 1) {
				secondUserId = userId;
				mainController.SetSecondPlayerId(userId);
				mainController.OnNewSecondPlayer();
				users = 2;
				if (mainController.useKinect) kinectFeedback.AddItem(KinectFeedbackController.TWO_PLAYERS);
			}
			
			gestureStates.Add(userId,userGestureStates);
			activeGestures.Add(userId,userActiveGestures);
			zoomHistory.Add(userId,userZoomHistory);
			wasZooming.Add(userId,false);
			lastZoom.Add(userId,0f);
			currentZoomDelta.Add(userId,0f);
			faceTrackingAvailable.Add (userId,false);

		}
	}

	public void UserLost(long userId, int userIndex) {
		if (users == 2) {
			if (userId == firstUserId) {
				mainController.ClearFirstPlayer();
				ClearUserFromDictionaries(userId);
				users--;
				firstUserId = secondUserId;
				if (mainController.useKinect) kinectFeedback.AddItem(KinectFeedbackController.PLAYER_ONE_LEFT);
				return;
			} else if (userId == secondUserId) {
				mainController.ClearSecondPlayer();
				ClearUserFromDictionaries(userId);
				users--;
				if (mainController.useKinect) kinectFeedback.AddItem(KinectFeedbackController.PLAYER_TWO_LEFT);
				return;
			}
		} else if (users == 1) {
			if (userId == firstUserId) {
				ClearUserFromDictionaries(userId);
				mainController.ClearFirstPlayer();
				users--;
				if (mainController.useKinect) kinectFeedback.AddItem(KinectFeedbackController.LAST_PLAYER_LEFT);
				return;
			}
		}
		kinectFeedback.AddItem(KinectFeedbackController.UNKNOWN_PLAYER_LEFT);
	}

	public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                             KinectInterop.JointType joint, Vector3 screenPos)
	{
		if (activeGestures.ContainsKey(userId))
			activeGestures[userId].Enqueue(gesture);
		return true;
		// this true return will clear the gesture info, because once we've completed one gesture,
		// we assume that any others started at the same time were actually just misinterpretations.
	}
	
	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		if((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
		{

			wasZooming[userId] = true;
			currentZoomDelta[userId] = screenPos.z - lastZoom[userId];
			lastZoom[userId] = screenPos.z;

		}
		/*else if(gesture == KinectGestures.Gestures.Wheel && progress > 0.5f)
		{
			string sGestureText = string.Format ("{0} {1:F0} deg.", gesture, screenPos.z);
			if(GestureInfo != null)
			{
				GestureInfo.GetComponent<GUIText>().text = sGestureText;
			}
			
			//Debug.Log(sGestureText);
			progressDisplayed = true;
			progressGestureTime = Time.realtimeSinceStartup;
		}*/
	}

	public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                             KinectInterop.JointType joint)	{
		return true;
	}

	void Update() {
		
		// every update, reset gestures to 'not occurring', then check if that's correct

		kinectManager = KinectManager.Instance;

		if (users > 0) {
			ResetGestureStates(firstUserId);
			// reset gestures to not occurring
			CheckZooming(firstUserId);
			// check if zoom is happening, in or out
			UpdateGestureStates(firstUserId);
			// check if other gestures are happening
			CheckFaceTrackingAvailability(firstUserId);
			// check if face tracking is available
		}
		// repeat for second user
		if (users > 1) {
			ResetGestureStates(secondUserId);
			CheckZooming(secondUserId);
			UpdateGestureStates(secondUserId);
			CheckFaceTrackingAvailability(secondUserId);
		}

		
	}

	private void UpdateGestureStates(long userId) {
		while (activeGestures[userId].Count > 0) {
			(gestureStates[userId])[activeGestures[userId].Dequeue()] = true;
		}
	}
	private void ResetGestureStates(long userId) {
		for (int i = 0; i < gesturesToDetect.Length; i++)
			(gestureStates[userId])[gesturesToDetect[i]] = false;
	}

	private void CheckZooming(long userId) {
		if (wasZooming[userId]) {
			zoomHistory[userId].Enqueue(currentZoomDelta[userId]);
			while (zoomHistory[userId].Count > zoomFrames) zoomHistory[userId].Dequeue();
			float zoomSum = 0f;
			float thisZoom;
			for (int i = 0; i < zoomHistory[userId].Count; i++) {
				thisZoom = zoomHistory[userId].Dequeue();
				zoomSum += thisZoom;
				zoomHistory[userId].Enqueue(thisZoom);
			}
			if (kinectManager.GetLeftHandState(userId) == KinectInterop.HandState.Closed && kinectManager.GetRightHandState(userId) == KinectInterop.HandState.Closed) {
				if (zoomSum > zoomThreshold) {
					(gestureStates[userId])[KinectGestures.Gestures.ZoomIn] = true;
				} else if (2f * zoomSum < -zoomThreshold) {
					(gestureStates[userId])[KinectGestures.Gestures.ZoomOut] = true;
				} else {
					wasZooming[userId] = false;
				}
			} else {
				wasZooming[userId] = false;
			}
		} else {
			zoomHistory[userId].Clear();
		}
		currentZoomDelta[userId] = 0f;
		// This line makes it look as if currentZoomDelta is always 0f, but that's because it's updated outside of the Update loops
		// It gets set when GestureInProgress is called by KinectManager
	}
	private void CheckFaceTrackingAvailability(long userId) {
		faceTracker = FacetrackingManager.Instance;
		faceTrackingAvailable [userId] = faceTracker != null && faceTracker.IsTrackingFace (userId);
	}

	private void ClearUserFromDictionaries(long userId) {
		gestureStates.Remove(userId);
		activeGestures.Remove(userId);
		zoomHistory.Remove(userId);
		wasZooming.Remove(userId);
		lastZoom.Remove(userId);
		currentZoomDelta.Remove(userId);
		faceTrackingAvailable.Remove(userId);
	}
	
	public bool IsGestureActive(KinectGestures.Gestures g) {
		return (users > 0) ? IsGestureActive(firstUserId, g) : false;
	}
	public bool IsGestureActive(long userId, KinectGestures.Gestures g) {
		return (gestureStates[userId].ContainsKey(g)) ? (gestureStates[userId])[g]: false;
	}
	public bool IsFaceTrackingAvailable(long userId) {
		faceTracker = FacetrackingManager.Instance;
		return (userId == firstUserId || userId == secondUserId) && faceTracker.IsTrackingFace (userId);
	}
	public Quaternion GetUserFaceDirection() {
		return (users > 0) ? GetUserFaceDirection(firstUserId) : Quaternion.identity;
	}
	public Quaternion GetUserFaceDirection(long userId) {
		if (IsFaceTrackingAvailable(userId)) {
			return faceTracker.GetHeadRotation(userId,false);
		}
		return Quaternion.identity;
	}
	
}
