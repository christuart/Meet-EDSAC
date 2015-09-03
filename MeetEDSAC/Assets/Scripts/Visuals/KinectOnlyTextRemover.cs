using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KinectOnlyTextRemover : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (!GameObject.FindGameObjectWithTag ("GameController").GetComponent<Controller>().useKinect)
			GetComponent<Text>().text = "Take a guided tour of the EDSAC";
		Destroy (this);
	}
}
