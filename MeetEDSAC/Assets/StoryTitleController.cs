using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryTitleController : MonoBehaviour {

	private string title = "EDSAC Tour";

	public static int slideCount;
	public static bool slidesCounted = false;

	public static int GetSlideCount() {
		if (slidesCounted) {
			return slideCount;
		} else {
			StoryTitleController[] stc = Resources.FindObjectsOfTypeAll(typeof(StoryTitleController)) as StoryTitleController[];
			slideCount = stc.Length;
			slidesCounted = true;
			return slideCount;
		}
	}

	void Awake () {
		GetComponent<Text>().text = title + ": " + transform.parent.name.Substring (transform.parent.name.LastIndexOf (" ")) + "/" + GetSlideCount();
	}

	void Start () {
		Destroy(this);
	}
}
