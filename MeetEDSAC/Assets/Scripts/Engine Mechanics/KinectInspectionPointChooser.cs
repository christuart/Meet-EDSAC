using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KinectInspectionPointChooser : MonoBehaviour {
	
	public InspectionPointController chosenInspectionPoint;
	public InspectionPointController inspectionPointToChoose;
	public GameObject inspectorLabel;
	public Text inspectorLabelText;

	public void EnableKinectInspectionPointLabel(bool _enable) {
		inspectorLabel.SetActive(_enable);
	}

	public InspectionPointController FindNewInspectionPoint(Vector3 cameraLocation, Quaternion cameraRotation) {
		InspectionPointController[] inspectionPoints = GameObject.FindObjectsOfType<InspectionPointController>();
		InspectionPointController bestBet;
		float bestDistance = 1000f;
		if (inspectionPoints.Length > 0) {
			bestBet = inspectionPoints[0];
			foreach (InspectionPointController i in inspectionPoints) {
				// if the inspection point is at least vaguely aligned with the camera view
				if (Vector3.Dot(i.transform.position - cameraLocation,cameraRotation * Vector3.forward) > 0.1) {
					float s = (i.transform.position - cameraLocation).magnitude;
					if (s < bestDistance) {
						bestBet = i;
						bestDistance = s;
					}
				}
			}
			if (bestDistance < 1000f) {
				inspectionPointToChoose = bestBet;
			}
		}
		return inspectionPointToChoose;
	}

	public void ActivateChosenInspectionPointForKinect() {
		//Debug.Log ("Step 1: Check for target inspection point existing");
		if (inspectionPointToChoose != null) {
			//Debug.Log ("Step 2: Check for target inspection point having changed");
			if (inspectionPointToChoose != chosenInspectionPoint) {
				chosenInspectionPoint = inspectionPointToChoose;
				//Debug.Log ("Step 3: Try to activate it");
				chosenInspectionPoint.Choose (false);
				EnableKinectInspectionPointLabel(true);
				Text inspectionPointText = chosenInspectionPoint.gameObject.GetComponentInChildren<Text>();
				if (inspectionPointText != null)
					inspectorLabelText.text = inspectionPointText.text.Replace("\n"," ");
			}
		}
	}

}
