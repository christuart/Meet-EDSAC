using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InspectorController : MonoBehaviour {

	private RectTransform rectTransform;
	private Image image;
	private Canvas canvas;
	private RectTransform canvasRectTransform;

	private Vector2 targetSizeDelta;
	public float heightSlide = .2f;

	// Use this for initialization
	void Start () {
		
		rectTransform = GetComponent<RectTransform>();
		image = GetComponent<Image>();
		canvas = GetComponentInParent<Canvas>();
		canvasRectTransform = canvas.GetComponent<RectTransform>();

		ResizeToImage();
	}
	
	// Update is called once per frame
	void Update () {
		canvasRectTransform.sizeDelta = Vector2.Lerp (canvasRectTransform.sizeDelta,targetSizeDelta,heightSlide*60f*Time.deltaTime);
	}

	public void SetImage(Sprite target) {
		image.sprite = target;
		ResizeToImage();
	}

	public void ResizeToImage() {
		float spriteHeight = rectTransform.rect.width * image.sprite.rect.height / image.sprite.rect.width;
		
		targetSizeDelta = canvasRectTransform.sizeDelta;
		targetSizeDelta.y -= rectTransform.rect.height - spriteHeight;
	}

}
