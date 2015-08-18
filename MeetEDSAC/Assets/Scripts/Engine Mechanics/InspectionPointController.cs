using UnityEngine;
using System.Collections;

public class InspectionPointController : MonoBehaviour {

	public CanvasGroup group;

	public float alphaSlide = .1f;
	public float alphaThresh = .02f;

	public float showFor = 4f;
	public float fadeFor = 1.5f;

	public float maxAlpha = .4f;

	public string onClick;
	
	private float alphaTarget;
	private float pointerLeftAt = 0f;

	private bool hovering = false;

	// Use this for initialization
	void Start () {
		group.alpha = 0f;
		alphaTarget = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (group.alpha - alphaTarget) < alphaThresh) {
			group.alpha = alphaTarget;
		} else {
			group.alpha = Mathf.Lerp (group.alpha,alphaTarget,alphaSlide*60f*Time.deltaTime);
		}
		if (!hovering) {
			float timeSince = Time.time - pointerLeftAt;
			if (timeSince < showFor + fadeFor) {
				if (timeSince > showFor) {
					alphaTarget = maxAlpha * (1f - (timeSince - showFor) / fadeFor);
				}
			} else {
				alphaTarget = 0f;
			}
		}
	}
	
	public void HoverEnter() {
		alphaTarget = maxAlpha;
		hovering = true;
		GetComponent<Animator>().SetTrigger("HoverEnter");
	}
	public void HoverLeave() {
		alphaTarget = maxAlpha;
		hovering = false;
		pointerLeftAt = Time.time;
		GetComponent<Animator>().SetTrigger("HoverExit");
	}
	public void Choose() {
		Debug.Log ("Activated a message! \"" + onClick + "\"");
	}
}
