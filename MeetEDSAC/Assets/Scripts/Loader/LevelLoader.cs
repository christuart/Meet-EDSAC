using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

	public bool go = false;

	// Update is called once per frame
	void Update () {
		if (go || Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel ("Main");
	}
}
