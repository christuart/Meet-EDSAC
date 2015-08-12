using UnityEngine;
using System.Collections;

public class GUIClearOnAwake : MonoBehaviour {

	public GUIText guiText;

	void Awake() {
		if (guiText != null) guiText.text = "";
		Destroy(this);
	}
}
