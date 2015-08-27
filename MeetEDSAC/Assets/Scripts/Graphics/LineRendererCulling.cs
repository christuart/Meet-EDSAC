using UnityEngine;
using System.Collections;

public class LineRendererCulling : MonoBehaviour {
	WireRenderer wr;
	LineRenderer lr;
	void Start() {
		wr = GetComponent<WireRenderer>();
		lr = GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	public void UpdateLineRenderers(Transform cameraNewPosition) {
		Vector3 offsetCameraPosition = cameraNewPosition.position + .3f * cameraNewPosition.forward;
		if (Vector3.Dot (cameraNewPosition.forward,(wr.start - offsetCameraPosition).normalized) > 0.15f ||
		    Vector3.Dot (cameraNewPosition.forward,(wr.start + wr.across - offsetCameraPosition).normalized) > 0.15f) {
			lr.enabled = true;
		} else {
			lr.enabled = false;
		}
	}

}
