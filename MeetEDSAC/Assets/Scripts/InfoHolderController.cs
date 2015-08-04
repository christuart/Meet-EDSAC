﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoHolderController : MonoBehaviour {

	public GameObject[] content;
	public int shownContentId = 0;

	private int currentContentId = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (currentContentId != shownContentId) {
			PlaceObjectInInfoUI(shownContentId);
		}
	}

	public void PlaceObjectInInfoUI(int contentId) {
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

				currentContentId = contentId;
			} else {
				content[i].SetActive(false);
			}
		}
	}
}
