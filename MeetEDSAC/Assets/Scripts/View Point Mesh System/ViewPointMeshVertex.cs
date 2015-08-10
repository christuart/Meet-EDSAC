using UnityEngine;
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

}
