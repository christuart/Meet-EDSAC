using UnityEngine;
using System.Collections;

public class LoadingPanelFadeOutController : MonoBehaviour {
	
	[Range(0,1)]
	public float startAlpha = 0.98f;

	private CanvasGroup group;
	private int updates = 0;

	// Use this for initialization
	void Start () {
		group = GetComponent<CanvasGroup>();
		group.alpha = startAlpha;
	}
	
	// Update is called once per frame
	void Update () {
		if (updates > 5) {
			group.alpha = Mathf.Lerp (group.alpha,0f,5f * Time.deltaTime);
			if (group.alpha < 0.02f) {
				Destroy (transform.parent.gameObject);
				return;
			}
		}
		updates++;
	}
}
