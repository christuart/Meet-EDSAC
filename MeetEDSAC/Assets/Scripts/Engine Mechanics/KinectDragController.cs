using UnityEngine;
using System.Collections;

public class KinectDragController : MonoBehaviour {

	private KinectManager kinect;
	private Controller controller;
	public KinectInfoInterpreter kinectInterpreter;

	public bool isLeftHand;
	public KinectInterop.JointType handJoint;
	public long userId;

	public float dragBaseRepeatTime = 1f;
	public float dragStepDistance = 0.2f;
	public int maxDragSteps = 5;

	public bool canDrag = false;
	public int dragBuffer = 0;
	private int dragBufferTriggerSize = 3; // say =3, that means that on the 3rd consecutive hand close step, dragging will start
	public bool wasDragging = false;

	private Vector2 dragStartPos;
	private float lastDragTime = 0f;
	
	private static float tan15 = 0.26795f;
	private static float tan75 = 3.73205f;
	
	private bool isDraggingLeft = false;
	private bool isDraggingRight = false;
	private bool isDraggingUp = false;
	private bool isDraggingDown = false;

	void Awake() {
		controller = Object.FindObjectOfType<Controller> ();
	}

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
		if (!kinectInterpreter.useGamepad) {
			isDraggingLeft = false;
			isDraggingRight = false;
			isDraggingUp = false;
			isDraggingDown = false;
			if (canDrag && HandIsGrabbing (kinect)) {
				if (!wasDragging) {
					dragStartPos = (Vector2)(kinect.GetJointPosition (userId, (int)handJoint));
					controller.OnHandClosed (userId);
				}
				wasDragging = true;
				Vector2 dragVector = (Vector2)(kinect.GetJointPosition (userId, (int)handJoint)) - dragStartPos;
				float dragTan = Mathf.Abs (dragVector.x / dragVector.y);
				if (dragTan < tan15 || dragTan > tan75) {
					int steps = Mathf.FloorToInt (dragVector.magnitude / dragStepDistance);
					if (steps > 0) {
						steps = Mathf.Min (steps, maxDragSteps);
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
			} else if (wasDragging) {
				wasDragging = false;
				controller.OnHandOpened (userId);
			}
		} else {
			wasDragging = HandIsGrabbing (kinect);
			isDraggingLeft = Input.GetAxis("Horizontal" + kinectInterpreter.gamepad) > 0f;
			isDraggingRight = Input.GetAxis("Horizontal" + kinectInterpreter.gamepad) < 0f;
			isDraggingUp = Input.GetAxis("Vertical" + kinectInterpreter.gamepad) > 0f;
			isDraggingDown = Input.GetAxis("Vertical" + kinectInterpreter.gamepad) < 0f;
		}
	}

	private bool HandIsGrabbing(KinectManager km) {
		if (kinectInterpreter.useGamepad) {
			if (isLeftHand) {
				return Input.GetAxis("Horizontal" + kinectInterpreter.gamepad) != 0f || Input.GetAxis("Vertical" + kinectInterpreter.gamepad) != 0f;
			} else {
				return false;
			}
		} else {
			if (isLeftHand) {
				if (km.GetLeftHandState (userId) != KinectInterop.HandState.Closed || km.GetRightHandState (userId) == KinectInterop.HandState.Closed) {
					dragBuffer = 0;
					return false;
				}
			} else {
				if (km.GetRightHandState (userId) != KinectInterop.HandState.Closed || km.GetLeftHandState (userId) == KinectInterop.HandState.Closed) {
					dragBuffer = 0;
					return false;
				}
			}
			if (dragBuffer < dragBufferTriggerSize)
				dragBuffer++;
			return dragBuffer == dragBufferTriggerSize;
		}
	}
}
