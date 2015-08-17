using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class HingePanelDoubleClickController : MonoBehaviour {

	public HingeButtonController hingeButton;
	public float doubleClickSpeed = 0.6f;

	private float lastClick;

	public void ReceiveClick(BaseEventData data) {
		PointerEventData pData = (PointerEventData)data;
		if (pData.button == PointerEventData.InputButton.Left) {
			if (Time.time - lastClick < doubleClickSpeed) {
				lastClick = 0f; // so that you can't click a third time and get another double click
				hingeButton.ToggleHinge();
			} else {
				lastClick = Time.time;
			}
		}
	}
}
