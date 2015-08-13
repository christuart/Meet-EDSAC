using UnityEngine;
using System.Collections;

public class GUIClearOnAwake : MonoBehaviour {

	public GUIText GUITextComponent;

	void Awake() {
		if (GUITextComponent != null) GUITextComponent.text = "";
		Destroy(this);
	}
}
