using UnityEngine;
using System.Collections;

public enum FakeGestures { LEFT_SWIPE, RIGHT_SWIPE, UP_SWIPE, DOWN_SWIPE, STRETCH, SQUASH, SELECT };

public class GestureInfoInterpreter : MonoBehaviour {

	public MyGestureListener gestureListener;
	public uint userId;

	public FakeGestures[] gesturesBeingRead;

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
	
	public bool GetGestureTriggered(FakeGestures gesture) {
		return gestureSwitchedOn[(int)gesture];
	}
	public bool GetGestureToggled(FakeGestures gesture) {
		return gestureSwitchedOn[(int)gesture] || gestureSwitchedOff[(int)gesture];
	}
	public bool GetGestureEnded(FakeGestures gesture) {
		return gestureSwitchedOff[(int)gesture];
	}
	public bool GetGestureHappening(FakeGestures gesture) {
		return gestureTogglePositive[(int)gesture];
	}

	void Start() {

		gesturesCount = gesturesBeingRead.Length;
		
		gestureOn = new bool[gesturesCount];
		gesturePreviouslyOn = new bool[gesturesCount];
		gestureTogglePositive = new bool[gesturesCount];
		gestureSwitchedOn = new bool[gesturesCount];
		gestureSwitchedOff = new bool[gesturesCount];

		mappedGestureContinuousTime = new float[System.Enum.GetValues (typeof(FakeGestures)).Length];
		mappedGestureRepeatGap = new float[System.Enum.GetValues (typeof(FakeGestures)).Length];
		
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
	}

	void Update() {

		gesturePreviouslyOn = (bool[])gestureOn.Clone();
		//gestureOn

		// These only happen for one frame at a time, so reset them.
		gestureSwitchedOn = new bool[gesturesCount];
		gestureSwitchedOff = new bool[gesturesCount];

		// here we will calculate the new state of gestures and store it ready to be requested by whomever
		// for now, there are just no real gestures happening EVER

		for (int i=0; i < gesturesBeingRead.Length; i++) {
			FakeGestures gesture = gesturesBeingRead[i];
			// GestureData data = ReadRawGestureInfo(gesture);
			gestureOn[i] = ReadRawGestureInfo(gesture);
			//Debug.Log ("checking gesture #" + i);
			if (gestureOn[i]) {
				//Debug.Log ("gesture is on");
				gestureLastOn[i] = Time.time;
				//Debug.Log (gesturePreviouslyOn[i]);
				if (!gesturePreviouslyOn[i]) {
					//Debug.Log ("gesture wasn't previously on");
					if (gestureRepeatGap[i] > 0) {
						if (Time.time - gestureLastSwitchedOff[i] > gestureRepeatGap[i]) {
							gestureSwitchedOn[i] = true;
							gestureTogglePositive[i] = true;
							gestureLastSwitchedOn[i] = Time.time;
						}
					} else {
						gestureSwitchedOn[i] = true;
						gestureTogglePositive[i] = true;
						gestureLastSwitchedOn[i] = Time.time;
					}
				} else if (gestureTogglePositive[i] && gestureContinuousTime[i] > 0) {
					//Debug.Log ("gesture was previously on and continuous repeat is activated");
					if (Time.time - gestureLastSwitchedOn[i] > gestureContinuousTime[i]) {
						//Debug.Log ("Continuous repeat condition met, so 'switched on'");
						gestureSwitchedOn[i] = true;
						gestureTogglePositive[i] = true;
						gestureLastSwitchedOn[i] = Time.time;
					}
				}
			} else if (gestureTogglePositive[i]) {
				//Debug.Log ("gesture is off, used to be on, so 'switched off'");
				gestureSwitchedOff[i] = true;
				gestureLastSwitchedOff[i] = Time.time;
				gestureTogglePositive[i] = false;
			}
		}

	}

	private bool ReadRawGestureInfo(FakeGestures gesture) {
		// some fakery
		switch(gesture) {
		case FakeGestures.LEFT_SWIPE:
			return gestureListener.IsGestureActive(KinectGestures.Gestures.SwipeLeft);
		case FakeGestures.RIGHT_SWIPE:
			return gestureListener.IsGestureActive(KinectGestures.Gestures.SwipeRight);
		case FakeGestures.UP_SWIPE:
			return gestureListener.IsGestureActive(KinectGestures.Gestures.SwipeUp);
		case FakeGestures.DOWN_SWIPE:
			return gestureListener.IsGestureActive(KinectGestures.Gestures.SwipeDown);
		case FakeGestures.STRETCH:
			return gestureListener.IsGestureActive(KinectGestures.Gestures.ZoomIn);
		case FakeGestures.SQUASH:
			return gestureListener.IsGestureActive(KinectGestures.Gestures.ZoomOut);
		default:
			return false;
		}
	}
}
