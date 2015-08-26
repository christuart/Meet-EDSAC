using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EdsacXmlGenerator : MonoBehaviour {

	public EdsacXmlPopulater populater;

	public GameObject[] rearRacks;
	public GameObject[] middleRacks;
	public GameObject[] frontRacks;

	public Camera viewCamera;
	public GameObject prefabUILabel;
	public GameObject prefabUILabelTarget;
	public GameObject labelsHolder;
	private GameObject labelTargetsHolder;

	public float labelOneAlphaDistance = 1.3f;
	public float labelZeroAlphaDistance = 2f;
	public float labelMinAlpha = 0.05f;
	public bool labelConsiderNormal = true;
	public float labelNormalInfluence = .5f;
	public float labelPower = 6f;

	// prefabs for instantiation
	public GameObject prefabChassis;
	public GameObject prefabChassisLabel;
	
	public GameObject prefabTP1;
	public GameObject prefabTP2;
	public GameObject prefabTP3;
	public GameObject prefabTP4;
	
	public GameObject prefabValveBigFlat;
	public GameObject prefabValveBigShiny;
	
	public GameObject prefabValveSmallBlack;
	public GameObject prefabValveSmallCream;
	public GameObject prefabValveSmallRed;

	public GameObject prefabWithRectTransform;

	// for placing the chassises
	private float chassis_start_y = -0.983f;
	private float chassis_offset_z = 0.003f;
	private float chassis_delta_y = 0.153f;
	
	// for placing the two rows of valves
	private float valve_offset_y = -0.0019f;
	
	// for placing the testing points
	private float testing_pt_offset_y = -0.789f;
	private float testing_pt_offset_z = 0.0602f;

	// holds all the model structure info
	private EDSAC edsac;
	private Dictionary<EDSAC.TestingPointsType,GameObject> testingPoints;
	private Dictionary<EDSAC.ValveType,GameObject> valves;

	GameObject wireHolder;
	
	// Use this for initialization	
	void Awake () {
		
		testingPoints = new Dictionary<EDSAC.TestingPointsType,GameObject>{
			{ EDSAC.TestingPointsType.TYPE1, prefabTP1 },
			{ EDSAC.TestingPointsType.TYPE2, prefabTP2 },
			{ EDSAC.TestingPointsType.TYPE3, prefabTP3 },
			{ EDSAC.TestingPointsType.TYPE4, prefabTP4} };
		
		valves = new Dictionary<EDSAC.ValveType,GameObject>{
			{ EDSAC.ValveType.BIG_SHINY, prefabValveBigShiny},
			{ EDSAC.ValveType.BIG_FLAT, prefabValveBigFlat},
			{ EDSAC.ValveType.SMALL_BLACK, prefabValveSmallBlack},
			{ EDSAC.ValveType.SMALL_CREAM, prefabValveSmallCream},
			{ EDSAC.ValveType.SMALL_RED, prefabValveSmallRed} };

		edsac = populater.Populate();
		
		labelTargetsHolder = new GameObject();
		labelTargetsHolder.transform.SetParent(transform);
		labelTargetsHolder.transform.name = "Label Targets";

		if (edsac.rows.ContainsKey(EDSAC.RowName.REAR)) {
			FillRow(rearRacks, edsac.rows[EDSAC.RowName.REAR]);
		}
		if (edsac.rows.ContainsKey(EDSAC.RowName.MIDDLE)) {
			FillRow(middleRacks, edsac.rows[EDSAC.RowName.MIDDLE]);
		}
		if (edsac.rows.ContainsKey(EDSAC.RowName.FRONT)) {
			FillRow(frontRacks, edsac.rows[EDSAC.RowName.FRONT]);
		}
		FillWires();

		Destroy(this);
	}

	public void FillRow(GameObject[] physicalRow, EDSAC.Row infoRow) {
		
		EDSAC.Row ro = infoRow;
		GameObject[] theseRacks = physicalRow;
		foreach (EDSAC.Rack ra in ro.racks) {
			if (ra.rackNumber <= theseRacks.Length && ra.rackNumber > 0) {
				GameObject prefabRack = theseRacks[ra.rackNumber-1];
				GameObject rackLabelsHolder = Instantiate(prefabWithRectTransform) as GameObject;
				rackLabelsHolder.transform.SetParent(labelsHolder.transform,false);
				switch(ro.name) {
				case EDSAC.RowName.REAR:
					rackLabelsHolder.transform.name = "R" + ra.rackNumber;
					break;
				case EDSAC.RowName.MIDDLE:
					rackLabelsHolder.transform.name = "M" + ra.rackNumber;
					break;
				case EDSAC.RowName.FRONT:
					rackLabelsHolder.transform.name = "F" + ra.rackNumber;
					break;
				}
				foreach (EDSAC.Chassis ch in ra.chassis) {
					int i = ch.chassisNumber-1;
					if (i >= 0 && i < 14) {
						
						GameObject chassis = Instantiate(prefabChassis) as GameObject;
						chassis.transform.parent = prefabRack.transform;
						chassis.transform.localScale = new Vector3 (1f, 1f, 1f);
						chassis.transform.localPosition = new Vector3 (0f,
						                                               chassis_start_y+i*chassis_delta_y,
						                                               chassis_offset_z);
						GameObject chassisLabel = Instantiate(prefabChassisLabel) as GameObject;
						chassisLabel.transform.SetParent(chassis.transform,false);
						foreach (TextMesh t in chassisLabel.GetComponentsInChildren<TextMesh>()) {
							if (t.gameObject.name == "Chassis Name") {
								t.text = ch.physicalLabelNameText;
							} else if (t.gameObject.name == "Chassis Number") {
								t.text = ch.physicalLabelNumberText;
							}
						}

						GameObject uiLabel = Instantiate (prefabUILabel) as GameObject;
						uiLabel.transform.SetParent(rackLabelsHolder.transform);
						uiLabel.transform.localScale = new Vector3(.8f,.8f,1f);
						uiLabel.transform.name = ch.uiLabelText + " Label";
						GameObject uiLabelTarget = Instantiate(prefabUILabelTarget) as GameObject;
						uiLabelTarget.transform.SetParent(labelTargetsHolder.transform);
						uiLabelTarget.transform.name = ch.uiLabelText + " Label Target";

						LabelController labelController = uiLabel.GetComponent<LabelController>();
						labelController.viewCamera = viewCamera;
						labelController.targetForLabel = uiLabelTarget.GetComponent<Renderer>();
						
						labelController.oneAlphaDistance = labelOneAlphaDistance;
						labelController.zeroAlphaDistance = labelZeroAlphaDistance;
						labelController.minAlpha = labelMinAlpha;
						labelController.considerNormal = labelConsiderNormal;
						labelController.normalInfluence = labelNormalInfluence;
						labelController.power = labelPower;

						Text uiLabelText = uiLabel.GetComponentInChildren<Text>();
						uiLabelText.text = ch.uiLabelText;
						while (uiLabelText.preferredWidth > 200f && uiLabelText.fontSize > 10)
							uiLabelText.fontSize--;
						((RectTransform)uiLabelText.transform).sizeDelta = new Vector2(uiLabelText.preferredWidth, 30f);

						uiLabelTarget.GetComponent<LabelAlignmentOnChassis>().chassis = chassis.transform;

						if (testingPoints.ContainsKey(ch.chassisType.testingPoints)) {
							GameObject tpChosen = testingPoints[ch.chassisType.testingPoints];
							GameObject tp = Instantiate(tpChosen) as GameObject;
							
							tp.transform.parent = chassis.transform;
							tp.transform.localScale = new Vector3 (1f, 1f, 1f);
							tp.transform.localPosition = new Vector3 (0f, testing_pt_offset_y, testing_pt_offset_z);
						}
						foreach (EDSAC.Valve v in ch.chassisType.valves) {
							if (valves.ContainsKey(v.valveType)) {
								GameObject valveChosen = valves[v.valveType];
								GameObject valve = Instantiate(valveChosen) as GameObject;
								
								valve.transform.parent = chassis.transform;
								valve.transform.localScale = new Vector3 (1f, 1f, 1f);
								valve.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
								valve.transform.localPosition = new Vector3 (
									v.x,
									valve_offset_y,
									v.y);
							}
						}
					}
				}
			}
		}
	}

	public void FillWires() {

		wireHolder = new GameObject();
		wireHolder.transform.SetParent(transform,true);
		wireHolder.name = "(wires)";

		AddRowWires(rearRacks);
		AddRowWires(middleRacks);
		AddRowWires(frontRacks);
		AddInterRowWires(new GameObject[][] { rearRacks,middleRacks,frontRacks });

	}

	public void AddRowWires(GameObject[] racks) {

		GameObject w; // will be used to hold wires in many places

		// Add wires leading to overhead wire transfer
		foreach (GameObject rack in racks) {

			w = new GameObject();
			w.name = "(" + rack.transform.parent.name + " upwards wires)"; // "(R1 upwards wires)"
			w.transform.SetParent (wireHolder.transform,true);

			// make between 4 and 10 wires
			for (int i = Random.Range (0,7); i < 10; i++) {
				WireBuilder wb = w.AddComponent<WireBuilder>();
				wb.overrideWithRandom = false;
				wb.outwardDistance = 0.02f;
				wb.downwardDistance = 0.05f;
				wb.start = rack.transform.parent.position + new Vector3(-0.4f,2.4f,.22f);

				wb.end = wb.start + new Vector3(.2f * (Mathf.Pow (Random.Range (1f,2f),2f)-.8f),0f,-.15f); 
				int chassisNumber = Random.Range (0,rack.transform.childCount);
				wb.end.y = rack.transform.GetChild (chassisNumber).position.y + .12f;
			}
		}

		// Add wires between chassis
		w = new GameObject();
		w.name = "(chassis to chassis wires)";
		w.transform.SetParent (wireHolder.transform,true);
		for (int r = 0; r < racks.Length; r++) {
			GameObject rack = racks[r];
			foreach (Transform chassisTransform in rack.transform) {
				// make between 2 and 3 wires
				for (int i = 2-Mathf.FloorToInt (Mathf.Pow (Random.value,1.5f) * 2); i < 3; i++) {
					float distanceAllowedDecider = Random.value;
					GameObject targetRack;
					if (distanceAllowedDecider < .05) {
						// allow any rack
						targetRack = racks[Random.Range(0,racks.Length)];
					} else if (distanceAllowedDecider < 0.3) {
						// allow up to 2 racks away
						targetRack = racks[Random.Range(Mathf.Max (0,r-2),Mathf.Min (racks.Length,r+3))]; // remember, .Range(min,max) is always less than max, so add 3 not 2
					} else {
						// allow only 1 rack away

						targetRack = racks[Random.Range(Mathf.Max (0,r-1),Mathf.Min (racks.Length,r+2))]; // remember, .Range(min,max) is always less than max, so add 2 not 1
					}
					Transform targetChassis = targetRack.transform.GetChild(Random.Range (0,targetRack.transform.childCount));
					WireBuilder wb = w.AddComponent<WireBuilder>();
					wb.start = chassisTransform.position + new Vector3(Random.Range (-.38f,.38f),.075f,.082f);
					wb.end = targetChassis.position + new Vector3(Random.Range (-.38f,.38f),.075f,.082f);
					wb.overrideWithRandom = false;
					wb.outwardDistance = Random.Range (.18f,.19f);
					wb.downwardDistance = Random.Range (.1f,.3f);
				}
			}
		}

		// Add wires on the chassis
		w = new GameObject();
		w.name = "(back of chassis wires)";
		w.transform.SetParent (wireHolder.transform,true);
		foreach (GameObject rack in racks) {
			foreach (Transform chassisTransform in rack.transform) {
				for (int i = 6; i < 31; i+=2) {
					if (Random.value < 0.8f) {
						WireBuilder wb = w.AddComponent<WireBuilder>();
						wb.start = chassisTransform.position + new Vector3(Mathf.Lerp (-.38f,.38f,i/31f),.058f,.023f);
						wb.end = wb.start + new Vector3(0f,-.08f,0f);
						wb.overrideWithRandom = false;
						wb.outwardDistance = .015f;
						wb.downwardDistance = 0f;
						wb.setLooks = true;
						wb.colour = Color.black;
						wb.width = .0075f;
						wb = w.AddComponent<WireBuilder>();
						wb.start = chassisTransform.position + new Vector3(Mathf.Lerp (-.38f,.38f,(i+1)/31f),.058f,.023f);
						wb.end = wb.start + new Vector3(0f,-.07f,0f);
						wb.overrideWithRandom = false;
						wb.outwardDistance = .015f;
						wb.downwardDistance = 0f;
						wb.setLooks = true;
						wb.colour = new Color(.25f,.08f,.08f);
						wb.width = .0075f;
					}
				}
			}
		}

		// Add wires between racks
		w = new GameObject();
		w.name = "(back of chassis wires)";
		w.transform.SetParent (wireHolder.transform,true);
		for (int i = 0; i < racks.Length - 1; i ++) {
			
			// make between 3 and 6 wires
			for (int j = Random.Range (0,4); j < 6; j++) {
				WireBuilder wb = w.AddComponent<WireBuilder>();
				wb.overrideWithRandom = false;
				wb.outwardDistance = 0f;
				wb.downwardDistance = Random.Range (0.04f,0.1f);
				wb.start = racks[i].transform.parent.position + new Vector3(-0.4f,2.4f,.22f);
				wb.end = racks[i+1].transform.parent.position + new Vector3(-0.4f,2.4f,.22f);
			}
		}
	}

	public void AddInterRowWires(GameObject[][] rows) {
		// Add wires between racks
		GameObject w = new GameObject();
		w.name = "(overhead wires)";
		w.transform.SetParent (wireHolder.transform,true);
		for (int i = 0; i < rows.Length - 1; i ++) {
			
			// make 8 wires
			for (int j = 0; j < 8; j++) {
				WireBuilder wb = w.AddComponent<WireBuilder>();
				wb.overrideWithRandom = false;
				wb.outwardDistance = Random.Range (-0.01f,0.01f);
				wb.downwardDistance = Random.Range (0.04f,0.3f);
				wb.start = rows[i][0].transform.parent.position + new Vector3(0f,2.4f,.22f);
				wb.end = rows[i+1][0].transform.parent.position + new Vector3(0f,2.4f,.22f);
				wb.start.x = -.4f;
				wb.end.x = -.4f;
			}
		}
		// make 8 wires
		for (int j = 0; j < 8; j++) {
			WireBuilder wb = w.AddComponent<WireBuilder>();
			wb.overrideWithRandom = false;
			wb.outwardDistance = Random.Range (-0.01f,0.01f);
			wb.downwardDistance = Random.Range (0.04f,0.3f);
			wb.start = rows[0][0].transform.parent.position + new Vector3(0f,2.4f,.22f);
			wb.end = rows[rows.Length-1][0].transform.parent.position + new Vector3(0f,2.4f,.22f);
			wb.start.x = -2.4f;
			wb.end.x = -2.4f;
		}
	}
}
