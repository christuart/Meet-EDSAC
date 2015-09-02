using UnityEngine;
using System.Collections;

public class EyesController : MonoBehaviour {

	[Range(-1,1)]
	public float position = 0f;
	private float areaHalfWidth = 160f;

	public Transform leftPupil;
	public Transform rightPupil;
		
	// Update is called once per frame
	void Update () {
		transform.localPosition = new Vector2(position * areaHalfWidth,0f);
		leftPupil.localPosition = new Vector2(position * 3.5f,0f);
		rightPupil.localPosition = new Vector2(position * 3.5f,0f);
	}
}
