using UnityEngine;
using System.Collections;


public class ZoomSettings : MonoBehaviour {

	public enum ZoomSource { MOUSE, KEYBOARD, KINECT };

	public float zoomSlide = 0.4f;
	public float zoomThresh = 0.3f;
	public float zoomStepBig = 10f;
	public float zoomStepSmall = 5f;

	public float minimumFieldOfView = 5f;
	public float maximumFieldOfView = 80f;
	
	public bool mouseUsesBig = false;
	public bool keyboardUsesBig = true;
	public bool kinectUsesBig = false;
	public bool otherUsesBig = false;
	
	public float ZoomIn(float from, ZoomSource source) {
		switch(source) {
		case ZoomSource.MOUSE:
			return Mathf.Max(minimumFieldOfView,from - (mouseUsesBig ? zoomStepBig : zoomStepSmall));
		case ZoomSource.KEYBOARD:
			return Mathf.Max(minimumFieldOfView,from - (keyboardUsesBig ? zoomStepBig : zoomStepSmall));
		case ZoomSource.KINECT:
			return Mathf.Max(minimumFieldOfView,from - (kinectUsesBig ? zoomStepBig : zoomStepSmall));
		default:
			return Mathf.Max(minimumFieldOfView,from - (otherUsesBig ? zoomStepBig : zoomStepSmall));
		}
	}
	
	public float ZoomOut(float from, ZoomSource source) {
		switch(source) {
		case ZoomSource.MOUSE:
			return Mathf.Min(maximumFieldOfView,from + (mouseUsesBig ? zoomStepBig : zoomStepSmall));
		case ZoomSource.KEYBOARD:
			return Mathf.Min(maximumFieldOfView,from + (keyboardUsesBig ? zoomStepBig : zoomStepSmall));
		case ZoomSource.KINECT:
			return Mathf.Min(maximumFieldOfView,from + (kinectUsesBig ? zoomStepBig : zoomStepSmall));
		default:
			return Mathf.Min(maximumFieldOfView,from + (otherUsesBig ? zoomStepBig : zoomStepSmall));
		}
	}

}
