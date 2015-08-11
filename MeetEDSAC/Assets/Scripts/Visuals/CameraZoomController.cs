using UnityEngine;
using System.Collections;

public class CameraZoomController : MonoBehaviour {
	
	public Camera cam;
	public ZoomSettings zoomSettings;

	private float target;

	// Use this for initialization
	void Start () {
		target = cam.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (target-cam.fieldOfView) > zoomSettings.zoomThresh) {
			cam.fieldOfView = Mathf.Lerp (cam.fieldOfView,target,zoomSettings.zoomSlide);
		}
	}

	public float GetZoom() {
		return cam.fieldOfView;
	}
	public void SetZoom(float _target) {
		if (_target != 0f)
			target = _target;
	}
	public void ZoomIn(ZoomSettings.ZoomSource source) {
		target = zoomSettings.ZoomIn(target,source);
	}
	public void ZoomOut(ZoomSettings.ZoomSource source) {
		target = zoomSettings.ZoomOut(target,source);
	}
}
