using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

	public bool go = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (transform.childCount > 0) {
				Animator a = GetComponent<Animator> ();
				a.speed = 50f;
			}
		}
		if (go) {
			Application.LoadLevel ("Main");
		}
	}
}
