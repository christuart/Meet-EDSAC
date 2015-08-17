using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuSceneController : MonoBehaviour {

	public Toggle kinectSensorToggle;
	public Toggle kinectFaceTrackingToggle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnKinectSensorToggleClick() {
		kinectSensorToggle.isOn = !kinectSensorToggle.isOn;
	}

	public void OnKinectSensorToggleChanged(bool val) {
		if (val == true) {
			kinectFaceTrackingToggle.interactable = true;
		} else {
			kinectFaceTrackingToggle.isOn = false;
			kinectFaceTrackingToggle.interactable = false;
		}
	}
}
