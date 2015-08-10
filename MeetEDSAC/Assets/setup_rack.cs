using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class setup_rack : MonoBehaviour {

	// prefabs for instantiation
	public GameObject prefabChassis;

	public GameObject prefabTP1;
	public GameObject prefabTP2;
	public GameObject prefabTP3;
	public GameObject prefabTP4;
	
	public GameObject prefabValveBigFlat;
	public GameObject prefabValveBigShiny;

	public GameObject prefabValveSmallBlack;
	public GameObject prefabValveSmallCream;
	public GameObject prefabValveSmallRed;

	// probabilities of randomly skipping bits
	private float prob_chassis = 0.8f;
	private float prob_valve = 0.9f;

	// for placing the chassises
	private float chassis_start_y = -0.83f;
	private float chassis_offset_z = 0.003f;
	private float chassis_delta_y = 0.153f;

	// for placing the two rows of valves
	private float valve_start_x = 0.34f;
	private float valve_delta_x = -0.052f;
	private float valve_offset_y = -0.0019f;
	private float valve_back_row_z = 0.006f;
	private float valve_front_row_z = 0.055f;

	// for placing the testing points
	private float testing_pt_offset_y = -0.789f;
	private float testing_pt_offset_z = 0.0602f;
	
	// Use this for initialization	
	void Awake () {

		List<GameObject> testingPoints = new List<GameObject>{
			prefabTP1,
			prefabTP2,
			prefabTP3,
			prefabTP4};

		List<GameObject> bigValves = new List<GameObject>{
			prefabValveBigShiny,
			prefabValveBigFlat};

		List<GameObject> smallValves = new List<GameObject>{
			prefabValveSmallBlack,
			prefabValveSmallCream,
			prefabValveSmallRed};

		// make chassises
		for (int i = 0; i < 13; i++) {

			if (Random.value > prob_chassis) continue;

			GameObject chassis = Instantiate(prefabChassis) as GameObject;
			chassis.transform.parent = this.transform;
			chassis.transform.localScale = new Vector3 (1, 1, 1);
			chassis.transform.localPosition = new Vector3 (
				0,
			    chassis_start_y+i*chassis_delta_y,
			    chassis_offset_z);

			// for each chassis, make testing point
			int idx = (int) (Random.value * testingPoints.Count);
			GameObject tpChosen = testingPoints[idx];
			GameObject tp = Instantiate(tpChosen) as GameObject;

			tp.transform.parent = chassis.transform;
			tp.transform.localScale = new Vector3 (1, 1, 1);
			tp.transform.localPosition = new Vector3 (0f, testing_pt_offset_y, testing_pt_offset_z);

			// for each chassis, make big valves
			for (int j = 0; j < 14; j++) {

				idx = (int) (Random.value * bigValves.Count);
				GameObject valveChosen = bigValves[idx];
				GameObject valve = Instantiate(valveChosen) as GameObject;

				valve.transform.parent = chassis.transform;
				valve.transform.localScale = new Vector3 (1, 1, 1);
				valve.transform.rotation *= Quaternion.Euler(0, 180, 0);
				valve.transform.localPosition = new Vector3 (
					valve_start_x+valve_delta_x*j,
					valve_offset_y,
					(j==1 || j==12) ? valve_front_row_z : valve_back_row_z);
			}

			// for each chassis, make small valves
			for (int j = 0; j < 9; j++) {

				if (Random.value > prob_valve) continue;
				
				idx = (int) (Random.value * smallValves.Count);
				GameObject valveChosen = smallValves[idx];
				GameObject valve = Instantiate(valveChosen) as GameObject;

				valve.transform.parent = chassis.transform;
				valve.transform.localScale = new Vector3 (1, 1, 1);
				valve.transform.rotation *= Quaternion.Euler(0, 180, 0);
				valve.transform.localPosition = new Vector3 (
					valve_start_x+valve_delta_x*(j+2.5f),
					valve_offset_y,
					valve_front_row_z);
			}

		}
		Destroy(this);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
