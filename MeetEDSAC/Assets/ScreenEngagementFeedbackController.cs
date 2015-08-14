using UnityEngine;
using System.Collections;

public class ScreenEngagementFeedbackController : MonoBehaviour {
	
	public BlurGradual mainCameraBlurGradual;
	public UnityStandardAssets.ImageEffects.ScreenOverlay overlayForLeft;
	public UnityStandardAssets.ImageEffects.ScreenOverlay overlayForRight;
	public CanvasGroup groupForLeft;
	public CanvasGroup groupForRight;
	
	public float overlayIntensity = 0.6f;
	public float canvasAlpha = 0.4f;

	// ************************************************************ HAVEN'T USED THIS LIKE I WAS MEANT TO
	public bool engagementRegionsModeActive = true;
	
	// controls for when only one user
	public bool singleEngagementRegion = true;
	[Range( -1, 1 )]
	public float engagementInput = 0f;
	public float engagementTransitionSlide = 0.15f;
	private float singleEngagementPosition = 0f;

	// controls for when multiple users
	private bool modelEngaged = false;
	private bool leftPanelEngaged = false;
	private bool rightPanelEngaged = false;

	// Use this for initialization
	void Start () {
		SetNeutral();
	}

	void Update() {
		if (singleEngagementRegion) {
			singleEngagementPosition = Mathf.Lerp(singleEngagementPosition,engagementInput,engagementTransitionSlide);
			mainCameraBlurGradual.SetBlur(Mathf.Abs (singleEngagementPosition) > 0.3f);
			overlayForLeft.intensity = Mathf.Clamp ((singleEngagementPosition-0.1f)/0.6f,0f,1f) * overlayIntensity;
			overlayForRight.intensity = Mathf.Clamp ((singleEngagementPosition+0.1f)/(-0.6f),0f,1f) * overlayIntensity;
			groupForLeft.alpha = canvasAlpha + (Mathf.Clamp (-(singleEngagementPosition),0,0.6f)/0.6f * (1f-canvasAlpha));
			groupForRight.alpha = canvasAlpha + (Mathf.Clamp (singleEngagementPosition,0,0.6f)/0.6f * (1f-canvasAlpha));
		}
	}

	public void SetSingleEngagementInput(float input) {
		engagementInput = Mathf.Clamp (input,-1f,1f);
	}
	public void SetNeutral() {
		mainCameraBlurGradual.SetBlur(false);
		groupForLeft.alpha = 1f;
		groupForRight.alpha = 1f;
	}
	public void SetModelEngaged() {
		// if there are multiple users, then each section will be controlled indiviually, externally
		if (singleEngagementRegion) {
			engagementInput = 0f;
		} else {
			mainCameraBlurGradual.SetBlur(false);
		}
	}
	public void SetPanelEngaged(bool isLeft) {
		// if there are multiple users, then each section will be controlled indiviually, externally
		if (singleEngagementRegion) {
			engagementInput = isLeft ? -1f : 1f;
		}
		else {
			if (isLeft) {
				overlayForRight.intensity = overlayIntensity;
			} else {
				overlayForLeft.intensity = overlayIntensity;
			}
		}
	}
	public void UseSingleEngagementRegion(bool use) {
		singleEngagementRegion = use;
	}

}
