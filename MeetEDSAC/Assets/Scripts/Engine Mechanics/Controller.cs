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
using System.Collections.Generic;

public class Controller : MonoBehaviour {
	
	public KinectInfoInterpreter firstPlayerKinectInfo;
	public KinectInfoInterpreter secondPlayerKinectInfo;

	public ViewPointMeshCameraController meshSystemCameraController;

	public CameraZoomController cameraZoom;

	public InfoHolderController infoHolder;
	public InspectorController inspector;

	public ScreenEngagementFeedbackController engagementController;

	public MouseDragController mouseDrag;

	public KinectInspectionPointChooser kinectInspectionPointChooser;

	public HingeButtonController infoController;
	public HingeButtonController inspectorController;
	
	public StoryController storyController;

	private FacetrackingManager faceTrack;

	public ViewPointMeshVertex activeVertex;
	private Queue vertexTargets;

	private List<LabelController> activeLabels;

	public bool debugModes = false;
	public bool useKeyboard = true;
	public bool useKinect = true;
	public bool useFace = false;
	public bool useMouse = true;
	
	public GameObject kinectUserInfo;
	public GameObject debugOnlyGUI;

	private bool updating = false;

	// Use this for initialization
	void Awake () {
		vertexTargets = new Queue();
		activeLabels = new List<LabelController> ();
//		foreach (LabelController lc in FindObjectsOfType<LabelController>())
//			lc.Deactivate();
		Application.targetFrameRate = 60;
	}

	void Start () {
		if (useKinect) {
			foreach(InspectionPointController ipc in GameObject.FindObjectsOfType<InspectionPointController>())
				ipc.Hide();
			kinectInspectionPointChooser.EnableKinectInspectionPointLabel(true);
			if (activeVertex != null && storyController.storyMode == StoryController.StoryMode.DISABLED) {
				kinectInspectionPointChooser.FindNewInspectionPoint(activeVertex.transform.position,activeVertex.transform.rotation);
			} else if (storyController.storyMode == StoryController.StoryMode.PLAYING) {
				kinectInspectionPointChooser.chosenInspectionPoint = storyController.inspectionContent;
			}
			kinectInspectionPointChooser.ActivateChosenInspectionPointForKinect();
		}
	}
	
