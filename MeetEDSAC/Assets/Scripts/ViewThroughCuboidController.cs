using UnityEngine;
using System.Collections;

public class ViewThroughCuboidController : MonoBehaviour {
	
	public int divX = 1;
	public int divY = 1;
	public int divZ = 1000;

	// Use this for initialization
	void Start () {
		Material m = GetComponent<MeshRenderer>().material;
		GameObject g;
		for (float i=0.5f-divX/2f; i+.1f<divX/2f; i++) {
			for (float j=+0.5f-divY/2f; j+.1f<divY/2f; j++) {
				for (float k=+0.5f-divZ/2f; k+.1f<divZ/2f; k++) {
					g = GameObject.CreatePrimitive(PrimitiveType.Cube);
					g.GetComponent<MeshRenderer>().material = m;
					g.transform.localScale = new Vector3(
						transform.localScale.x / divX,
						transform.localScale.y / divY,
						transform.localScale.z / divZ);
					g.transform.position = transform.position - new Vector3(
						i * transform.localScale.x / divX,
						j * transform.localScale.y / divY,
						k * transform.localScale.z / divZ);
					g.transform.parent = transform;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
