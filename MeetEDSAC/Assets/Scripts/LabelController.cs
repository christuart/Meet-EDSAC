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
	public float power = 2;

	public bool considerNormal = true;

	private Image[] panels;
	private Text[] texts;
	public bool atRight = false;
	public bool atTop = false;
	public float rightX;
	public float topY;
	public Vector2 initialPosMin;
	public Vector2 initialPosMax;

	// Use this for initialization
	void Start () {
		panels = GetComponentsInChildren<Image>();
		texts = GetComponentsInChildren<Text>();
		initialPosMin = nameTag.offsetMin;
		initialPosMax = nameTag.offsetMax;
	}
	
	// Update is called once per frame
	void Update () {
		Vector4 minMaxViewport = Tools.ViewportFromBounds(targetForLabel.bounds.min,targetForLabel.bounds.max,viewCamera);
		Vector2 min = viewCamera.ViewportToScreenPoint(new Vector2(minMaxViewport.x,minMaxViewport.y));
		Vector2 max = viewCamera.ViewportToScreenPoint(new Vector2(minMaxViewport.z,minMaxViewport.w));
		
		nameTag.offsetMin = initialPosMin;
		nameTag.offsetMax = initialPosMax;
		Vector2 offMin = nameTag.offsetMin;
		Vector2 offMax = nameTag.offsetMax;
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
		nameTag.offsetMin = offMin;
		nameTag.offsetMax = offMax;
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
		nameTag.offsetMin = offMin;
		nameTag.offsetMax = offMax;

		label.offsetMax = max + panelMargin;
		label.offsetMin = min - panelMargin;
		float alpha = 1.0f-Mathf.Clamp((Tools.MinDistanceFromBounds(targetForLabel.bounds.min,targetForLabel.bounds.max,viewCamera)-oneAlphaDistance)/(zeroAlphaDistance-oneAlphaDistance),0f,1f-minAlpha);
		if (considerNormal) {
			alpha *= (Vector3.Dot (viewCamera.transform.forward,(targetForLabel.transform.position-viewCamera.transform.position).normalized));
		}
		alpha = Mathf.Pow(alpha,power);
		foreach (Image i in panels) {
			i.color = new Color(i.color.r,i.color.g,i.color.b,alpha);
		}
		foreach (Text t in texts) {
			t.color = new Color(t.color.r,t.color.g,t.color.b,alpha);
		}
		//highlighterController.SetLineRenderers(renderer.bounds.min,renderer.bounds.max);

	}
}
