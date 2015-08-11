using UnityEngine;
using System.Collections;

public class SimpleGalleryController : MonoBehaviour {

	public Sprite[] gallery;

	public KeyCode nextKey = KeyCode.DownArrow;
	public KeyCode prevKey = KeyCode.UpArrow;

	public float scrollTime = .2f;
	public float autoScrollTime = 2f;
	public float autoScrollResumeTime = 10f;
	private float lastScroll = 0f;
	private bool autoScroll = true;


	private int shownImageId = 0;

	// Update is called once per frame
	void Update () {
		if (gallery.Length != 0) {
			int moveImage = 0;
			if (autoScroll) {
				if (Time.time - lastScroll > autoScrollTime) 
					moveImage = 1;
			} else {
				if (Time.time - lastScroll > autoScrollResumeTime) {
					autoScroll = true;
					moveImage = 1;
				}
			}
			if (Time.time - lastScroll > scrollTime) {
				if (Input.GetKey (nextKey)) {
					moveImage = 1;
					autoScroll = false;
				}
				if (Input.GetKey (prevKey)) {
					moveImage = -1;
					autoScroll = false;
				}
			}
			if (moveImage != 0) {
				lastScroll = Time.time;
				moveImage += shownImageId;
				shownImageId = (gallery.Length + moveImage) % gallery.Length;
				GetComponent<InspectorController>().SetImage(gallery[shownImageId]);
			}
		}
	}
}
