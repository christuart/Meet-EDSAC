using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StoryController : MonoBehaviour {

	public enum StoryMode { DISABLED, STOPPED, PLAYING, PAUSED };

	public StoryMode storyMode;
	public GameObject storyModeIndicator;
	private ViewPointMeshVertex normalModeVertex;
	private float normalModeFOV;
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
	private Quaternion animationRotation;
	private int lastAnimationKeyframe;

	void Awake () {
		storyVertexHolder = new GameObject();
		storyVertex = storyVertexHolder.AddComponent<StoryViewpointSystemVertex>();
		animationPosition = new Vector3();
		animationRotation = new Quaternion();
		storyModeIndicator.SetActive (false);
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
					if (activeWaypoint.cameraRotationAnimation.Values[lastAnimationKeyframe+1] != activeWaypoint.cameraRotationAnimation.Values[lastAnimationKeyframe]) {
						animationRotation = Quaternion.Slerp(activeWaypoint.cameraRotationAnimation.Values[lastAnimationKeyframe+1],activeWaypoint.cameraRotationAnimation.Values[lastAnimationKeyframe],remainingTime/totalTime);
					} else {
						animationRotation = activeWaypoint.cameraRotationAnimation.Values[lastAnimationKeyframe];
					}
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
		normalModeFOV = controller.cameraZoom.GetZoom();
		foreach(InspectionPointController ipc in GameObject.FindObjectsOfType<InspectionPointController>())
			ipc.Hide();
		storyModeIndicator.SetActive (true);
		storyMode = StoryMode.STOPPED;
		activeWaypointIndex = -1;
		EnterNextWaypoint();
	}
	public void LeaveStoryMode() {
		controller.ActivateVertex (normalModeVertex);
		controller.cameraZoom.SetZoom(normalModeFOV);
		if (!controller.useKinect) {
			foreach(InspectionPointController ipc in GameObject.FindObjectsOfType<InspectionPointController>())
				ipc.Unhide();
		}
		storyModeIndicator.SetActive (false);
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
			animationRotation = Quaternion.identity;
			cameraController.continuousTarget = false;
		}
		if (activeWaypoint.videoContent != Videos.NONE) {
			controller.inspector.SetVideo (activeWaypoint.videoContent);
		} else if (activeWaypoint.imageContent != null) {
			controller.inspector.SetImage(activeWaypoint.imageContent);
		}
		if (activeWaypoint.inspectorCaption != "") {
			controller.inspector.SetCaption (activeWaypoint.inspectorCaption);
		}
		if (activeWaypoint.audioContent != null) {
			StartCoroutine(PlayAudioContent(activeWaypoint.audioContent,activeWaypoint.audioDelay));
		} else {
			controller.audioController.contentAudioSource.Stop ();
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
		storyVertex.transform.rotation = animationRotation * activeWaypoint.cameraRotation;
		controller.SetCameraZoom(activeWaypoint.cameraFieldOfView);
		storyVertex.associatedLabels = new LabelController[activeWaypoint.associatedLabels.Length];
		activeWaypoint.associatedLabels.CopyTo (storyVertex.associatedLabels,0);
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
	}
	public bool IsAtFirstWaypoint() {
		return activeWaypointIndex == 0;
	}
	private IEnumerator PlayAudioContent(AudioClip content, float delay) {
		if (delay != 0f) {
			yield return new WaitForSeconds(delay);
		}
		if (controller.storyController.activeWaypoint.audioContent == content) {
			controller.audioController.contentAudioSource.Stop();
			controller.audioController.contentAudioSource.clip = content;
			controller.audioController.contentAudioSource.Play();
		}
		yield break;
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
					animationRotations = new Vector3[w.cameraRotationAnimation.Count];
					for (int i = 0; i < w.cameraRotationAnimation.Count; i++)
						animationRotations[i] = w.cameraRotationAnimation.Values[i].eulerAngles;
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
	public Vector3[] animationRotations;
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
					w.SetAnimation(animationKeys,animationValues,animationRotations);
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
				if (!_visualiseWaypoint || !_editMode) {
					visualiseNewWaypoint = false;
					if (visCameraHolder.GetComponent<Camera>() != null) DestroyImmediate (visCameraHolder.GetComponent<Camera>());
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
					animationRotations = new Vector3[0];
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
					waypoints[workingWaypoint].SetAnimation(animationKeys,animationValues,animationRotations);
					waypoints[workingWaypoint].PopulateAnimation();
					waypoints[workingWaypoint].loopAnimation = loopAnimation;
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
