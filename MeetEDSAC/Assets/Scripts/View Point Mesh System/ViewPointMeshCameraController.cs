using UnityEngine;
using System.Collections;

public class ViewPointMeshCameraController : MonoBehaviour {

	public bool usingViewPointMeshSystem = true;

	public ViewPointMeshVertex startingVertex;

	public float slideRate = 0.3f;
	public float slideThresh = 0.02f;

	public ViewPointMeshVertex vert;

	public float closeDepthOfField = 1.77f;
	public float farDepthOfField = 2.77f;
	private float targetDepthOfField;
	
	private Vector3 targetPosition;
	private Quaternion targetRotation;

	public UnityStandardAssets.ImageEffects.DepthOfField depthOfFieldScript;

	void Start() {
		targetDepthOfField = depthOfFieldScript.focalLength;
		if (startingVertex != null && vert == null) {
			GoToVertex(startingVertex);
		}
	}

	void Update () {
		if ((transform.position-targetPosition).magnitude > slideThresh) {
			transform.position = Vector3.Lerp(transform.position,targetPosition,slideRate);
			transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,slideRate);
		}
		if ((depthOfFieldScript.focalLength-targetDepthOfField) > slideThresh) {
			depthOfFieldScript.focalLength = Mathf.Lerp(depthOfFieldScript.focalLength,targetDepthOfField,slideRate);
		}
	}

	public void GoToVertex(ViewPointMeshVertex target) {
		vert = target;
		targetPosition = target.transform.position;
		targetRotation = target.transform.rotation;
		targetDepthOfField = target.isCloseToObjects ? closeDepthOfField : farDepthOfField;
	}

}
