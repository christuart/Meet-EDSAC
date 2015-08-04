using UnityEngine;
using System.Collections;

public class WorldSpaceCanvasHinge : MonoBehaviour {

	public float displayAngle = -5f;
	public float hideAngle = -110f;

	public float extension = 1.0f;
	public float target = 1.0f;
	public float slide;
	
	public KeyCode inKey = KeyCode.A;
	public KeyCode outKey = KeyCode.D;
		
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (inKey)) target = 0.0f;
		if (Input.GetKeyDown (outKey)) target = 1.0f;
		extension = Mathf.Lerp (extension,Mathf.Clamp01(target),slide);
		transform.eulerAngles = new Vector3(0,Mathf.Lerp (hideAngle,displayAngle,extension),0);
	}
}
