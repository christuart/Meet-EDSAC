using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WireBuilder : MonoBehaviour {

	public Vector3 start;
	public bool startAtTransform = true;
	public Vector3 end;
	public bool endAtTransform = true;
	[Range(0,.4f)]
	public float outwardDistance = 0.1f;
	[Range(0,2f)]
	public float downwardDistance = 0.2f;
	public bool overrideWithRandom = true;
	public bool setLooks = false;
	public Color colour;
	public float width;

	
	WireRenderer w;

	void Start() {
		GameObject g = new GameObject();
		g.transform.SetParent (transform);
		g.name = "(built wire)";
		w = g.AddComponent<WireRenderer>();
		if (overrideWithRandom) {
			outwardDistance = Random.Range(0.06f,0.12f);
			downwardDistance = Mathf.Pow(Random.Range (0.15f,.65f),2f);
		}
		w.SetWire(start,end,outwardDistance,downwardDistance);
		if (setLooks) w.SetLooks(colour,width);
		w.BuildWire();
		Destroy (this);
	}

	void OnDrawGizmos() {
		if (startAtTransform) start = transform.position;
		if (endAtTransform) end = transform.position;
		startAtTransform = false;
		endAtTransform = false;
	}

}
