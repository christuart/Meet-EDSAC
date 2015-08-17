using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LabelController : MonoBehaviour {

	public Camera viewCamera;
	public Renderer targetForLabel;
	public RectTransform label;
	public Vector2 panelMargin;
	public RectTransform nameTag;

	public float oneAlphaDistance = 1.0f;
	public float zeroAlphaDistance = 3.0f;
	public float minAlpha = 0.2f;
	public bool considerNormal = true;
	public float normalInfluence = 1f;
	public float power = 2f;

	private Image[] panels;
	private Text[] texts;
	public bool atRight = false;
	public bool atTop = false;
	public float rightX;
	public float topY;
	public Vector2 initialPosMin;
	public Vector2 initialPosMax;
	public float desiredWidth;

	public bool localisedLabel = false;
	public bool keepWithinScreen = false;

	public bool active = true;
	private bool childrenActivated = true;
	private Queue activateQueue;

	public bool debug = false;

	void Awake() {
		activateQueue = new Queue();
	}

	// Use this for initialization
	void Start () {

		panels = GetComponentsInChildren<Image>();
		texts = GetComponentsInChildren<Text>();
		
		float maxTextWidth = 0f;
		foreach (Text t in texts)
			maxTextWidth = Mathf.Max (maxTextWidth, t.preferredWidth);
		if (maxTextWidth == 0f) maxTextWidth = 124f;
		desiredWidth = maxTextWidth + 25f;
		// the desired padding is 25f

		initialPosMin = nameTag.offsetMin;
		initialPosMax = new Vector2(nameTag.offsetMin.x + desiredWidth, nameTag.offsetMax.y);

	}

	
	// Update is called once per frame
	void Update () {
		while (activateQueue.Count > 0) {
			if ((bool)activateQueue.Dequeue()) ActivateNow();
			else DeactivateNow();
		}
		if (active) {
			Vector4 minMaxViewport = Tools.ViewportFromBounds(targetForLabel.bounds.min,targetForLabel.bounds.max,viewCamera);
			Vector2 min = viewCamera.ViewportToScreenPoint(new Vector2(minMaxViewport.x,minMaxViewport.y));
			Vector2 max = viewCamera.ViewportToScreenPoint(new Vector2(minMaxViewport.z,minMaxViewport.w));
			
			nameTag.offsetMin = initialPosMin;
			nameTag.offsetMax = initialPosMax;
			Vector2 offMin = nameTag.offsetMin;
			Vector2 offMax = nameTag.offsetMax;
			if (keepWithinScreen) {
				if (!atRight) {
					if (max.x > Screen.width-120f) {
						atRight = true;
						rightX = nameTag.localPosition.x;
					}
				} else {
					nameTag.localPosition = new Vector2(rightX,nameTag.localPosition.y);
					offMin.x -= max.x - (Screen.width-120f);
					offMax.x -= max.x - (Screen.width-120f);
					if (max.x <= Screen.width-120f) {
						atRight = false;
					}
				}
			}
			nameTag.offsetMin = offMin;
			nameTag.offsetMax = offMax;
			if (localisedLabel) {
				// place the label in the middle at the top of the target,
				// but also don't let it go off the top of the screen
				if (keepWithinScreen) {
					max.y = Mathf.Min (max.y, Screen.height-65f);
					min.y = Mathf.Min (min.y, Screen.height-65f);
				}
				//min.y = max.y;
				min.x += max.x;
				min.x /= 2;
				max.x = min.x;
			}
			if (keepWithinScreen) {
				if (!atTop) {
					if (max.y > Screen.height-35f) {
						atTop = true;
						topY = nameTag.localPosition.y;
					}
				} else {
					//nameTag.localPosition = new Vector2(nameTag.localPosition.x,topY);
					//offMin.y -= max.y - (Screen.height-35f);
					//offMax.y -= max.y - (Screen.height-35f);
					if (max.y <= Screen.height-35f) {
						atTop = false;
					}
				}
			}
			nameTag.offsetMin = offMin;
			nameTag.offsetMax = offMax;

			label.offsetMax = max + panelMargin;
			label.offsetMin = min - panelMargin;
			float dist = Tools.MinDistanceFromBounds(targetForLabel.bounds.min,targetForLabel.bounds.max,viewCamera);
			float alpha = 1.0f-Mathf.Clamp((dist-oneAlphaDistance)/(zeroAlphaDistance-oneAlphaDistance),0f,1f-minAlpha);
			if (considerNormal) {
				alpha *= Mathf.Pow (Vector3.Dot (viewCamera.transform.forward,(targetForLabel.transform.position-viewCamera.transform.position).normalized),normalInfluence);
			}
			alpha = Mathf.Pow(alpha,power);
			foreach (Image i in panels) {
				i.color = new Color(i.color.r,i.color.g,i.color.b,alpha);
			}
			foreach (Text t in texts) {
				t.color = new Color(t.color.r,t.color.g,t.color.b,alpha);
			}
			//highlighterController.SetLineRenderers(renderer.bounds.min,renderer.bounds.max);

			if (!childrenActivated) {
				foreach (Image i in panels)
					i.enabled = true;
				foreach (Text t in texts)
					t.enabled = true;
			}
		}
	}
	
	public void Activate() {
		bool obj = new bool();
		obj = true;
		activateQueue.Enqueue(obj);
	}
	public void Deactivate() {
		bool obj = new bool();
		obj = false;
		//activateQueue.Enqueue(obj);
	}
	private void ActivateNow() {
		active = true;
	}
	private void DeactivateNow() {
		if (active) {
			foreach (Image i in panels)
				i.enabled = false;
			foreach (Text t in texts)
				t.enabled = false;
			active = false;
			childrenActivated = false;
		}
	}
}
