using UnityEngine;
using System.Collections;

public class WireRenderer : MonoBehaviour {
	
	LineRenderer lr;
	
	public int divisions = 40;
	public float divisionsFactor = 0.06f;
	
	public Vector3 start;
	public Vector3 across;
	Vector3 down;
	Vector3 back;
	static float acosh2 = 1.3169578969248167086f;

	public bool setLooks = false;
	public Color colour;
	public float width;
	public Quaternion outwardDirection = Quaternion.identity;

	void Awake() { 
		lr = gameObject.GetComponent<LineRenderer>();
		if (lr == null)	lr = gameObject.AddComponent<LineRenderer>();
		lr.material = new Material(Shader.Find ("Unlit/Color"));
		if (setLooks) {
			SetLooks (colour, width);
		} else {
			float r = Random.Range (.1f,.26f);
			float g = Random.Range (.1f,.22f);
			Color lineColour = new Color(r,g,0.05f);
			SetLooks(lineColour, 0.01f);
		}
		lr.useWorldSpace = false;
	}

	public void SetWire(Vector3 _start, Vector3 _end, float _outwards = 0.1f, float _downwards = .2f) {
		start = _start;
		across = _end - _start;
		back = outwardDirection * (Vector3.forward * _outwards);
		down = outwardDirection * (Vector3.down * _downwards);
	}

	public void SetLooks(Color _colour, float _width) {
		colour = _colour;
		width = _width;
		Color rendererColour = new Color(_colour.r,_colour.g,_colour.b);
		//lr.material.SetColor ("_EmisColor",rendererColour);
		lr.material.SetColor ("_Color",rendererColour);
		lr.SetColors(_colour,_colour);
		lr.SetWidth (_width,_width);
	}
	
	public void BuildWire() {
		
		divisions = 1 + Mathf.FloorToInt (Mathf.Pow (Mathf.Pow (across.magnitude,2)+2*Mathf.Pow (down.magnitude,2)+2*Mathf.Pow (back.magnitude,2),.25f)/ divisionsFactor);
		
		lr.SetVertexCount(divisions+3);

		for (int i=0; i <= divisions; i++) {
			float progress = (1f - Mathf.Cos(Mathf.PI * (float)i/divisions))/2f;
			float coshProgress = 2f - Tools.Cosh((2f*progress-1f)*acosh2);
			Vector3 xyz = start + progress * across + Mathf.Pow(coshProgress,0.25f) * back + coshProgress * down;
			lr.SetPosition(i+1,xyz);
			if (i == 1) {
				lr.SetPosition(0,xyz);
			} else if (i==divisions-1) {
				lr.SetPosition (divisions+2,xyz);
			}
		}
	}
}
