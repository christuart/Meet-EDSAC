using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectFeedbackController : MonoBehaviour {

	public static string[] messages = new string[] {
		"Swipe Left",
		"Swipe Right",
		"Swipe Up",
		"Swipe Down",
		"Zoom In",
		"Zoom Out",
		"Grab",
		"A maximum of two users is allowed.",
		"Single user Kinect mode activated.",
		"Dual user Kinect mode activated.",
		"User 1 left. Single user mode activated for user 2.",
		"User 2 left. Single user mode activated for user 1.",
		"No users. Kinect mode deactivated.",
		"Unexpected 'user left' event! Who were you...",
		"Debug mode activated.",
		"Debug mode deactivated."
	};

	#region MESSAGE-IDS
	public static Dictionary<EdsacGestures,int> gestureMessageIds = new Dictionary<EdsacGestures, int>() {
		{EdsacGestures.LEFT_SWIPE, 0},
		{EdsacGestures.RIGHT_SWIPE, 1},
		{EdsacGestures.UP_SWIPE, 2},
		{EdsacGestures.DOWN_SWIPE, 3},
		{EdsacGestures.STRETCH, 4},
		{EdsacGestures.SQUASH, 5},
		{EdsacGestures.SELECT, 6}
	};
	public static int TOO_MANY_PLAYERS = 7;
	public static int SINGLE_PLAYER = 8;
	public static int TWO_PLAYERS = 9;
	public static int PLAYER_ONE_LEFT = 10;
	public static int PLAYER_TWO_LEFT = 11;
	public static int LAST_PLAYER_LEFT = 12;
	public static int UNKNOWN_PLAYER_LEFT = 13;
	public static int DEBUG_ON = 14;
	public static int DEBUG_OFF = 15;
	#endregion MESSAGE-IDS

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
