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
 * 
 * Connect meshes together using the zoom model
 * 
 */

using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	public GestureInfoInterpreter firstPlayerGestures;

	public ViewPointMeshCameraController meshSystemCameraController;
	
	public InfoHolderController infoHolder;
	public InspectorController inspector;

	private ViewPointMeshVertex activeVertex;
	private Queue vertexTargets;

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
			foreach (LabelController lc in GameObject.FindObjectsOfType<LabelController>())
				lc.Deactivate();
			foreach (LabelController lc in activeVertex.associatedLabels)
				lc.Activate();
			
			OnAfterChangeVertex();

		}

		if (Input.GetKeyDown(KeyCode.D) || firstPlayerGestures.GetGestureStarted(FakeGestures.LEFT_SWIPE)) {
			OnPanRight();
		}
		if (Input.GetKeyDown(KeyCode.A) || firstPlayerGestures.GetGestureStarted(FakeGestures.RIGHT_SWIPE)) {
			OnPanLeft();
		}
		if (Input.GetKeyDown(KeyCode.W) || firstPlayerGestures.GetGestureStarted(FakeGestures.DOWN_SWIPE)) {
			OnPanUp();
		}
		if (Input.GetKeyDown(KeyCode.S) || firstPlayerGestures.GetGestureStarted(FakeGestures.UP_SWIPE)) {
			OnPanDown();
		}
	}
	
	/*****************************
	// PUBLIC GAME EVENTS		*/
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

	
	/*****************************
	// PUBLIC GAME FUNCTIONS	*/
	public void ActivateVertex(ViewPointMeshVertex vert, bool openInfo = true) {
		vertexTargets.Enqueue(vert);
		vertexTargets.Enqueue(openInfo);
	}

}
