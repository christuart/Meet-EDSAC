using UnityEngine;
using System.Collections;

public class ViewPointMeshCameraController : MonoBehaviour {

	public bool usingViewPointMeshSystem = true;

	public ViewPointMeshVertex startingVertex;

	public float slideRate = 0.3f;
	public float slideThresh = 0.02f;

	public float maxSpeed = 1f;

	public ViewPointMeshVertex vert;

	public float closeDepthOfField = 1.77f;
	public float farDepthOfField = 2.77f;
	private float targetDepthOfField;
	
	private Vector3 targetPosition;
	private Quaternion targetRotation;
	private Vector3 lastPosition;

	public bool nudging = false;
	public Vector3 nudge;
	public enum NudgeDirection { LEFT,RIGHT,UP,DOWN,NONE };
	private NudgeDirection nudgeDirection = NudgeDirection.NONE;

	public UnityStandardAssets.ImageEffects.DepthOfField depthOfFieldScript;

	public bool continuousTarget;

	void Start() {
		targetDepthOfField = depthOfFieldScript.focalLength;
		if (startingVertex != null && vert == null) {
			GoToVertex(startingVertex);
		}
	}

	void Update () {
		if (continuousTarget) {
			targetPosition = vert.transform.position;
		}
		if (nudging || (transform.position-targetPosition).magnitude > slideThresh) {

			transform.position = Vector3.Lerp(transform.position,targetPosition+nudge,slideRate*20f*Time.deltaTime);

			float speedRatio = ((transform.position+nudge-lastPosition).magnitude/Time.deltaTime)/maxSpeed;
			if (speedRatio > 1f)
				transform.position = lastPosition + (transform.position+nudge-lastPosition) / speedRatio;

			float slerpT = ((targetPosition+nudge-lastPosition) == Vector3.zero) ? .5f :(transform.position-lastPosition).magnitude / (targetPosition+nudge-lastPosition).magnitude;
			transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,slerpT);

			lastPosition = transform.position;

		}
		if ((depthOfFieldScript.focalLength-targetDepthOfField) > slideThresh) {
			depthOfFieldScript.focalLength = Mathf.Lerp(depthOfFieldScript.focalLength,targetDepthOfField,slideRate*20f*Time.deltaTime);
		}
	}

	public void GoToVertex(ViewPointMeshVertex target) {
		vert = target;
		targetPosition = target.transform.position;
		targetRotation = target.transform.rotation;
		targetDepthOfField = target.isCloseToObjects ? closeDepthOfField : farDepthOfField;
	}
	
	public IEnumerator Nudge(NudgeDirection _nudgeDirection) {

		// if we're already nudging left, don't add another over the top
		if (nudgeDirection == _nudgeDirection || _nudgeDirection == NudgeDirection.NONE)
			yield break;

		// start nudging left
		nudgeDirection = _nudgeDirection;
		nudging = true;
		Vector3 nudgeVector = Vector3.zero;
		switch (nudgeDirection) {
		case NudgeDirection.LEFT:
			nudgeVector = Vector3.left;
			break;
		case NudgeDirection.RIGHT:
			nudgeVector = Vector3.right;
			break;
		case NudgeDirection.UP:
			nudgeVector = Vector3.up;
			break;
		case NudgeDirection.DOWN:
			nudgeVector = Vector3.down;
			break;
		}
		nudgeVector = .1f * (targetRotation * nudgeVector);

		for (float i = 0.2f; i < 1f; i+=0.2f) {
			// make sure nobody else has nudged us another way
			if (nudgeDirection == _nudgeDirection) {
				nudge = nudgeVector * (Mathf.Sin (Mathf.PI * i));
				yield return null;
			} else {
				// if a different direction has started, stop this one
				yield break;
			}
		}

		// if we get here uninterrupted, finish the nudging so another left could start
		nudgeDirection = NudgeDirection.NONE;
		nudging = false;
		nudge = Vector3.zero;

	}

}
