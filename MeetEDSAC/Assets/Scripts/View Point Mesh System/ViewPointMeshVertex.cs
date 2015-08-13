﻿using UnityEngine;
using System.Collections;

public class ViewPointMeshVertex : MonoBehaviour {

	public ViewPointMeshVertex left;
	public ViewPointMeshVertex right;
	public ViewPointMeshVertex up;
	public ViewPointMeshVertex down;

	public InformationContent informationContent = InformationContent.NONE;
	public LabelController[] associatedLabels;
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
		if (associatedLabelParent != null)
			associatedLabels = associatedLabelParent.GetComponentsInChildren<LabelController>();
		foreach (LabelController lc in associatedLabels)
			lc.Deactivate();
	}
	
	public ViewPointMeshVertex Left() {
		if (left != null)
			return left;
		return this;
	}
	public ViewPointMeshVertex Right() {
		if (right != null)
			return right;
		return this;
	}
	public ViewPointMeshVertex Up() {
		if (up != null)
			return up;
		return this;
	}
	public ViewPointMeshVertex Down() {
		if (down != null)
			return down;
		return this;
	}

	public ViewPointMeshVertex ClosestMatch(ViewPointMeshBuilder builder) {
		return builder.ClosestMatch(this);
	}

}