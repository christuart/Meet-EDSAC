using UnityEngine;
using System.Collections;

public class CreditController : MonoBehaviour {

	public CanvasGroup canvasGroup;

	private float targetAlpha = 0f;
	public float alphaSlide = 0.25f;

	// Use this for initialization
	void Start () {
		canvasGroup.alpha = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha,targetAlpha,alphaSlide);
	}

	public void ShowCredits() {
		targetAlpha = 1f;
	}
	public void HideCredits() {
		targetAlpha = 0f;
	}

}
