using UnityEngine;
using System.Collections;

public class ObscurableObjectsController : MonoBehaviour {
	
	private Renderer[] renderers;

	void Start() {
		renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in renderers) {
			rend.material.renderQueue = 2002; // set their renderQueue
		}
	}


}
