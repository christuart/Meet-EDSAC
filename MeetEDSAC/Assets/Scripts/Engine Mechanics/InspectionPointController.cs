using UnityEngine;
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
	private bool hidden = false;

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
		if (!hidden) {
			alphaTarget = maxAlpha;
			hovering = true;
			GetComponent<Animator>().SetTrigger("HoverEnter");
		}
	}
	public void HoverLeave() {
		if (!hidden) {
			alphaTarget = maxAlpha;
			hovering = false;
			pointerLeftAt = Time.time;
			GetComponent<Animator>().SetTrigger("HoverExit");
		}
	}
	public void ChooseByClick() {
		Choose();
		GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().audioController.RunAudioEvent(UIAudioController.AudioEvent.INSPECTION_POINT_CLICKED);
	}
	public void Choose() {
		Choose (true);
	}
	public void Choose(bool pauseOnRechoose) {
		InspectorController inspectorController = GameObject.FindObjectOfType<InspectorController>();
		if (isVideo) {
			//Debug.Log ("Step a4: It's a video, check if it's the same one as before");
			if (inspectorController.videoController.textureTarget.mainTexture == inspectorController.videoController.videos[(int)videoContent]) {
				//Debug.Log ("Step aa5: It's the same one as before, make sure it's playing");
				if (pauseOnRechoose || !inspectorController.videoController.videos[(int)videoContent].isPlaying){
					//Debug.Log ("Step aa6: Running 'PauseVideo' because it was paused and we want it unpaused");
					inspectorController.PauseVideo();
				}
			} else {
				//Debug.Log ("Step ba5: It's a different video from before");
				inspectorController.SetVideo (videoContent,true);
			}
		} else {
			//Debug.Log ("Step b4: It's an image");
			inspectorController.SetImage(imageContent);
		}
	}
	public void Hide() {
		if (hovering) {
			GetComponent<Animator>().SetTrigger("HoverExit");
		}
		hidden = true;
		GetComponent<ParticleSystem>().enableEmission = false;
		HoverLeave();
	}
	public void Unhide() {
		hidden = false;
		GetComponent<ParticleSystem>().enableEmission = true;
	}

}
