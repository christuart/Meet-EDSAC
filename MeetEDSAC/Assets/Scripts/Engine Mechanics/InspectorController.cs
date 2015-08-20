using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InspectorController : MonoBehaviour {
	
	public Image photoImage;
	public VideoTextureController videoController;
	public Sprite startImage;

	private RectTransform rectTransform;
	private Canvas canvas;
	private RectTransform canvasRectTransform;

	private Vector2 targetSizeDelta;
	public float heightSlide = .2f;

	// Use this for initialization
	void Start () {
		
		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		canvasRectTransform = canvas.GetComponent<RectTransform>();

		if (startImage != null)
			SetImage(startImage);
	}
	
	// Update is called once per frame
	void Update () {
		canvasRectTransform.sizeDelta = Vector2.Lerp (canvasRectTransform.sizeDelta,targetSizeDelta,heightSlide*20f*Time.deltaTime);
	}

	public void SetImage(Sprite target) {
		videoController.Stop();
		videoController.gameObject.SetActive(false);
		photoImage.gameObject.SetActive(true);
		photoImage.sprite = target;
		ResizeToImage();
	}
	public void SetVideo(Videos target, bool play = true) {
		videoController.gameObject.SetActive(true);
		photoImage.gameObject.SetActive(false);
		videoController.SetVideo(target,play);
		ResizeToVideo();
	}
	public void PauseVideo() {
		if (videoController.IsPlaying()) {
			videoController.Pause();
		} else {
			videoController.Play();
		}
	}
	
	public void ResizeToImage() {
		float spriteHeight = rectTransform.rect.width * photoImage.sprite.rect.height / photoImage.sprite.rect.width;
		Resize(spriteHeight);
	}
	public void ResizeToVideo() {
		float spriteHeight = rectTransform.rect.width * videoController.textureTarget.mainTexture.height / videoController.textureTarget.mainTexture.width;
		Resize(spriteHeight);
	}
	public void Resize(float spriteHeight) {
		targetSizeDelta = canvasRectTransform.sizeDelta;
		targetSizeDelta.y -= rectTransform.rect.height - spriteHeight;
	}

}
