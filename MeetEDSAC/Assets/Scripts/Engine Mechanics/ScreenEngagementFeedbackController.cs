using UnityEngine;
using System.Collections;

public class ScreenEngagementFeedbackController : MonoBehaviour {

	public GUIText testInputIndicator;

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
	[Range( 0, 1 )]
	public float panelOutEngagementDeflectionTrigger = 0.3f;
	[Range( 0, 1 )]
	public float panelAwayEngagementDeflectionTrigger = 0.8f;
	private bool leftOut;
	private bool rightOut;

	// controls for when dual users
	public bool dualEngagementRegions = false;
	[Range( -1, 1 )]
	public float secondEngagementInput = 0f;

	// variables for smoothly sliding the input from face tracking
	public float engagementTransitionSlide = 0.15f;
	public float engagementTransitionStickyThreshold = 0.05f;
	private float singleEngagementPosition = 0f;
	private float secondEngagementPosition = 0f;

	public bool modelEngaged = true;
	public bool leftPanelEngaged = false;
	public bool rightPanelEngaged = false;
	public bool[] userEngagingModel = new bool[2];
	public bool[] userEngagingLeftPanel = new bool[2];
	public bool[] userEngagingRightPanel = new bool[2];

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
				if (Mathf.Abs (engagementInput - singleEngagementPosition) > engagementTransitionStickyThreshold) {
					singleEngagementPosition = Mathf.Lerp(singleEngagementPosition,engagementInput,engagementTransitionSlide*20f*Time.deltaTime);
				}
				testInputIndicator.transform.localPosition = new Vector3 (0.5f+0.5f*singleEngagementPosition, 0.8f);
				mainCameraBlurGradual.SetBlur( (singleEngagementPosition < LeftPanelEngagamentDeflectionTrigger()) || (singleEngagementPosition > RightPanelEngagamentDeflectionTrigger()));
				overlayForLeft.intensity = Mathf.Clamp ((singleEngagementPosition-0.1f)/0.6f,0f,1f) * overlayIntensity;
				overlayForRight.intensity = Mathf.Clamp ((singleEngagementPosition+0.1f)/(-0.6f),0f,1f) * overlayIntensity;
				groupForLeft.alpha = canvasAlpha + (Mathf.Clamp (-(singleEngagementPosition),0,0.6f)/0.6f * (1f-canvasAlpha));
				groupForRight.alpha = canvasAlpha + (Mathf.Clamp (singleEngagementPosition,0,0.6f)/0.6f * (1f-canvasAlpha));
				if (singleEngagementPosition < LeftPanelEngagamentDeflectionTrigger()) {
					MarkOnlyLeftPanelEngaged();
				} else if (singleEngagementPosition <= RightPanelEngagamentDeflectionTrigger()) {
					MarkOnlyModelEngaged();
				} else {
					MarkOnlyRightPanelEngaged();
				}
			} else if (dualEngagementRegions) {
				if (Mathf.Abs (engagementInput - singleEngagementPosition) > engagementTransitionStickyThreshold) {
					singleEngagementPosition = Mathf.Lerp(singleEngagementPosition,engagementInput,engagementTransitionSlide*20f*Time.deltaTime);
				}
				if (Mathf.Abs (secondEngagementInput - secondEngagementPosition) > engagementTransitionStickyThreshold) {
					secondEngagementPosition = Mathf.Lerp(secondEngagementPosition,secondEngagementInput,engagementTransitionSlide*20f*Time.deltaTime);
				}
				mainCameraBlurGradual.SetBlur(  ((singleEngagementPosition < LeftPanelEngagamentDeflectionTrigger()) || (singleEngagementPosition > RightPanelEngagamentDeflectionTrigger())) && ((secondEngagementPosition < LeftPanelEngagamentDeflectionTrigger()) || (secondEngagementPosition > RightPanelEngagamentDeflectionTrigger()))  );
				overlayForLeft.intensity = Mathf.Clamp ((Mathf.Min(singleEngagementPosition,secondEngagementPosition)-0.1f)/0.6f,0f,1f) * overlayIntensity;
				overlayForRight.intensity = Mathf.Clamp ((Mathf.Max(singleEngagementPosition,secondEngagementPosition)+0.1f)/(-0.6f),0f,1f) * overlayIntensity;
				groupForLeft.alpha = canvasAlpha + (Mathf.Clamp (-Mathf.Min(singleEngagementPosition,secondEngagementPosition),0,0.6f)/0.6f * (1f-canvasAlpha));
				groupForRight.alpha = canvasAlpha + (Mathf.Clamp (Mathf.Max(singleEngagementPosition,secondEngagementPosition),0,0.6f)/0.6f * (1f-canvasAlpha));

				// now set what is engaged, starting by clearing everything
				ClearEngagement();
				if (singleEngagementPosition < LeftPanelEngagamentDeflectionTrigger()) {
					SetPanelEngaged(0,true);
				} else if (singleEngagementPosition <= RightPanelEngagamentDeflectionTrigger()) {
					SetModelEngaged(0);
				} else {
					SetPanelEngaged(0,false);
				}
				if (secondEngagementPosition < LeftPanelEngagamentDeflectionTrigger()) {
					SetPanelEngaged(1,true);
				} else if (secondEngagementPosition <= RightPanelEngagamentDeflectionTrigger()) {
					SetModelEngaged(1);
				} else {
					SetPanelEngaged(1,false);
				}
			} else {
				overlayForLeft.intensity = Mathf.Lerp(overlayForLeft.intensity,leftIntensityTarget,multiEngagementTransitionSlide*20f*Time.deltaTime);
				overlayForRight.intensity = Mathf.Lerp(overlayForRight.intensity,rightIntensityTarget,multiEngagementTransitionSlide*20f*Time.deltaTime);
				groupForLeft.alpha = Mathf.Lerp(groupForLeft.alpha,leftAlphaTarget,multiEngagementTransitionSlide*20f*Time.deltaTime);
				groupForRight.alpha = Mathf.Lerp(groupForRight.alpha,rightAlphaTarget,multiEngagementTransitionSlide*20f*Time.deltaTime);
			}
		}
	}
	
	private float LeftPanelEngagamentDeflectionTrigger() {
		return leftOut ? -panelOutEngagementDeflectionTrigger : -panelAwayEngagementDeflectionTrigger;
	}
	private float RightPanelEngagamentDeflectionTrigger() {
		return rightOut ? panelOutEngagementDeflectionTrigger : panelAwayEngagementDeflectionTrigger;
	}
	public void SetSingleEngagementInput(float input) {
		engagementInput = Mathf.Clamp (input,-1f,1f);
	}
	public void SetSecondEngagementInput(float input) {
		secondEngagementInput = Mathf.Clamp (input,-1f,1f);
	}
	public void SetNeutral() {
		mainCameraBlurGradual.SetBlur(false);
		groupForLeft.alpha = 1f;
		groupForRight.alpha = 1f;
	}
	public void SetModelEngaged() {
		SetModelEngaged (0);
	}
	public void SetModelEngaged(int user) {
		userEngagingModel[user] = true;
		modelEngaged = true;
	}
	public void SetPanelEngaged(bool isLeft) {
		SetPanelEngaged(0,isLeft);
	}
	public void SetPanelEngaged(int user, bool isLeft) {
		if (isLeft) {
			userEngagingLeftPanel[user] = true;
			leftPanelEngaged = true;
		} else {
			userEngagingRightPanel[user] = true;
			rightPanelEngaged = true;
		}
	}
	public void SetModelDisengaged() {
		SetModelDisengaged (0);
	}
	public void SetModelDisengaged(int user) {
			userEngagingModel[user] = false;
			modelEngaged = false;
			foreach (bool maybe in userEngagingModel)
				modelEngaged = modelEngaged || maybe;
	}
	public void SetPanelDisengaged(bool isLeft) {
		SetPanelDisengaged (0,isLeft);
	}
	public void SetPanelDisengaged(int user, bool isLeft) {
		if (isLeft) {
			userEngagingLeftPanel[user] = false;
			leftPanelEngaged = false;
			foreach (bool maybe in userEngagingLeftPanel)
				leftPanelEngaged = leftPanelEngaged || maybe;
		} else {
			userEngagingRightPanel[user] = false;
			rightPanelEngaged = false;
			foreach (bool maybe in userEngagingRightPanel)
				rightPanelEngaged = rightPanelEngaged || maybe;
		}
	}
	public void ClearEngagement() {
		SetPanelDisengaged(0,false);
		SetPanelDisengaged(1,false);
		SetPanelDisengaged(0,true);
		SetPanelDisengaged(1,true);
		SetModelDisengaged(0);
		SetModelDisengaged(1);
	}
	public void MarkOnlyLeftPanelEngaged() {
		ClearEngagement ();
		SetPanelEngaged (true);
	}
	public void MarkOnlyModelEngaged() {
		ClearEngagement ();
		SetModelEngaged ();
	}
	public void MarkOnlyRightPanelEngaged() {
		ClearEngagement ();
		SetPanelEngaged (false);
	}
	public bool IsOnlyLeftPanelEngaged() {
		return leftPanelEngaged && !modelEngaged && !rightPanelEngaged;
	}
	public bool IsOnlyModelEngaged() {
		return !leftPanelEngaged && modelEngaged && !rightPanelEngaged;
	}
	public bool IsOnlyRightPanelEngaged() {
		return !leftPanelEngaged && !modelEngaged && rightPanelEngaged;
	}
	public void UseSingleEngagementRegion(bool use) {
		singleEngagementRegion = use;
		if (use) {
			// make sure that engagement regions are being used at all
			engagementRegionsModeActive = true;
			dualEngagementRegions = false;
		} else if (engagementRegionsModeActive) {
			// if we'll now be using the multi user mode, we don't want to immediately
			// change the overlays and alphas (we wait for external input to do that) so
			// we need to set the targets to the current values
			leftIntensityTarget = overlayForLeft.intensity;
			rightIntensityTarget = overlayForRight.intensity;
			leftAlphaTarget = groupForLeft.alpha;
			rightAlphaTarget = groupForRight.alpha;
		}
	}
	public void UseDualEngagementRegions(bool use) {
		dualEngagementRegions = use;
		if (use) {
			engagementRegionsModeActive = true;
			singleEngagementRegion = false;
		}
	}
	public void DontUseEngagementRegions() {
		engagementRegionsModeActive = false;
		singleEngagementRegion = false;
		dualEngagementRegions = false;
	}
	public void MarkPanelOut(bool isLeft, bool isOut) {
		if (isLeft) {
			leftOut = isOut;
		} else {
			rightOut = isOut;
		}
	}

}
