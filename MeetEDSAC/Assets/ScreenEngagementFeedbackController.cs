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

	// variables for smoothly sliding the input from face tracking
	public float singleEngagementTransitionSlide = 0.15f;
	private float singleEngagementPosition = 0f;


	// controls for when multiple users
	private bool modelEngaged = false;
	private bool leftPanelEngaged = false;
	private bool rightPanelEngaged = false;

	// variables for sliding towards the engagement state
	// requested when multiple users are impacting it
	private float leftIntensityTarget;
	private float rightIntensityTarget;
	private float leftAlphaTarget;
	private float rightAlphaTarget;
	public float multiEngagementTransitionSlide = 0.3f;

	// Use this for initialization
	void Start () {
		SetNeutral();
		leftIntensityTarget = overlayForLeft.intensity;
		rightIntensityTarget = overlayForRight.intensity;
		leftAlphaTarget = groupForLeft.alpha;
		rightAlphaTarget = groupForRight.alpha;
	}

	void Update() {
		if (engagementRegionsModeActive) {
			if (singleEngagementRegion) {
				singleEngagementPosition = Mathf.Lerp(singleEngagementPosition,engagementInput,singleEngagementTransitionSlide);
				mainCameraBlurGradual.SetBlur(Mathf.Abs (singleEngagementPosition) > 0.3f);
				overlayForLeft.intensity = Mathf.Clamp ((singleEngagementPosition-0.1f)/0.6f,0f,1f) * overlayIntensity;
				overlayForRight.intensity = Mathf.Clamp ((singleEngagementPosition+0.1f)/(-0.6f),0f,1f) * overlayIntensity;
				groupForLeft.alpha = canvasAlpha + (Mathf.Clamp (-(singleEngagementPosition),0,0.6f)/0.6f * (1f-canvasAlpha));
				groupForRight.alpha = canvasAlpha + (Mathf.Clamp (singleEngagementPosition,0,0.6f)/0.6f * (1f-canvasAlpha));
			} else {
				overlayForLeft.intensity = Mathf.Lerp(overlayForLeft.intensity,leftIntensityTarget,multiEngagementTransitionSlide);
				overlayForRight.intensity = Mathf.Lerp(overlayForRight.intensity,rightIntensityTarget,multiEngagementTransitionSlide);
				groupForLeft.alpha = Mathf.Lerp(groupForLeft.alpha,leftAlphaTarget,multiEngagementTransitionSlide);
				groupForRight.alpha = Mathf.Lerp(groupForRight.alpha,rightAlphaTarget,multiEngagementTransitionSlide);
			}
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
		if (use) {
			// make sure that engagement regions are being used at all
			engagementRegionsModeActive = true;
		}
		else if (engagementRegionsModeActive) {
			// if we'll now be using the multi user mode, we don't want to immediately
			// change the overlays and alphas (we wait for external input to do that) so
			// we need to set the targets to the current values
			leftIntensityTarget = overlayForLeft.intensity;
			rightIntensityTarget = overlayForRight.intensity;
			leftAlphaTarget = groupForLeft.alpha;
			rightAlphaTarget = groupForRight.alpha;
		}
	}
	public void DontUseEngagementRegions() {
		engagementRegionsModeActive = false;
	}

}
