using UnityEngine;
using System.Collections;

public class WorldSpaceCanvasHinge : MonoBehaviour {

	public float displayAngle = -5f;
	public float hideAngle = -110f;

	public float extension = 1.0f;
	public float target = 1.0f;
	public float slide;
		
	// Update is called once per frame
	void Update () {
		extension = Mathf.Lerp (extension,Mathf.Clamp01(target),slide);
		transform.eulerAngles = new Vector3(0,Mathf.Lerp (hideAngle,displayAngle,extension),0);
	}
	
	public void SetHinge(float t) {
		target = t;
	}
	public void MoveHingeAway() {
		SetHinge (0.0f);
	}
	public void MoveHingeOut() {
		SetHinge (1.0f);
	}
}
