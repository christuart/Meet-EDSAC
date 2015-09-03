using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum InformationContent {
	INTRODUCTION,
	INITIAL_ORDERS, 
	MEMORY_STORAGE, 
	DIVISION, 
	ORDER_CODES, 
	WORD_LENGTH, 
	ACCUMULATOR, 
	SCALING, 
	COMPUTATION_SPEED, 
	STORY_ONE, 
	STORY_TWO,
	STORY_THREE,
	STORY_FOUR,
	STORY_FIVE,
	STORY_SIX,
	STORY_SEVEN,
	STORY_EIGHT,
	STORY_NINE,
	STORY_TEN,
	STORY_ELEVEN,
	STORY_TWELVE,
	STORY_THIRTEEN,
	STORY_FOURTEEN,
	STORY_FIFTEEN,
	STORY_SIXTEEN,
	STORY_SEVENTEEN,
	STORY_EIGHTEEN,
	NONE };

public class InfoHolderController : MonoBehaviour {

	public GameObject[] content;
	public int shownContentId = 0;

	private int currentContentId = -1;

	private float infoAreaHeight;
	public Scrollbar scrollBar;
	public float scrollSlide = 3f;
	private bool scrolling = false;
	private IEnumerator scrollCoroutine;

	// Use this for initialization
	void Start () {
		infoAreaHeight = Mathf.Abs(GetComponent<RectTransform>().rect.height);
	}
	
	// Update is called once per frame
	void Update () {
		if (currentContentId != shownContentId) {
			PlaceObjectInInfoUI(shownContentId);
		}
	}
	
	public void PlaceObjectInInfoUI(InformationContent contentId) {
		PlaceObjectInInfoUI((int)contentId);
	}
	public void PlaceObjectInInfoUI(int contentId) {
		if (currentContentId != -1) {
			DeemphasiseButtonInInfoContent ();
		}
		if (contentId < 0 || contentId >= content.Length) {
			return;
		}
		currentContentId = -1;
		for (int i = 0; i < content.Length; i++) {
			if (i == contentId) {

				content[i].SetActive(true);

				RectTransform rt = content[i].GetComponent<RectTransform>();
				ScrollRect sr = GetComponent<ScrollRect>();
				sr.content = rt;
				sr.verticalNormalizedPosition = 1f;

				shownContentId = contentId;
				currentContentId = contentId;
			} else {
				content[i].SetActive(false);
			}
		}
	}
	public void PressButtonInInfoContent() {
		GameObject infoContent = ActiveContent ();
		if (infoContent != null) {
			Button contentButton = infoContent.GetComponentInChildren<Button>();
			if (contentButton != null)
				contentButton.onClick.Invoke();
		}
	}
	public void EmphasiseButtonInInfoContent() {
		GameObject infoContent = ActiveContent ();
		if (infoContent != null) {
			Button contentButton = infoContent.GetComponentInChildren<Button>();
			if (contentButton != null)
				contentButton.transform.localScale = 1.4f * Vector3.one;
		}
	}
	public void DeemphasiseButtonInInfoContent() {
		GameObject infoContent = ActiveContent ();
		if (infoContent != null) {
			Button contentButton = infoContent.GetComponentInChildren<Button>();
			if (contentButton != null)
				contentButton.transform.localScale = Vector3.one;
		}
	}

	public GameObject ActiveContent() {
		if (currentContentId >= 0 && currentContentId < content.Length) {
			return content[currentContentId];
		} else {
			return null;
		}
	}

	public void Scroll(bool scrollDown) {
		if (scrolling) {
			StopCoroutine(scrollCoroutine);
		}
		scrollCoroutine = ScrollActiveContent(scrollDown);
		StartCoroutine(scrollCoroutine);
	}


	private IEnumerator ScrollActiveContent(bool scrollDown) {
		GameObject scrollContent = ActiveContent();
		if (scrollContent == null) {
			yield break;
		}
		float contentHeight = Mathf.Abs (scrollContent.GetComponent<RectTransform>().rect.height);
		float scrollableHeight = contentHeight - infoAreaHeight;
		if (scrollableHeight < 0) {
			yield break;
		}
		float targetScrollRatio = (scrollDown ? -1 : 1) * (3f/4) * infoAreaHeight / scrollableHeight;
		float targetScrollBarPosition = Mathf.Clamp01(scrollBar.value+targetScrollRatio);
		scrolling = true;
		while (Mathf.Abs(scrollBar.value - targetScrollBarPosition) > 0.01f) {
			scrollBar.value = Mathf.Lerp (scrollBar.value,targetScrollBarPosition,scrollSlide*Time.deltaTime);
			yield return null;
		}
		scrolling = false;
		yield break;
	}

}
