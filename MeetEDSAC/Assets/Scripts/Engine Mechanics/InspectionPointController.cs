﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InspectionPointController : MonoBehaviour {

	public CanvasGroup group;

	public float alphaSlide = .1f;
	public float alphaThresh = .02f;

	public float showFor = 4f;
	public float fadeFor = 1.5f;

	public float maxAlpha = .4f;

	public Sprite imageContent;
	public bool isVideo;
	public Videos videoContent;
	
	private float alphaTarget;
	private float pointerLeftAt = 0f;

	private bool hovering = false;

	// Use this for initialization
	void Start () {
		group.alpha = 0f;
		alphaTarget = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (group.alpha - alphaTarget) < alphaThresh) {
			group.alpha = alphaTarget;
		} else {
			group.alpha = Mathf.Lerp (group.alpha,alphaTarget,alphaSlide*20f*Time.deltaTime);
		}
		if (!hovering) {
			float timeSince = Time.time - pointerLeftAt;
			if (timeSince < showFor + fadeFor) {
				if (timeSince > showFor) {
					alphaTarget = maxAlpha * (1f - (timeSince - showFor) / fadeFor);
				}
			} else {
				alphaTarget = 0f;
			}
		}
	}
	
	public void HoverEnter() {
		alphaTarget = maxAlpha;
		hovering = true;
		GetComponent<Animator>().SetTrigger("HoverEnter");
	}
	public void HoverLeave() {
		alphaTarget = maxAlpha;
		hovering = false;
		pointerLeftAt = Time.time;
		GetComponent<Animator>().SetTrigger("HoverExit");
	}
	public void Choose() {
		InspectorController inspectorController = GameObject.FindObjectOfType<InspectorController>();
		if (isVideo) {
			if (inspectorController.videoController.textureTarget.mainTexture == inspectorController.videoController.videos[(int)videoContent]) {
				inspectorController.PauseVideo();
			} else {
				inspectorController.SetVideo (videoContent,true);
			}
		} else {
			inspectorController.SetImage(imageContent);
		}
	}
}
