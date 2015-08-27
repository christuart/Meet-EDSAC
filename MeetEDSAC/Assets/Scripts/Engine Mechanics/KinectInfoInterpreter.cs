using UnityEngine;
using System.Collections;

public enum EdsacGestures { LEFT_SWIPE, RIGHT_SWIPE, UP_SWIPE, DOWN_SWIPE, STRETCH, SQUASH, SELECT };

public class KinectInfoInterpreter : MonoBehaviour {

	public GUIText gazeVectorText;

	public MyKinectListener kinectListener;
	public KinectFeedbackController feedbackController;
	private long userId = -1;

	public EdsacGestures[] gesturesBeingRead;

	public float[] gestureContinuousTime;
	public float[] gestureRepeatGap;
	
	private float[] mappedGestureContinuousTime;
	private float[] mappedGestureRepeatGap;

	private bool[] gestureOn;						// Tells you if the gesture system says this is happening
	private bool[] gesturePreviouslyOn;				// Tells you if ^^^ was happening last frame
	private bool[] gestureSwitchedOn;				// Tells you if the interpreter says this frame it switched on
	private bool[] gestureTogglePositive;			// Tells you if the interpreter says we are between on and off calls
	private bool[] gestureSwitchedOff;				// Tells you if the interpreter says this frame it switched off

	private float[] gestureLastOn;
	private float[] gestureLastSwitchedOn;
	private float[] gestureLastSwitchedOff;

	private int gesturesCount;

	private bool gazeAvailable;
	private float gazeYaw;
	public GameObject gazeIndicator;

	public bool useNumpad = false;
	
	public bool GetGestureTriggered(EdsacGestures gesture) {
		return gestureSwitchedOn[(int)gesture];
	}
	public bool GetGestureToggled(EdsacGestures gesture) {
		return gestureSwitchedOn[(int)gesture] || gestureSwitchedOff[(int)gesture];
	}
	public bool GetGestureEnded(EdsacGestures gesture) {
		return gestureSwitchedOff[(int)gesture];
	}
	public bool GetGestureHappening(EdsacGestures gesture) {
		return gestureTogglePositive[(int)gesture];
	}
	public bool GetGazeDirectionAvailable() {
		return gazeAvailable;
	}
	public float GetGazeDirection() {
		return gazeYaw;
	}

	void Start() {

		gesturesCount = gesturesBeingRead.Length;
		
		gestureOn = new bool[gesturesCount];
		gesturePreviouslyOn = new bool[gesturesCount];
		gestureTogglePositive = new bool[gesturesCount];
		gestureSwitchedOn = new bool[gesturesCount];
		gestureSwitchedOff = new bool[gesturesCount];

		mappedGestureContinuousTime = new float[System.Enum.GetValues (typeof(EdsacGestures)).Length];
		mappedGestureRepeatGap = new float[System.Enum.GetValues (typeof(EdsacGestures)).Length];
		
		gestureLastOn = new float[gesturesCount];
		gestureLastSwitchedOn = new float[gesturesCount];
		gestureLastSwitchedOff = new float[gesturesCount];

		for (int i = 0; i < gesturesCount; i++) {
			gestureOn[i] = false;
			gesturePreviouslyOn[i] = false;
			gestureSwitchedOn[i] = false;
			gestureSwitchedOff[i] = false;
			
			gestureLastOn[i] = 0f;
			gestureLastSwitchedOn[i] = 0f;
			gestureLastSwitchedOff[i] = 0f;
			
			int gestureId = (int)gesturesBeingRead[i];
			mappedGestureContinuousTime[gestureId] = gestureContinuousTime[i];
			mappedGestureRepeatGap[gestureId] = gestureRepeatGap[i];
		}

		gazeAvailable = false;

	}

