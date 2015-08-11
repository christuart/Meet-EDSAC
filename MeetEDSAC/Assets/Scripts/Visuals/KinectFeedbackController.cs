using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectFeedbackController : MonoBehaviour {

	public static string[] messages = new string[] {
		"This is a message.",
		"This aint a message." };
	public GameObject prefab;
	public float startWidth = 500f;
	public float height = 35f;
	public float slide = 0.5f;
	public float stackMultiplier = 0.8f;
	public float messageLifetime = 20f;
	public int maxStack = 3; // can go higher than this, but as soon as it does, they
							 // fade out instead of waiting for lifetime

	public bool test = false;
	public int testMessage = 0;

	private IList<KinectFeedbackItemController> items;

	// Use this for initialization
	void Awake () {
		items = new List<KinectFeedbackItemController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (test) {
			AddItem(testMessage);
			test = false;
		}
	}

	public void AddItem(int messageId) {
		if (items.Count > 0) {
			if (items[0].IsShowingMessage(messageId)) {
				items[0].Increment();
				return;
			}
		}
		GameObject holder = (GameObject)Instantiate(prefab);
		KinectFeedbackItemController item = holder.GetComponent<KinectFeedbackItemController>();
		item.SetFeedbackController(this);
		item.SetMessage(messageId);
		item.SetSize(startWidth,height);
		item.SetSlide(slide);
		item.SetFlat();
		item.SetLifetime(messageLifetime);
		item.SetMaxStack(maxStack);
		items.Insert (0,item);
		for (int i=1; i<items.Count; i++) {
			items[i].SetStackPosition (i);
			items[i].SetWidthMultiplier(stackMultiplier);
		}
	}
	public void RemoveItem(int i) {
		items.RemoveAt(i);
	}
}
