using UnityEngine;
using System.Collections;
using UnityEditor;

public class ViewPointMeshVertex : MonoBehaviour {
	
	public bool viewInSceneView;

	public ViewPointMeshVertex left;
	public ViewPointMeshVertex right;
	public ViewPointMeshVertex up;
	public ViewPointMeshVertex down;

	public InformationContent informationContent = InformationContent.NONE;
	public LabelController[] associatedLabels;
	public InspectionPointController[] associatedInspectionPoints;
	public GameObject associatedLabelParent;
	public bool isCloseToObjects;


	/* These values are set as if we were zooming in, but apply for the opposite job
	 * when zooming out. 0f suggests that there is no mesh to zoom in/out to.
	 */

	public float entryByZoomFieldOfView = 0f; 
	// this is the field of view that we will adopt when we've entered because we zoomed too much

	public float exitByZoomFieldOfView = 0f;
	// this is the field of view value to exit at

	public ViewPointMeshBuilder lessZoomedBuilder;
	// this is what we come from for entryByZoomFieldOfView

	public ViewPointMeshBuilder moreZoomedBuilder;
	// this is what we come from for exitByZoomFieldOfView


	//public Action SelectAction

	void Start() {
		RepopulateAssociatedLabels();
		if (associatedLabels != null) {
			foreach (LabelController lc in associatedLabels)
				lc.Deactivate();
		}
	}
	
	public virtual ViewPointMeshVertex Left() {
		if (left != null)
			return left;
		return this;
	}
	public virtual ViewPointMeshVertex Right() {
		if (right != null)
			return right;
		return this;
	}
	public virtual ViewPointMeshVertex Up() {
		if (up != null)
			return up;
		return this;
	}
	public virtual ViewPointMeshVertex Down() {
		if (down != null)
			return down;
		return this;
	}

	public ViewPointMeshVertex ClosestMatch(ViewPointMeshBuilder builder) {
		return builder.ClosestMatch(this);
	}

	public void RepopulateAssociatedLabels() {
		if (associatedLabelParent != null)
			associatedLabels = associatedLabelParent.GetComponentsInChildren<LabelController>();
	}

	void OnDrawGizmos() {
		if (viewInSceneView) {
			SceneView.lastActiveSceneView.pivot = transform.position;
			SceneView.lastActiveSceneView.rotation = transform.rotation;
			SceneView.lastActiveSceneView.Repaint();
		}
	}
}
