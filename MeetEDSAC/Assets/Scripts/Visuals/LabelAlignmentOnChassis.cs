using UnityEngine;
using System.Collections;

public class LabelAlignmentOnChassis : MonoBehaviour {

	public Transform chassis;

	// Use this for initialization
	void Start () {
		if (chassis != null) {
			transform.position = chassis.position + new Vector3(.36f,-.082f, -.003f);
			transform.localPosition += new Vector3(.03f,.035f, -.059f);
			transform.localScale = new Vector3(.4f,.01f,.222f);
		}
	}
}
