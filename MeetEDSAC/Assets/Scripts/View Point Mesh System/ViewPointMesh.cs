using UnityEngine;
using System.Collections;

public class ViewPointMesh : MonoBehaviour {

	public ViewPointMeshVertex defaultVertex;
	public bool showGizmo = true;
	public bool showAllBuilderGizmos = false;

	void OnDrawGizmos() {
		if (showGizmo) {
			ViewPointMeshVertex[] points = GetComponentsInChildren<ViewPointMeshVertex> ();
			foreach (ViewPointMeshVertex vert in points) {
				Gizmos.color = Color.red;
				float scale = 0.02f;
				float arg1, arg2;
				for (int i = 0; i < 4; i++) {
					arg1 = Mathf.PI*(0.5f+i)/2f;
					arg2 = Mathf.PI*(1.5f+i)/2f;
					Gizmos.DrawLine (vert.transform.position + vert.transform.right * 2 * scale * Mathf.Sin (arg1) + vert.transform.up * scale * Mathf.Cos (arg1),
					                 vert.transform.position + vert.transform.right * 2 * scale * Mathf.Sin (arg2) + vert.transform.up * scale * Mathf.Cos (arg2));
					Gizmos.DrawLine (vert.transform.position + vert.transform.right * 4 * scale * Mathf.Sin (arg1) + vert.transform.up * 2 * scale * Mathf.Cos (arg1) + vert.transform.forward * 5 * scale,
					                 vert.transform.position + vert.transform.right * 4 * scale * Mathf.Sin (arg2) + vert.transform.up * 2 * scale * Mathf.Cos (arg2) + vert.transform.forward * 5 * scale);
					Gizmos.DrawLine (vert.transform.position + vert.transform.right * 2 * scale * Mathf.Sin (arg1) + vert.transform.up * scale * Mathf.Cos (arg1),
					                 vert.transform.position + vert.transform.right * 4 * scale * Mathf.Sin (arg1) + vert.transform.up * 2 * scale * Mathf.Cos (arg1) + vert.transform.forward * 5 * scale);
					
				}

				Gizmos.color = Color.yellow;
				ViewPointMeshVertex[] connectedPoints = {vert.Left(), vert.Right (), vert.Up (), vert.Down()};
				foreach (ViewPointMeshVertex connectedPoint in connectedPoints) {
					Gizmos.DrawLine(vert.transform.position,connectedPoint.transform.position);
				}
			}
		}
		foreach (ViewPointMeshBuilder builder in GetComponentsInChildren<ViewPointMeshBuilder>())
			builder.DrawGizmo(showAllBuilderGizmos);
	}

	void Update() {
		foreach (ViewPointMeshVertex v in GetComponentsInChildren<ViewPointMeshVertex>())
			v.RepopulateAssociatedLabels();
		enabled = false;
	}
}
