using UnityEngine;
using System.Collections;

public class LabelAlignmentOnChassis : MonoBehaviour {

	public Transform chassis;

	// Use this for initialization
	void Awake () {
		if (chassis != null)
			transform.position = chassis.position + new Vector3(.36f,-.082f, -.003f);
	}
}
