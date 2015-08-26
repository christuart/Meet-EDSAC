using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LabelFadeIn : MonoBehaviour {

	public CanvasGroup cg;

	// Use this for initialization
	void Start () {
		cg = GetComponent<CanvasGroup>();
		StartCoroutine(FadeIn ());
	}

	public IEnumerator FadeIn() {
		while (cg.alpha < 0.98f) {
			cg.alpha = Mathf.Lerp (cg.alpha,1f,0.3f);
			yield return null;
		}
		cg.alpha = 1f;
	}
}