	// Update is called once per frame
	void Update () {

		updating = true;

		while (vertexTargets.Count > 1) {

			ViewPointMeshVertex v = (ViewPointMeshVertex)vertexTargets.Dequeue();
			bool b = (bool)vertexTargets.Dequeue();
			ActivateVertexNow(v,b);

		}
		if (useKeyboard) {
			if (Input.GetKeyDown(KeyCode.D)) {
				OnPanRight();
			}
			if (Input.GetKeyDown(KeyCode.A)) {
				OnPanLeft();
			}
			if (Input.GetKeyDown(KeyCode.W)) {
				OnPanUp();
			}
			if (Input.GetKeyDown(KeyCode.S)) {
				OnPanDown();
			}
			if (Input.GetKeyDown(KeyCode.E)) {
				cameraZoom.ZoomIn(ZoomSettings.ZoomSource.KEYBOARD);
				OnZoomIn();
			}
			if (Input.GetKeyDown(KeyCode.Q)) {
				cameraZoom.ZoomOut(ZoomSettings.ZoomSource.KEYBOARD);
				OnZoomOut();
			}
		}
		if (useKinect) {
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.LEFT_SWIPE)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanRight();
				} else if (engagementController.IsOnlyLeftPanelEngaged()) {
					if (infoController.hingeOut) infoController.ToggleHinge();
				} else if (engagementController.IsOnlyRightPanelEngaged()) {
					if (!inspectorController.hingeOut) inspectorController.ToggleHinge();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.LEFT_DRAG)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanRight();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.RIGHT_SWIPE)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanLeft();
				} else if (engagementController.IsOnlyLeftPanelEngaged()) {
					if (!infoController.hingeOut) infoController.ToggleHinge();
				} else if (engagementController.IsOnlyRightPanelEngaged()) {
					if (inspectorController.hingeOut) inspectorController.ToggleHinge();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.RIGHT_DRAG)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanLeft();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.DOWN_SWIPE)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanUp();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.DOWN_DRAG)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanUp();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.UP_SWIPE)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanDown();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.UP_DRAG)) {
				if (engagementController.IsOnlyModelEngaged()) {
					OnPanDown();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.STRETCH)) {
				if (engagementController.IsOnlyModelEngaged()) {
					cameraZoom.ZoomIn(ZoomSettings.ZoomSource.KINECT);
					OnZoomIn();
				}
			}
			if (firstPlayerKinectInfo.GetGestureTriggered(EdsacGestures.SQUASH)) {
				if (engagementController.IsOnlyModelEngaged()) {
					cameraZoom.ZoomOut(ZoomSettings.ZoomSource.KINECT);
					OnZoomOut();
				}
			}
		}
		if (useMouse) {
			if (engagementController.modelEngaged) {

				if (Input.mouseScrollDelta.y > 0) {
					cameraZoom.ZoomIn(ZoomSettings.ZoomSource.MOUSE);
					OnZoomIn();
				}
				if (Input.mouseScrollDelta.y < 0) {
					cameraZoom.ZoomOut(ZoomSettings.ZoomSource.MOUSE);
					OnZoomOut();
				}
				mouseDrag.canDrag = true;
				if (mouseDrag.GetDraggingLeft()) {
					OnPanRight();
				} else if (mouseDrag.GetDraggingRight()) {
					OnPanLeft();
				} else if (mouseDrag.GetDraggingUp()) {
					OnPanDown();
				} else if (mouseDrag.GetDraggingDown()) {
					OnPanUp();
				}
			} else {
				mouseDrag.canDrag = false;
			}
			if (!Input.GetMouseButton(2)) {
				engagementController.SetSingleEngagementInput (    Mathf.Clamp(Input.mousePosition.x / (Screen.width/2f)-1f,-1f,1f)    );
			}
		}
		if (Input.GetKeyDown(KeyCode.F3)) {
			EnableDebugModes(!debugModes);
		}
		if (debugModes) {
			if (Input.GetKeyDown(KeyCode.V)) {
				engagementController.UseSingleEngagementRegion(true);
				useFace = false;
				useMouse = false;
				engagementController.SetPanelEngaged(true);
			}
			if (Input.GetKeyDown(KeyCode.B)) {
				engagementController.UseSingleEngagementRegion(true);
				useFace = false;
				useMouse = false;
				engagementController.SetModelEngaged();
			}
			if (Input.GetKeyDown(KeyCode.N)) {
				engagementController.UseSingleEngagementRegion(true);
				useFace = false;
				useMouse = false;
				engagementController.SetPanelEngaged(false);
			}
			if (Input.GetKeyDown(KeyCode.U)) {
				engagementController.UseSingleEngagementRegion(false);
				engagementController.DontUseEngagementRegions();
				useFace = false;
				useMouse = false;
				engagementController.SetNeutral();
			}
			if (Input.GetKeyDown(KeyCode.F)) {
				engagementController.UseSingleEngagementRegion(true);
				useFace = true;
				useMouse = false;
			}
			if (Input.GetKeyDown(KeyCode.M)) {
				engagementController.UseSingleEngagementRegion(true);
				useFace = false;
				useMouse = true;
			}
			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				firstPlayerKinectInfo.UseNumpad(!firstPlayerKinectInfo.useNumpad);
			}
			if (Input.GetKeyDown (KeyCode.Alpha2)) {
				secondPlayerKinectInfo.UseNumpad(!secondPlayerKinectInfo.useNumpad);
			}
			if (Input.GetKeyDown (KeyCode.O)) {
				debugOnlyGUI.SetActive(!debugOnlyGUI.activeSelf);
			}
			if (Input.GetKeyDown (KeyCode.P)) {
				infoHolder.Scroll(true);
			}

		}

		if (useFace) {
			if (firstPlayerKinectInfo.GetGazeDirectionAvailable()) {
				//Debug.Log (firstPlayerKinectInfo.GetGazeDirection());
				engagementController.SetSingleEngagementInput (    Mathf.Clamp(firstPlayerKinectInfo.GetGazeDirection() / (Mathf.PI/4f),-1f,1f)    );
				//Mathf.Lerp(-1f,1f,0.5f+faceTrack.GetHeadRotation(false).eulerAngles.y/60f)
				//engagementController.SetSingleEngagementInput (firstPlayerKinectInfo.GetUserFaceDirection());
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
		if (storyController.storyMode == StoryController.StoryMode.PLAYING) {
			storyController.EnterPreviousWaypoint();
		} else if (activeVertex.Left() != activeVertex) {
			ActivateVertex(activeVertex.Left ());
		} else if (storyController.storyMode == StoryController.StoryMode.DISABLED){
			StartCoroutine(meshSystemCameraController.Nudge(ViewPointMeshCameraController.NudgeDirection.LEFT));
		}
	}
	public void OnPanRight() {
		if (storyController.storyMode == StoryController.StoryMode.PLAYING) {			
			storyController.EnterNextWaypoint();
		} else if (activeVertex.Right() != activeVertex) {
			ActivateVertex(activeVertex.Right ());
		} else if (storyController.storyMode == StoryController.StoryMode.DISABLED){
			StartCoroutine(meshSystemCameraController.Nudge(ViewPointMeshCameraController.NudgeDirection.RIGHT));
		}
	}
	public void OnPanUp() {
		if (storyController.storyMode == StoryController.StoryMode.PLAYING) {

		} else if (activeVertex.Up() != activeVertex) {
			ActivateVertex(activeVertex.Up ());
		} else if (storyController.storyMode == StoryController.StoryMode.DISABLED){
			StartCoroutine(meshSystemCameraController.Nudge(ViewPointMeshCameraController.NudgeDirection.UP));
		}
	}
	public void OnPanDown() {
		if (storyController.storyMode == StoryController.StoryMode.PLAYING) {
			
		} else if (activeVertex.Down() != activeVertex) {
			ActivateVertex(activeVertex.Down ());
		} else if (storyController.storyMode == StoryController.StoryMode.DISABLED){
			StartCoroutine(meshSystemCameraController.Nudge(ViewPointMeshCameraController.NudgeDirection.DOWN));
		}
	}
	public void OnBeforeChangeVertex() {
		if (activeVertex != null && activeVertex.associatedInspectionPoints != null) {
			foreach (InspectionPointController ipc in activeVertex.associatedInspectionPoints) {
				ipc.gameObject.layer = 0; // Default
				foreach (Transform child in ipc.transform)
					child.gameObject.layer = 0;
			}
		}
	}
	public void OnAfterChangeVertex() {
		if (activeVertex != null) {
			if (useKinect) {
				kinectInspectionPointChooser.FindNewInspectionPoint(activeVertex.transform.position,activeVertex.transform.rotation);
				kinectInspectionPointChooser.ActivateChosenInspectionPointForKinect();
			} else if (activeVertex.associatedInspectionPoints != null) {
				foreach (InspectionPointController ipc in activeVertex.associatedInspectionPoints) {
					ipc.gameObject.layer = 16; // Inspection Point Markers Detail
					foreach (Transform child in ipc.transform)
						child.gameObject.layer = 16;
				}
			}
			foreach (LineRendererCulling lrc in MonoBehaviour.FindObjectsOfType<LineRendererCulling>()) {
				lrc.UpdateLineRenderers(activeVertex.transform);
			}
		}
	}
	public void OnZoomIn() {
		if (cameraZoom.GetZoom() <= activeVertex.exitByZoomFieldOfView) {
			Debug.Log (activeVertex.moreZoomedBuilder.name);
			ViewPointMeshVertex nextVertex = activeVertex.ClosestMatch (activeVertex.moreZoomedBuilder);
			ActivateVertex(nextVertex);
			cameraZoom.SetZoom(nextVertex.entryByZoomFieldOfView);
		}
	}
	public void OnZoomOut() {
		if (activeVertex.entryByZoomFieldOfView != 0f && cameraZoom.GetZoom() >= activeVertex.entryByZoomFieldOfView) {
			Debug.Log (activeVertex.lessZoomedBuilder.name);
			ViewPointMeshVertex nextVertex = activeVertex.ClosestMatch (activeVertex.lessZoomedBuilder);
			ActivateVertex(nextVertex);
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
		engagementController.MarkPanelOut(true, true);
		if (!infoController.hingeOut) {
			infoController.ToggleHinge();
		}
	}
	public void OnInspectorHingeOut() {
		engagementController.MarkPanelOut(false,true);
		if (!inspectorController.hingeOut) {
			inspectorController.ToggleHinge();
		}
		StartCoroutine(inspector.videoController.Unmute());
	}
	public void OnHingeAway(bool leftHandHinge) {
		if (leftHandHinge) {
			OnInfoHingeAway();
		} else {
			OnInspectorHingeAway();
		}
	}
	public void OnInfoHingeAway() {
		engagementController.MarkPanelOut(true, false);
		if (infoController.hingeOut) {
			infoController.ToggleHinge();
		}
	}
	public void OnInspectorHingeAway() {
		engagementController.MarkPanelOut(false,false);
		if (inspectorController.hingeOut) {
			inspectorController.ToggleHinge();
		}
		StartCoroutine(inspector.videoController.Mute());
	}
	public void OnNewFirstPlayer() {
	}
	public void OnNewSecondPlayer() {
	}

	
	/*****************************
	// PUBLIC GAME FUNCTIONS	*/
	public void ActivateVertex(ViewPointMeshVertex vert, bool openInfo = true) {
		if (!updating) {
			vertexTargets.Enqueue (vert);
			vertexTargets.Enqueue (openInfo);
		} else {
			ActivateVertexNow(vert,openInfo);
		}
	}
	private void ActivateVertexNow(ViewPointMeshVertex vert, bool openInfo) {
		OnBeforeChangeVertex();
		activeVertex = vert;
		meshSystemCameraController.GoToVertex(activeVertex);
		if (openInfo) {
			if (activeVertex.informationContent != InformationContent.NONE) {
				ActivateInformationContent(activeVertex.informationContent);
			}
		}
		ActivateAssociatedLabels();
		OnAfterChangeVertex();
	}
	public void ActivateAssociatedLabels() {
		while (activeLabels.Count > 0) {
			activeLabels[0].Deactivate();
			activeLabels.RemoveAt (0);
		}
		if (activeVertex.associatedLabels != null) {
			foreach (LabelController lc in activeVertex.associatedLabels) {
				lc.Activate();
				activeLabels.Add(lc);
			}
		}
	}
	public void ActivateInformationContent(InformationContent _info) {
		infoHolder.PlaceObjectInInfoUI(_info);
	}
	public void ActivateInformationContent(StoryContent _story) {
		infoHolder.PlaceObjectInInfoUI((InformationContent)(int)_story);
	}
	public void SetFirstPlayerId(long userId) {
		firstPlayerKinectInfo.SetUserId(userId);
	}
	public void SetSecondPlayerId(long userId) {
		secondPlayerKinectInfo.SetUserId(userId);
	}
	public void ClearFirstPlayer() {
		firstPlayerKinectInfo.SetUserId (-1);
	}
	public void ClearSecondPlayer() {
		secondPlayerKinectInfo.SetUserId (-1);
	}
	public void DecideWhetherKinectDebugShows() {
		kinectUserInfo.SetActive (useKinect);
	}
	public void SetCameraZoom(float target) {
		cameraZoom.SetZoom(target);
	}
	public void StartTour() {
		storyController.EngageStoryMode();
	}

	private void EnableDebugModes(bool enable) {
		KinectFeedbackController f = GameObject.FindObjectOfType<KinectFeedbackController>();
		debugModes = enable;
		if (enable) {
			f.AddItem(KinectFeedbackController.DEBUG_ON);
		} else {
			f.AddItem(KinectFeedbackController.DEBUG_OFF);
			useFace = false;
			useMouse = false;
			firstPlayerKinectInfo.UseNumpad(false);
			secondPlayerKinectInfo.UseNumpad(false);
		}
	}

}
