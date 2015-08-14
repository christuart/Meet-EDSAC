/*
 * Eventual order of generating content will need to be:
 * 
 * Use xml parser to create the:
 * 		Chassis
 * 		Valves
 * 		Chassis label targets
 * 		Chassis labels
 * Copypaste these to editor
 * 
 * Complete all the high level (row) and medium level (when
 * a row contains more than one logical section) label
 * and labels.
 * 
 * Complete all the final info panel content
 * 
 * Use mesh builders
 * 		Assign info content to mesh builders
 * 		Assign labels to mesh builders
 * 		Generate meshes and copypaste to Editor
 * 		REMOVE the associated label parents from
 *	 		mesh vertices because otherwise they
 *			will overwrite your allocated labels
 * 
 * Connect meshes together using the zoom model
 * 
 */

using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	public KinectInfoInterpreter firstPlayerKinectInput;

	public ViewPointMeshCameraController meshSystemCameraController;

	public CameraZoomController cameraZoom;

	public InfoHolderController infoHolder;
	public InspectorController inspector;

	public ScreenEngagementFeedbackController engagementController;

	private FacetrackingManager faceTrack;

	private ViewPointMeshVertex activeVertex;
	private Queue vertexTargets;

	public bool sendFace = false;

	// Use this for initialization
	void Awake () {
		vertexTargets = new Queue();
	}
	
	// Update is called once per frame
	void Update () {

		while (vertexTargets.Count > 1) {
			
			OnBeforeChangeVertex();

			activeVertex = (ViewPointMeshVertex)vertexTargets.Dequeue();
			meshSystemCameraController.GoToVertex(activeVertex);
			if ((bool)vertexTargets.Dequeue()) {
				if (activeVertex.informationContent != InformationContent.NONE) {
					infoHolder.PlaceObjectInInfoUI(activeVertex.informationContent);
				}
			}
			foreach (LabelController lc in GameObject.FindObjectsOfType<LabelController>()) {
				lc.Deactivate();
			}
			foreach (LabelController lc in activeVertex.associatedLabels) {
				lc.Activate();
			}
			
			OnAfterChangeVertex();

		}

		if (Input.GetKeyDown(KeyCode.D) || firstPlayerKinectInput.GetGestureTriggered(EdsacGestures.LEFT_SWIPE)) {
			OnPanRight();
		}
		if (Input.GetKeyDown(KeyCode.A) || firstPlayerKinectInput.GetGestureTriggered(EdsacGestures.RIGHT_SWIPE)) {
			OnPanLeft();
		}
		if (Input.GetKeyDown(KeyCode.W) || firstPlayerKinectInput.GetGestureTriggered(EdsacGestures.DOWN_SWIPE)) {
			OnPanUp();
		}
		if (Input.GetKeyDown(KeyCode.S) || firstPlayerKinectInput.GetGestureTriggered(EdsacGestures.UP_SWIPE)) {
			OnPanDown();
		}
		if (Input.GetKeyDown(KeyCode.Q)) {
			cameraZoom.ZoomIn(ZoomSettings.ZoomSource.KEYBOARD);
			OnZoomIn();
		}
		if (Input.mouseScrollDelta.y > 0) {
			cameraZoom.ZoomIn(ZoomSettings.ZoomSource.MOUSE);
			OnZoomIn();
		}
		if (firstPlayerKinectInput.GetGestureTriggered(EdsacGestures.STRETCH)) {
			cameraZoom.ZoomIn(ZoomSettings.ZoomSource.KINECT);
			OnZoomIn();
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			cameraZoom.ZoomOut(ZoomSettings.ZoomSource.KEYBOARD);
			OnZoomOut();
		}
		if (Input.mouseScrollDelta.y < 0) {
			cameraZoom.ZoomOut(ZoomSettings.ZoomSource.MOUSE);
			OnZoomOut();
		}
		if (firstPlayerKinectInput.GetGestureTriggered(EdsacGestures.SQUASH)) {
			cameraZoom.ZoomOut(ZoomSettings.ZoomSource.KINECT);
			OnZoomOut();
		}
		if (Input.GetKeyDown(KeyCode.V)) {
			engagementController.UseSingleEngagementRegion(true);
			sendFace = false;
			engagementController.SetPanelEngaged(true);
		}
		if (Input.GetKeyDown(KeyCode.B)) {
			engagementController.UseSingleEngagementRegion(true);
			sendFace = false;
			engagementController.SetModelEngaged();
		}
		if (Input.GetKeyDown(KeyCode.N)) {
			engagementController.UseSingleEngagementRegion(true);
			sendFace = false;
			engagementController.SetPanelEngaged(false);
		}
		if (Input.GetKeyDown(KeyCode.U)) {
			engagementController.UseSingleEngagementRegion(false);
			engagementController.DontUseEngagementRegions();
			sendFace = false;
			engagementController.SetNeutral();
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			engagementController.UseSingleEngagementRegion(true);
			sendFace = true;
		}

		if (sendFace) {
			if (firstPlayerKinectInput.GetGazeDirectionAvailable()) {
				Debug.Log (firstPlayerKinectInput.GetGazeDirection());
				//engagementController.SetSingleEngagementInput (firstPlayerKinectInput.GetUserFaceDirection());
			}
		}

	}
	
	/************************************************
	/* PUBLIC GAME EVENTS		
	// These are really badly implemented. Some of
	// them are what causes the event to do
	// anything, some of them are just for adding
	// extra stuff and the event itself is carried
	// out elsewhere. Sorry not sorry.				*/

	public void OnPanLeft() {
		ActivateVertex(activeVertex.Left ());
	}
	public void OnPanRight() {
		ActivateVertex(activeVertex.Right ());
	}
	public void OnPanUp() {
		ActivateVertex(activeVertex.Up ());
	}
	public void OnPanDown() {
		ActivateVertex(activeVertex.Down ());
	}
	public void OnBeforeChangeVertex() {
	}
	public void OnAfterChangeVertex() {
	}
	public void OnZoomIn() {
		if (cameraZoom.GetZoom() <= activeVertex.exitByZoomFieldOfView) {
			Debug.Log (activeVertex.moreZoomedBuilder.name);
			ViewPointMeshVertex nextVertex = activeVertex.ClosestMatch (activeVertex.moreZoomedBuilder);
			ActivateVertex(nextVertex);
			// REMEMEMEMEMEMEBERRR ActivateVertex *QUEUES* the activation of another vertex, so activeVertex hasn't changed yet for next line
			cameraZoom.SetZoom(nextVertex.entryByZoomFieldOfView);
		}
	}
	public void OnZoomOut() {
		if (activeVertex.entryByZoomFieldOfView != 0f && cameraZoom.GetZoom() >= activeVertex.entryByZoomFieldOfView) {
			Debug.Log (activeVertex.lessZoomedBuilder.name);
			ViewPointMeshVertex nextVertex = activeVertex.ClosestMatch (activeVertex.lessZoomedBuilder);
			ActivateVertex(nextVertex);
			// REMEMEMEMEMEMEBERRR ActivateVertex *QUEUES* the activation of another vertex, so activeVertex hasn't changed yet for next line
			cameraZoom.SetZoom(nextVertex.exitByZoomFieldOfView); // rememeber, naming is set for zooming in, so zoomout must use exit when it enters
		}
	}
	public void OnHingeOut(bool leftHandHinge) {
		if (leftHandHinge) {
			OnInfoHingeOut();
		} else {
			OnInspectorHingeOut();
		}
	}
	public void OnInfoHingeOut() {
	}
	public void OnInspectorHingeOut() {
	}
	public void OnHingeAway(bool leftHandHinge) {
		if (leftHandHinge) {
			OnInfoHingeAway();
		} else {
			OnInspectorHingeAway();
		}
	}
	public void OnInfoHingeAway() {
	}
	public void OnInspectorHingeAway() {
	}
	public void OnNewFirstPlayer() {
	}
	public void OnNewSecondPlayer() {
	}

	
	/*****************************
	// PUBLIC GAME FUNCTIONS	*/
	public void ActivateVertex(ViewPointMeshVertex vert, bool openInfo = true) {
		vertexTargets.Enqueue(vert);
		vertexTargets.Enqueue(openInfo);
	}
	public void SetFirstPlayerId(long userId) {
		firstPlayerKinectInput.SetUserId(userId);
	}
	public void SetSecondPlayerId(long userId) {
		firstPlayerKinectInput.SetUserId(userId);
	}

}
