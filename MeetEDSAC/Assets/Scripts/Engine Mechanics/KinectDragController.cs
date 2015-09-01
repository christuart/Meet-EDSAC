using UnityEngine;
using System.Collections;

public class KinectDragController : MonoBehaviour {

	private KinectManager kinect;
	public bool isLeftHand;
	public KinectInterop.JointType handJoint;
	public long userId;

	public float dragBaseRepeatTime = 1f;
	public float dragStepDistance = 0.2f;
	public int maxDragSteps = 5;

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

	void Start() {
		if (isLeftHand) {
			handJoint = KinectInterop.JointType.HandTipLeft;
		} else {
			handJoint = KinectInterop.JointType.HandTipRight;
		}
	}

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
		kinect = KinectManager.Instance;
		isDraggingLeft = false;
		isDraggingRight = false;
		isDraggingUp = false;
		isDraggingDown = false;
		if (canDrag && HandIsGrabbing(kinect)) {
			if (!wasDragging) {
				dragStartPos = Tools.Vector2fromXZ(kinect.GetJointPosition(userId,(int)handJoint));
			}
			wasDragging = true;
			Vector2 dragVector = Tools.Vector2fromXZ(kinect.GetJointPosition(userId,(int)handJoint)) - dragStartPos;
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
			wasDragging = false;
		}
	}

	private bool HandIsGrabbing(KinectManager km) {
		if (isLeftHand) {
			return km.GetLeftHandState(userId) == KinectInterop.HandState.Closed;
		} else {
			return km.GetRightHandState(userId) == KinectInterop.HandState.Closed;
		}
	}
}