	void Update() {

		gesturePreviouslyOn = (bool[])gestureOn.Clone();
		//gestureOn

		// These only happen for one frame at a time, so reset them.
		gestureSwitchedOn = new bool[gesturesCount];
		gestureSwitchedOff = new bool[gesturesCount];

		// This is checked every frame that a user is available, so reset to false in case no user is available
		gazeAvailable = false;

		// Here we will calculate the new state of gestures and store it ready to be requested by whomever.
		// We only want to try this if we have a genuine userId though, otherwise we're actually inactive for now
		if (userId != -1) {
			for (int i=0; i < gesturesBeingRead.Length; i++) {
				EdsacGestures gesture = gesturesBeingRead [i];
				// GestureData data = ReadRawGestureInfo(gesture);
				gestureOn [i] = ReadRawGestureInfo (gesture);
				//Debug.Log ("checking gesture #" + i);
				if (gestureOn [i]) {
					//Debug.Log ("gesture is on");
					gestureLastOn [i] = Time.time;
					//Debug.Log (gesturePreviouslyOn[i]);
					if (!gesturePreviouslyOn [i]) {
						//Debug.Log ("gesture wasn't previously on");
						if (gestureRepeatGap [i] > 0) {
							if (Time.time - gestureLastSwitchedOff [i] > gestureRepeatGap [i]) {
								GestureSwitchOn (i);
							}
						} else {
							GestureSwitchOn (i);
						}
					} else if (gestureTogglePositive [i] && gestureContinuousTime [i] > 0) {
						//Debug.Log ("gesture was previously on and continuous repeat is activated");
						if (Time.time - gestureLastSwitchedOn [i] > gestureContinuousTime [i]) {
							//Debug.Log ("Continuous repeat condition met, so 'switched on'");
							GestureSwitchOn (i);
						}
					}
				} else if (gestureTogglePositive [i]) {
					//Debug.Log ("gesture is off, used to be on, so 'switched off'");
					GestureSwitchOff (i);
				}
			}

			gazeAvailable = ReadFaceTrackingAvailable ();
			if (gazeAvailable) {
				gazeYaw = GazeFromQuaternion (ReadHeadRotation ());
				if (gazeIndicator != null) gazeIndicator.transform.eulerAngles = new Vector3(0f,180f,gazeYaw*60f);
			}
		}

	}
	
	private void GestureSwitchOn(int i) {
		gestureSwitchedOn[i] = true;
		gestureLastSwitchedOn[i] = Time.time;
		GestureTogglePositive(i, true);
		if (kinectListener.mainController.useKinect) feedbackController.AddItem(KinectFeedbackController.gestureMessageIds[gesturesBeingRead[i]]);
	}
	private void GestureSwitchOff(int i) {
		gestureSwitchedOff[i] = true;
		gestureLastSwitchedOff[i] = Time.time;
		GestureTogglePositive(i, false);
	}
	private void GestureTogglePositive(int i, bool positive = true) {
		gestureTogglePositive[i] = positive;
	}


	private bool ReadRawGestureInfo(EdsacGestures gesture) {
		switch(gesture) {
		case EdsacGestures.LEFT_SWIPE:
			if (useNumpad) {
				return (useNumpad && Input.GetKey(KeyCode.Keypad4));
			} else {
				return kinectListener.IsGestureActive(userId, KinectGestures.Gestures.SwipeLeft);
			}
		case EdsacGestures.RIGHT_SWIPE:
			if (useNumpad) {
				return (useNumpad && Input.GetKey(KeyCode.Keypad6));
			} else {
				return kinectListener.IsGestureActive(userId, KinectGestures.Gestures.SwipeRight);
			}
		case EdsacGestures.UP_SWIPE:
			if (useNumpad) {
				return (useNumpad && Input.GetKey(KeyCode.Keypad8));
			} else {
				return kinectListener.IsGestureActive(userId, KinectGestures.Gestures.SwipeUp);
			}
		case EdsacGestures.DOWN_SWIPE:
			if (useNumpad) {
				return (useNumpad && Input.GetKey(KeyCode.Keypad2));
			} else {
				return kinectListener.IsGestureActive(userId, KinectGestures.Gestures.SwipeDown);
			}
		case EdsacGestures.STRETCH:
			if (useNumpad) {
				return (useNumpad && Input.GetKey(KeyCode.KeypadPlus));
			} else {
				return kinectListener.IsGestureActive(userId, KinectGestures.Gestures.ZoomIn);
			}
		case EdsacGestures.SQUASH:
			if (useNumpad) {
				return (useNumpad && Input.GetKey(KeyCode.KeypadMinus));
			} else {
				return kinectListener.IsGestureActive(userId, KinectGestures.Gestures.ZoomOut);
			}
		default:
			return false;
		}
	}
	private bool ReadFaceTrackingAvailable() {
		return kinectListener.IsFaceTrackingAvailable(userId);
	}
	private Quaternion ReadHeadRotation() {
		return kinectListener.GetUserFaceDirection(userId);
	}

	private float GazeFromQuaternion(Quaternion headRotation) {
		Vector3 directionVector = headRotation * Vector3.forward;
		if (gazeVectorText != null) gazeVectorText.text = System.String.Format("({0:0.000},{1:0.000})", directionVector.x, directionVector.z);
		return Mathf.Atan2 (directionVector.x, directionVector.z);
	}

	public void SetUserId(long _userId) {
		userId = _userId;
	}

	public void UseNumpad(bool use) {
		if (use) {
			useNumpad = true;
			if (userId == -1) userId = -2;
		} else {
			useNumpad = false;
			if (userId == -2) userId = -1;
		}
	}

}
