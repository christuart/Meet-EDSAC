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
}
