using UnityEngine;
using System.Collections;

public class LoadingController : MonoBehaviour {

	CanvasGroup cg;

	// Use this for initialization
	void Awake () {
		cg = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
		if (cg.alpha < .95f) {
			cg.alpha = Mathf.Lerp (cg.alpha,1f,8f * Time.deltaTime);
		} else {
			if (Application.CanStreamedLevelBeLoaded("Main")) {
				Application.LoadLevel("Main");
				Destroy (this);
			}
		}
	}
}
