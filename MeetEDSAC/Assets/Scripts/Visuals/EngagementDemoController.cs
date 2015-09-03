using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EngagementDemoController : MonoBehaviour {

	[Range(-1,1)]
	public float engagementInput = 0f;
	public EyesController eyes;
	public Image forLookingLeft;
	public Image forLookingRight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		eyes.position = engagementInput;
		forLookingLeft.color = new Color(1f,1f,1f,-engagementInput);
		forLookingRight.color = new Color(1f,1f,1f,engagementInput);
	}
}
