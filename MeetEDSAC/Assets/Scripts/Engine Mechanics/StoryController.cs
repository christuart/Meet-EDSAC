using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class StoryController : MonoBehaviour {

	public enum StoryMode { DISABLED, STOPPED, PLAYING, PAUSED };

	public StoryMode storyMode;
	private ViewPointMeshVertex normalModeVertex;
	public int activeWaypointIndex;
	public StoryWaypoint activeWaypoint;
	public Controller controller;
	public ViewPointMeshCameraController cameraController;

	public Vector3 targetLocation;
	public Quaternion targetRotation;
	public Vector3 velocity;

	public InspectionPointController inspectionContent;
	public bool inspectionContentActivated;

	public StoryViewpointSystemVertex storyVertex;
	private GameObject storyVertexHolder;
	
	public StoryWaypoint[] waypoints;
	public GameObject waypointHolder;

	private float lastAnimationLoopStart = 0f;
	private float nextAnimationKeyframeStart;
	private Vector3 animationPosition;
	private int lastAnimationKeyframe;

	void Awake () {
		storyVertexHolder = new GameObject();
		storyVertex = storyVertexHolder.AddComponent<StoryViewpointSystemVertex>();
		animationPosition = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {
		if (storyMode == StoryMode.PLAYING) {
			if (activeWaypoint.cameraAnimation != null) {
				// first deal with getting the right keyframe
				if (nextAnimationKeyframeStart != -1f && lastAnimationKeyframe < activeWaypoint.cameraAnimation.Count() - 1) {
					if (Time.time > nextAnimationKeyframeStart)
						nextAnimationKeyframeStart = UpdateAnimation(lastAnimationKeyframe+1,nextAnimationKeyframeStart);
					float remainingTime = nextAnimationKeyframeStart - Time.time;
					float totalTime = activeWaypoint.cameraAnimation.Keys[lastAnimationKeyframe+1] - activeWaypoint.cameraAnimation.Keys[lastAnimationKeyframe];
					animationPosition = Vector3.Lerp (activeWaypoint.cameraAnimation.Values[lastAnimationKeyframe+1],activeWaypoint.cameraAnimation.Values[lastAnimationKeyframe],remainingTime/totalTime);
				}
				// then get the interpolated animation position
											// lerp from the next to the previous keyframes
											// by the amount that hasn't happened yet
				// then apply this to the vertex
				UpdateStoryVertex();
			}
		}
	}

	public void EngageStoryMode() {
		normalModeVertex = controller.activeVertex;
		foreach(InspectionPointController ipc in GameObject.FindObjectsOfType<InspectionPointController>())
			ipc.Hide();
		storyMode = StoryMode.STOPPED;
		activeWaypointIndex = -1;
		EnterNextWaypoint();
	}
	public void LeaveStoryMode() {
		controller.ActivateVertex (normalModeVertex);
		foreach(InspectionPointController ipc in GameObject.FindObjectsOfType<InspectionPointController>())
			ipc.Unhide();
		storyMode = StoryMode.DISABLED;
	}
		
	public void EnterNextWaypoint() {
		if (!IsAtLastWaypoint()) {
			ChangeWaypoint(1);
		} else {
			LeaveStoryMode();
		}
	}
	public void EnterPreviousWaypoint() {
		if (!IsAtFirstWaypoint()) {
			ChangeWaypoint(-1);
		} else {
			LeaveStoryMode();
		}
	}
	public void ChangeWaypoint(int _change) {
		storyMode = StoryMode.PLAYING;
		activeWaypointIndex+=_change;
		activeWaypoint = waypoints[activeWaypointIndex];
		if (activeWaypoint.cameraAnimation != null && activeWaypoint.cameraAnimation.Count > 0) {
			nextAnimationKeyframeStart = UpdateAnimation(0,Time.time);
			cameraController.continuousTarget = true;
		} else {
			animationPosition = new Vector3();
			cameraController.continuousTarget = false;
		}
		if (activeWaypoint.videoContent != Videos.NONE) {
			controller.inspector.SetVideo (activeWaypoint.videoContent);
		} else if (activeWaypoint.imageContent != null) {
			controller.inspector.SetImage(activeWaypoint.imageContent);
		}
		if (activeWaypoint.infoHingeAwayOnEnter) {
			controller.OnInfoHingeAway();
		} else if (activeWaypoint.infoHingeOutOnEnter) {
			controller.OnInfoHingeOut();
		}
		if (activeWaypoint.inspectorHingeAwayOnEnter) {
			controller.OnInspectorHingeAway();
		} else if (activeWaypoint.inspectorHingeOutOnEnter) {
			controller.OnInspectorHingeOut();
		}
		lastAnimationLoopStart = Time.time;
		UpdateStoryVertex();
		controller.ActivateVertex(storyVertex);
	}
	public void UpdateStoryVertex() {
		storyVertex.transform.position = activeWaypoint.cameraPosition + animationPosition;
		storyVertex.transform.rotation = activeWaypoint.cameraRotation;
		controller.SetCameraZoom(activeWaypoint.cameraFieldOfView);
		if (activeWaypoint.storyContent != StoryContent.NONE)
			storyVertex.informationContent = (InformationContent)(int)activeWaypoint.storyContent;
	}
	public float UpdateAnimation(int _keyframe, float _time) {
		// _time is the time at which we 'moved' to the new keyframe, so that even if the call to UpdateAnimation is delayed, the animation isn't slowed
		lastAnimationKeyframe = _keyframe;
		if (lastAnimationKeyframe < activeWaypoint.cameraAnimation.Count - 1) {
			// if we are not at the final keyframe, set the next keyframe's time
			return _time + activeWaypoint.cameraAnimation.Keys[lastAnimationKeyframe+1] - activeWaypoint.cameraAnimation.Keys[lastAnimationKeyframe];
		} else {
			// however if we are, maybe we loop?
			if (activeWaypoint.loopAnimation) {
				// in which case, the final keyframe is the same as the first keyframe, so we want to go straight to the first keyframe now
				lastAnimationLoopStart = _time;
				return UpdateAnimation(0, _time);
			} else {
				// otherwise, no more animation
				return -1f;
			}
		}
		
	}
	
	public bool IsAtLastWaypoint() {
		return activeWaypointIndex == waypoints.Length-1;
	}public bool IsAtFirstWaypoint() {
		return activeWaypointIndex == 0;
	}
	
	/*********** BEGIN EDITOR VALUES ***********/
	public bool editMode = false;
	private bool _editMode = false;
	public GameObject waypointMarker;
	
	public int waypointsCount;
	private int workingWaypoint = 0;
	public int WorkingWaypoint {
		get {
			return workingWaypoint;
		}
		set {
			if (waypoints.Length == 0) {
				workingWaypoint = 0;
			} else {
				workingWaypoint = Mathf.Min (Mathf.Max (value,0),waypoints.Length-1);
				if (workingWaypoint != value) Debug.LogWarning("Tried to access bad waypoint.");
				StoryWaypoint w = waypoints[workingWaypoint];
				waypointIsAnimated = w.cameraAnimation != null;
				if (waypointIsAnimated) {
					animationKeys = w.cameraAnimation.Keys.ToArray ();
					animationValues = w.cameraAnimation.Values.ToArray ();
				}
				loopAnimation = w.loopAnimation;
				if (waypointMarker != null) {
					waypointMarker.transform.position = w.cameraPosition;
					waypointMarker.transform.rotation = w.cameraRotation;
				}
				visualisationFieldOfView = w.cameraFieldOfView;
				_visualisationFieldOfView = visualisationFieldOfView;
			}
		}
	}
	
	public int lookAtWaypoint;
	private int _lookAtWaypoint;
	public bool visualiseWaypoint;
	private bool _visualiseWaypoint;
	public bool visualiseNewWaypoint;
	private bool _visualiseNewWaypoint;
	[Range(10,85)]
	public float visualisationFieldOfView = 60f;
	private float _visualisationFieldOfView;
	public bool waypointIsAnimated;
	public float[] animationKeys;
	public Vector3[] animationValues;
	public bool loopAnimation;
	public bool updateWaypointToVisualisation;
	public bool revertVisualisationToWaypoint;
	
	private GameObject visCameraHolder;
	private Camera visCamera;
	
	public bool insertWaypointsAtCurrentLocation = true;
	private bool _insertWaypointsAtCurrentLocation = true;
	public bool addNewWaypoint = false;
	/*********** END EDITOR VALUES ***********/

	/*********** BEGIN EDITOR METHODS ***********/
	void OnDrawGizmos() {
		if (editMode) {
			if (!_editMode) {
				DestroyImmediate(visCamera);
				visualiseWaypoint = _visualiseWaypoint;
				visualiseNewWaypoint = _visualiseNewWaypoint;
			}
			if (visCameraHolder == null) {
				visCameraHolder = new GameObject();
				visCameraHolder.name = "(story controller temp object)";
				visCameraHolder.transform.parent = transform;
			}
			if (addNewWaypoint) {
				if (visualiseNewWaypoint && waypointMarker != null) {
					StoryWaypoint w = waypointHolder.AddComponent<StoryWaypoint>();
					w.cameraPosition = waypointMarker.transform.position;
					w.cameraRotation = waypointMarker.transform.rotation;
					w.cameraFieldOfView = visualisationFieldOfView;
					w.SetAnimation(animationKeys,animationValues);
					w.PopulateAnimation();
					w.loopAnimation = loopAnimation;
					List<StoryWaypoint> waypointsList = waypoints.ToList();
					if (insertWaypointsAtCurrentLocation) {
						waypointsList.Insert (workingWaypoint,w);
						waypoints = waypointsList.ToArray();
					} else {
						waypointsList.Add (w);
						waypoints = waypointsList.ToArray();
						lookAtWaypoint = waypoints.Length+1;
					}
				}
				addNewWaypoint = false;
			}
			waypointsCount = waypoints.Length;
			if (lookAtWaypoint != workingWaypoint + 1) {
				WorkingWaypoint = lookAtWaypoint - 1; // this will clamp to valid values
				lookAtWaypoint = workingWaypoint + 1; // so this corrects the value if we inputted a nasty
			}
			if (visualiseWaypoint && waypoints.Length == 0) visualiseWaypoint = false;
			if (visualiseWaypoint) {
				if (!_visualiseWaypoint) {
					visualiseNewWaypoint = false;
					visCamera = visCameraHolder.AddComponent<Camera>() as Camera;
					visCamera.depth = 30;
					visCamera.clearFlags = CameraClearFlags.SolidColor;
					visCamera.backgroundColor = Color.grey;
				}
				if (waypointMarker != null) {
					visCamera.transform.position = waypointMarker.transform.position;
					visCamera.transform.rotation = waypointMarker.transform.rotation;
					visCamera.fieldOfView = visualisationFieldOfView;
				}
			}
			if (!visualiseWaypoint && _visualiseWaypoint) {
				DestroyImmediate(visCamera);
			}
			if (visualiseNewWaypoint && !_visualiseNewWaypoint) {
				if (visualiseWaypoint) {
					visualiseNewWaypoint = false;
				} else {
					visCamera = visCameraHolder.AddComponent<Camera>() as Camera;
					visCamera.depth = 30;
					visCamera.clearFlags = CameraClearFlags.SolidColor;
					visCamera.backgroundColor = Color.grey;
					waypointIsAnimated = false;
					animationKeys = new float[0];
					animationValues = new Vector3[0];
					loopAnimation = true;
				}
			}
			if (visualiseNewWaypoint) {
				if (visCamera == null) {
					visCamera = visCameraHolder.AddComponent<Camera>() as Camera;
					visCamera.depth = 30;
					visCamera.clearFlags = CameraClearFlags.SolidColor;
					visCamera.backgroundColor = Color.grey;
				}
				visCamera.transform.position = waypointMarker.transform.position;
				visCamera.transform.rotation = waypointMarker.transform.rotation;
				visCamera.fieldOfView = visualisationFieldOfView;
			}
			if (!visualiseNewWaypoint && _visualiseNewWaypoint) {
				DestroyImmediate(visCamera);
			}
			if (visualiseWaypoint && waypointsCount > 0) {
				if (updateWaypointToVisualisation) {
					waypoints[workingWaypoint].cameraPosition = waypointMarker.transform.position;
					waypoints[workingWaypoint].cameraRotation = waypointMarker.transform.rotation;
					waypoints[workingWaypoint].cameraFieldOfView = visualisationFieldOfView;
					updateWaypointToVisualisation = false;
				} else if (revertVisualisationToWaypoint) {
					waypointMarker.transform.position = waypoints[workingWaypoint].cameraPosition;
					waypointMarker.transform.rotation = waypoints[workingWaypoint].cameraRotation;
					visualisationFieldOfView = waypoints[workingWaypoint].cameraFieldOfView;
					revertVisualisationToWaypoint = false;
				}
			}

			// now store their values in case edit mode is turned off
			_visualiseWaypoint = visualiseWaypoint;
			_visualiseNewWaypoint = visualiseNewWaypoint;
			_lookAtWaypoint = lookAtWaypoint;
			_insertWaypointsAtCurrentLocation = insertWaypointsAtCurrentLocation;
			_visualisationFieldOfView = visualisationFieldOfView;
		} else {
			if (visCameraHolder != null) DestroyImmediate(visCameraHolder);
			// if no edit mode, don't let values change
			visualiseWaypoint = false;
			visualiseNewWaypoint = false;
			lookAtWaypoint = _lookAtWaypoint;
			insertWaypointsAtCurrentLocation = _insertWaypointsAtCurrentLocation;
			visualisationFieldOfView = _visualisationFieldOfView;
		}
		_editMode = editMode;
	}
	/*********** END EDITOR METHODS ***********/
}
