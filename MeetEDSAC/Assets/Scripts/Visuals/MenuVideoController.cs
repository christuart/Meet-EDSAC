using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuVideoController : MonoBehaviour {

//	public MovieTexture menuVideo;

	// Use this for initialization
	void Start () {
//		if (menuVideo == null) {
			Destroy(gameObject);
			return;
//		}
//		RectTransform rt = GetComponent<RectTransform>();
//		RawImage target = GetComponent<RawImage>();
//		if (9f * Screen.width > 16f * Screen.height) { // wider than video, so must make video taller than screen
//			rt.sizeDelta = new Vector2(rt.sizeDelta.x,rt.sizeDelta.x*9f/16f);
//		} else { // taller than video, so must make video wider than screen
//			rt.sizeDelta = new Vector2(rt.sizeDelta.y*16f/9f,rt.sizeDelta.y);
//		}
//		target.texture = menuVideo;
//		menuVideo.Play();
//		menuVideo.loop = true;
	}
}
