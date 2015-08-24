using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StoryContent {
	ONE = InformationContent.STORY_ONE,
	NONE = InformationContent.NONE
};

public class StoryWaypoint : MonoBehaviour {
	
	public bool infoHingeAwayOnEnter;
	public bool infoHingeOutOnEnter;
	public bool inspectorHingeAwayOnEnter;
	public bool inspectorHingeOutOnEnter;
	public Vector3 cameraPosition;
	public Quaternion cameraRotation;
	public float cameraFieldOfView;
	public SortedList<float,Vector3> cameraAnimation;
	public float[] animationKeys;
	public Vector3[] animationValues;
	public bool loopAnimation;
	
	public StoryContent storyContent = StoryContent.NONE;
	public Videos videoContent = Videos.NONE;
	public Sprite imageContent;

	void Awake() {
		PopulateAnimation();
	}
	public void SetAnimation(float[] keys, Vector3[] values) {
		animationKeys = new float[keys.Length];
		animationValues = new Vector3[values.Length];
		keys.CopyTo (animationKeys,0);
		values.CopyTo (animationValues,0);
		PopulateAnimation();
	}

	public void PopulateAnimation() {
		cameraAnimation = new SortedList<float, Vector3>();
		for (int i=0; i < animationKeys.Length && i < animationValues.Length; i++)
			cameraAnimation.Add (animationKeys[i],animationValues[i]);
	}

}