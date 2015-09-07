using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuSceneController : MonoBehaviour {
	
	private const int MenuSceneNumber = 0;
	private const int MainSceneNumber = 3;
	public Toggle kinectSensorToggle;
	public Toggle keyboardToggle;
	public Toggle kinectFaceTrackingToggle;
	public Toggle mouseToggle;

	public LoadingController loader;
	
	private bool useKinect;
	private bool useKeyboard;
	private bool useFace;
	private bool useMouse;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Application.loadedLevel == MenuSceneNumber)
				Application.Quit();
		}
	}

	public void OnKinectSensorToggleChanged(bool val) {
		if (val == true) {
			kinectFaceTrackingToggle.interactable = true;
		} else {
			kinectFaceTrackingToggle.isOn = false;
			mouseToggle.isOn = true;
			kinectFaceTrackingToggle.interactable = false;
		}
	}

	public void OnLoadEDSACButtonPressed() {
		
		useKinect = kinectSensorToggle.isOn;
		useKeyboard = keyboardToggle.isOn;
		useFace = kinectFaceTrackingToggle.isOn;
		useMouse = mouseToggle.isOn;

		loader.gameObject.SetActive(true);

	}
	
	void OnLevelWasLoaded(int level) {
		if (level == MainSceneNumber) {

			Controller controller = GameObject.FindObjectOfType<Controller>();
			if (controller == null) {
				Debug.Log ("Couldn't find the controller to apply settings. Oops!");
				Destroy(gameObject);
				return;
			}

			controller.useKinect = useKinect;
			controller.useKeyboard = useKeyboard;
			controller.useFace = useFace;
			controller.useMouse = useMouse;
			controller.SetupKinect();
			
			Destroy (gameObject);

		}
	}
}
