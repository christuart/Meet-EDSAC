using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KinectFeedbackItemController : MonoBehaviour {

	public KinectFeedbackController feedbackController;
	public CanvasGroup group;
	public Text text;
	public float slide = 0.2f;

	private int messageId;
	private float yTarget;
	private float heightTarget;
	private float widthTarget;
	private float lifeTime;
	private int maxStack;

	private float bornTime;
	private int stackPosition = 0;
	private int messageCount = 1;

	private bool dying = false;

	private RectTransform rectTransform;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
		group = GetComponent<CanvasGroup>();
	}

	// Use this for initialization
	void Start () {
		if (lifeTime == 0f) {
			lifeTime = 2f;
		}
		bornTime = Time.time;
		yTarget = YFromStackPosition(stackPosition);
	}
	
	// Update is called once per frame
	void Update () {
		rectTransform.localPosition = new Vector2(0f, Mathf.Lerp (rectTransform.localPosition.y, yTarget, slide*60f*Time.deltaTime));
		rectTransform.sizeDelta = Vector2.Lerp (rectTransform.sizeDelta, new Vector2(widthTarget, heightTarget), slide*60f*Time.deltaTime);
		if (!dying) {
			if (Time.time-bornTime > lifeTime)
				Die();
		} else {
			if (group.alpha > 0.05) {
				group.alpha = Mathf.Lerp (group.alpha,0,slide*60f*Time.deltaTime);
			} else {
				feedbackController.RemoveItem(stackPosition);
				Destroy (gameObject);
			}
		}
	}

	private float YFromStackPosition(int i) {
		return 2f + feedbackController.height * i;
	}

	public void SetFeedbackController(KinectFeedbackController kfc) {		
		feedbackController = kfc;
		transform.SetParent(kfc.transform,false);
		//transform.localScale = new Vector3(1f,1f,1f);
		//rectTransform.localPosition = new Vector2(0f,0f);
	}
	public void SetStackPosition(int i) {
		stackPosition = i;
		yTarget = YFromStackPosition(stackPosition);
		if (stackPosition >= maxStack) { // maxStack is size of stack, so max stack id is maxStack - 1
			dying = true;
		}
	}
	public void SetSize(float w, float h) {
		widthTarget = w;
		heightTarget = h;
	}
	public void SetSlide(float s) {
		slide = s;
	}
	public void SetLifetime(float t) {
		lifeTime = t;
	}
	public void SetMaxStack(int m) {
		maxStack = m;
	}
	public void SetFlat() {
		rectTransform.sizeDelta = new Vector2(widthTarget, 0f);
	}
	public void SetWidthMultiplier(float m) {
		widthTarget *= m;
	}
	public void SetMessage(int i) {
		SetMessage (i, 1);
	}
	public void SetMessage(int i, int n) {
		messageId = i;
		if (messageId >= 0 && messageId < KinectFeedbackController.messages.Length) {
			text.text = KinectFeedbackController.messages[messageId] + (n > 1 ? " [x" + messageCount + "]" : "");
		} else {
			text.text = "[Error m" + messageId + (n > 1 ? " x" + messageCount : "") + "]";
		}
	}
	public void Increment() {
		messageCount++;
		SetMessage (messageId,messageCount);
		bornTime = Time.time;
	}
	public bool IsShowingMessage(int i) {
		return messageId == i;
	}

	public void Die() {
		dying = true;
	}

}
