using UnityEngine;
using System.Collections;

public class LoadingController : MonoBehaviour {

	[Range(0,1)]
	public float finalAlpha = 0.98f;
	CanvasGroup cg;

	// Use this for initialization
	void Awake () {
		cg = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
		if (cg.alpha < finalAlpha) {
			cg.alpha = Mathf.Lerp (cg.alpha,1f,2f * Time.deltaTime);
		} else {
			if (Application.CanStreamedLevelBeLoaded("Main")) {
				Application.LoadLevel("Main");
				Destroy (this);
			}
		}
	}
}
