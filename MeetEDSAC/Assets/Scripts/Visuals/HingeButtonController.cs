using UnityEngine;
using System.Collections;

public class HingeButtonController : MonoBehaviour {

	public Controller controller;
	public WorldSpaceCanvasHinge hinge;

	public bool hingeOut = true;
	public bool leftHandHinge = true;
	
	public float buttonPositionWhenOut = 173.62f;
	public float buttonScaleWhenOut = 0.2f;
	public float buttonPositionWhenIn = 426f;
	public float buttonScaleWhenIn = 0.1f;

	public float slide = 0.4f;
	public float move = 20f;
	public float moveSlideThresh = 80f;

	private float targetX;
	private Vector3 targetScale;
	private float targetRotation = 0f;
		
	void Start() {
		ToggleHinge();
		ToggleHinge();
		// this makes sure it is being drawn correctly for the starting condition
	}

	void Update() {

		targetX = (leftHandHinge ? -1 : 1) * Screen.width * (hingeOut ? buttonPositionWhenOut : buttonPositionWhenIn);
		targetScale = hingeOut ? new Vector3(buttonScaleWhenOut,buttonScaleWhenOut,buttonScaleWhenOut) : new Vector3(buttonScaleWhenIn,buttonScaleWhenIn,buttonScaleWhenIn);
		if (Mathf.Abs(transform.localPosition.x-targetX) > moveSlideThresh) {
			transform.localPosition = new Vector3 (Mathf.MoveTowards (transform.localPosition.x,targetX,move*20f*Time.deltaTime),transform.localPosition.y,transform.localPosition.z);
		} else {
			transform.localPosition = new Vector3 (Mathf.Lerp (transform.localPosition.x,targetX,slide*20f*Time.deltaTime),transform.localPosition.y,transform.localPosition.z);
		}
		transform.localScale = Vector3.Lerp (transform.localScale,targetScale,slide*20f*Time.deltaTime);
		transform.eulerAngles = new Vector3(0f,0f,Mathf.Lerp (transform.eulerAngles.z,targetRotation,slide*20f*Time.deltaTime));

	}

	public void ToggleHinge() {
		if (hingeOut) {
			hingeOut = false;
			if (leftHandHinge) {
				controller.OnInfoHingeAway(); 
			}else { 
				controller.OnInspectorHingeAway(); 
			}
			hinge.MoveHingeAway();
			transform.eulerAngles = leftHandHinge ? new Vector3(0f,0f, -180f) : Vector3.zero;
			targetRotation = leftHandHinge ? 0f : 180f;
			controller.OnHingeAway(leftHandHinge);
		} else {
			hingeOut = true;
			if (leftHandHinge) { 
				controller.OnInfoHingeOut(); 
			} else {
				controller.OnInspectorHingeOut(); 
			}
			hinge.MoveHingeOut();
			transform.eulerAngles = leftHandHinge ? Vector3.zero : new Vector3(0f,0f, -180f);
			targetRotation = leftHandHinge ? 180f : 0f;
			controller.OnHingeOut(leftHandHinge);
		}
	}
}
