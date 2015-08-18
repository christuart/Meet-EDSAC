using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FrameIndicatorController : MonoBehaviour {
		
	Image i;

	void Awake() {
		i = GetComponent<Image> ();
	}

	// Update is called once per frame
	void Update () {
		i.enabled = !i.enabled;
	}
}
