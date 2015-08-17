using UnityEngine;
using System.Collections;

public class MouseDragController : MonoBehaviour {

	public float dragBaseRepeatTime = 1f;
	public float dragStepDistance = 50f;
	public int maxDragSteps = 3;

	public GameObject dragMarker;

	public bool canDrag = false;
	public bool wasDragging = false;

	private Vector2 dragStartPos;
	private float lastDragTime = 0f;
	
	private static float tan15 = 0.26795f;
	private static float tan75 = 3.73205f;
	
	private bool isDraggingLeft = false;
	private bool isDraggingRight = false;
	private bool isDraggingUp = false;
	private bool isDraggingDown = false;
	
	public bool GetDraggingLeft() {
		return isDraggingLeft;
	}
	public bool GetDraggingRight() {
		return isDraggingRight;
	}
	public bool GetDraggingUp() {
		return isDraggingUp;
	}
	public bool GetDraggingDown() {
		return isDraggingDown;
	}

	// Update is called once per frame
	void Update () {
		isDraggingLeft = false;
		isDraggingRight = false;
		isDraggingUp = false;
		isDraggingDown = false;
		if (canDrag && Input.GetMouseButton(2)) {
			if (!wasDragging) {
				dragStartPos = Input.mousePosition;
				dragMarker.transform.localPosition = dragStartPos - new Vector2(Screen.width/2, Screen.height/2);
				dragMarker.SetActive(true);
			}
			wasDragging = true;
			Vector2 dragVector = (Vector2)Input.mousePosition - dragStartPos;
			float dragTan = Mathf.Abs(dragVector.x / dragVector.y);
			if (dragTan < tan15 || dragTan > tan75) {
				int steps = Mathf.FloorToInt(dragVector.magnitude / dragStepDistance);
				if (steps > 0) {
					steps = Mathf.Min (steps,maxDragSteps);
					if (Time.time - lastDragTime > dragBaseRepeatTime / steps) {
						lastDragTime = Time.time;
						if (dragVector.x > dragVector.y && dragVector.x > -(dragVector.y)) {
							isDraggingRight = true;
						} else if (dragVector.x < dragVector.y && dragVector.x < -(dragVector.y)) {
							isDraggingLeft = true;
						} else if (dragVector.y > dragVector.x && dragVector.y > -(dragVector.x)) {
							isDraggingUp = true;
						} else if (dragVector.y < dragVector.x && dragVector.y < -(dragVector.x)) {
							isDraggingDown = true;
						}
					}
				}
			}
		} else {
			dragMarker.SetActive(false);
			wasDragging = false;
		}
	}
}
