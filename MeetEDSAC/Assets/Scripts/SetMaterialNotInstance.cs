using UnityEngine;
using System.Collections;

public class SetMaterialNotInstance : MonoBehaviour {

	public Material m;

	void Update() {
		if (Input.GetKeyDown(KeyCode.M)) {
			GetComponent<MeshRenderer>().sharedMaterial = m;
			Destroy(this);
		}
	}
}
