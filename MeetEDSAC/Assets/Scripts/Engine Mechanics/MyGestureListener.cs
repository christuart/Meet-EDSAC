using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface {

	public KinectGestures.Gestures[] gesturesToDetect = new KinectGestures.Gestures[] {
																KinectGestures.Gestures.SwipeLeft,
																KinectGestures.Gestures.SwipeRight,
																KinectGestures.Gestures.SwipeUp,
																KinectGestures.Gestures.SwipeDown,
																KinectGestures.Gestures.ZoomIn,
																KinectGestures.Gestures.ZoomOut };

	private Dictionary<KinectGestures.Gestures,bool> gestureStates;
	private Queue<KinectGestures.Gestures> activeGestures;

	private bool wasZooming = false;
	// this will say whether a zoom gesture was occurring last frame, so that we can
	// produce a zoomDelta and decide whether we are zooming in or out
	public int zoomFrames = 3;
	// this says how many frames worth of zoom data to average over
	public float zoomThreshold = 0.2f;
	// this says what the average zoom has to be to be outputted
	private Queue<float> zoomHistory;

	private float lastZoom;
	private float currentZoomDelta = 0f;

	void Awake() {
		zoomHistory = new Queue<float>();
		gestureStates = new Dictionary<KinectGestures.Gestures, bool>();
		activeGestures = new Queue<KinectGestures.Gestures>();
	}

	public void UserDetected(long userId, int userIndex)
	{

		KinectManager manager = KinectManager.Instance;
		foreach (KinectGestures.Gestures g in gesturesToDetect)
			manager.DetectGesture(userId, g);

	}
	public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                             KinectInterop.JointType joint, Vector3 screenPos)
	{
		
		activeGestures.Enqueue(gesture);
		
		return true;
	}
	
	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		if((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
		{

			if (!wasZooming) {
				wasZooming = true;
			} else {
				currentZoomDelta = screenPos.z - lastZoom;
			}
			lastZoom = screenPos.z;

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
	public void UserLost(long userId, int userIndex) {
		return;
	}

	void Update() {
		
		// every update, reset gestures to 'not occurring', then check if that's correct
		
		// reset to not occurring
		for (int i = 0; i < gesturesToDetect.Length; i++)
			gestureStates[gesturesToDetect[i]] = false;
		
		// check if zoom is happening, in or out
		zoomHistory.Enqueue(currentZoomDelta);
		while (zoomHistory.Count > zoomFrames) zoomHistory.Dequeue();
		float zoomSum = 0f;
		float thisZoom;
		for (int i = 0; i < zoomHistory.Count; i++) {
			thisZoom = zoomHistory.Dequeue();
			zoomSum += thisZoom;
			zoomHistory.Enqueue(thisZoom);
		}
		if (zoomSum > zoomThreshold) {
			gestureStates[KinectGestures.Gestures.ZoomIn] = true;
		} else if (zoomSum < -zoomThreshold) {
			gestureStates[KinectGestures.Gestures.ZoomOut] = true;
		}
		currentZoomDelta = 0f;
		
		// check if other gestures are happening
		while (activeGestures.Count > 0) {
			gestureStates[activeGestures.Dequeue()] = true;
		}
		
	}

	public bool IsGestureActive(KinectGestures.Gestures g) {
		if (gestureStates.ContainsKey(g)) {
			return gestureStates[g];
		} else {
			return false;
		}
	}
	
}
