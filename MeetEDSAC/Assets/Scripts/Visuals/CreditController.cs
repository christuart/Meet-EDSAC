using UnityEngine;
using System.Collections;

public class CreditController : MonoBehaviour {

	public CanvasGroup canvasGroup;

	private float targetAlpha = 0f;
	public float alphaSlide = 0.25f;

	// Use this for initialization
	void Start () {
		canvasGroup.alpha = 0f;
		canvasGroup.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (canvasGroup.gameObject.activeSelf)
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha,targetAlpha,alphaSlide);
		if (canvasGroup.alpha < 0.01f)
			canvasGroup.gameObject.SetActive(false);
	}

	public void ShowCredits() {
		canvasGroup.gameObject.SetActive(true);
		targetAlpha = 1f;
	}
	public void HideCredits() {
		targetAlpha = 0f;
	}

}
