using UnityEngine;
using System.Collections;

public class CameraSettingsMirror : MonoBehaviour {
	
	public Camera obj;
	public Camera im;

	// Use this for initialization
	void Start () {
		if (obj == null || im == null) {
			Destroy(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
			im.fieldOfView = obj.fieldOfView;
	}
}
