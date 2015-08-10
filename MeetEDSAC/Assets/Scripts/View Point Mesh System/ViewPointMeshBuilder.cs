using UnityEngine;
using System.Collections;

public class ViewPointMeshBuilder : MonoBehaviour {

	public bool visualise = true;
	public Controller gameController;

	public GameObject viewPointMeshVertex;
	public int levels = 3;
	public int columns = 5;
	public float angleTheta = 360f; // angle in floor plane
	public float anglePhiMax = 37f; // angle above horizontal at top level
	public float anglePhiMin = 10f; // angle above horizontal at bottom level
	public bool isCloseToObjects; // focal length will be set different
	public bool placeColumnAtBothEnds = false; // i.e. |...|...|...| or |..|..|..|...
	public bool loopAround = true;
	public bool thetaCentredAtOrigin = false;

	public bool enterDefaultVertexOnStart = true;
	public int[] defaultVertex = new int[] {2,3}; // starting at 1,1 and finishing at levels,columns

	public ViewPointMeshCameraController[] targets;
	
	public ViewPointMeshBuilder connectAnotherBuilderOnTheLeft;
	public ViewPointMeshBuilder connectAnotherBuilderOnTheRight;

	public InformationContent informationContent;

	public LabelController[] associatedLabels;
	public GameObject associatedLabelParent;

	private bool meshBuilt = false;
	
	private Vector3[,] camPositions;
	private Quaternion[,] camRotations;
	private GameObject[,] builtMesh;
	private ViewPointMeshVertex[,] builtMeshVertices;

	// Use this for initialization
	void Start () {
		
		camPositions = new Vector3[levels,columns];
		camRotations = new Quaternion[levels,columns];
		builtMesh = new GameObject[levels,columns];
		builtMeshVertices = new ViewPointMeshVertex[levels,columns];
		
		float phi0 = anglePhiMin;
		float dPhi = (anglePhiMax-anglePhiMin)/(levels-1);
		float theta0 = transform.localRotation.eulerAngles.y + (thetaCentredAtOrigin ? -angleTheta/2f : 0f);
		float dTheta = angleTheta / (placeColumnAtBothEnds ? columns-1 : columns);
		
		for (int i = 0; i < levels; i++) {
			for (int j = 0; j < columns; j ++) {
				camPositions[i,j] = new Vector3(
					Mathf.Cos (Mathf.Deg2Rad * (theta0 + j*dTheta)) * Mathf.Cos (Mathf.Deg2Rad * (phi0 + i*dPhi)) * transform.localScale.x,
					Mathf.Sin (Mathf.Deg2Rad * (phi0 + i*dPhi)) * transform.localScale.y,
					Mathf.Sin (Mathf.Deg2Rad * (theta0 + j*dTheta)) * Mathf.Cos (Mathf.Deg2Rad * (phi0 + i*dPhi)) * transform.localScale.z);
				camRotations[i,j].eulerAngles = -transform.eulerAngles + (new Vector3(phi0 + i*dPhi,-90f + ((thetaCentredAtOrigin ? angleTheta/2f : 0f)) - j*dTheta,0));
				builtMesh[i,j] = GameObject.Instantiate(viewPointMeshVertex,transform.position+camPositions[i,j],camRotations[i,j]) as GameObject;
				builtMesh[i,j].transform.parent = transform;
			}
		}
		for (int i = 0; i < levels; i++) {
			for (int j = 0; j < columns; j ++) {
				builtMeshVertices[i,j] = builtMesh[i,j].GetComponent<ViewPointMeshVertex>();
				if (i > 0) {
					builtMeshVertices[i,j].down = builtMeshVertices[i-1,j];
					builtMeshVertices[i-1,j].up = builtMeshVertices[i,j];
				}
				if (j > 0) {
					builtMeshVertices[i,j].left = builtMeshVertices[i,j-1];
					builtMeshVertices[i,j-1].right = builtMeshVertices[i,j];
				}
				if (associatedLabels != null) builtMeshVertices[i,j].associatedLabels = associatedLabels;
				if (associatedLabelParent != null) builtMeshVertices[i,j].associatedLabelParent = associatedLabelParent;
				builtMeshVertices[i,j].informationContent = informationContent;
				builtMeshVertices[i,j].isCloseToObjects = isCloseToObjects;
			}
			if (loopAround) {
				builtMeshVertices[i,0].left = builtMeshVertices[i,columns-1];
				builtMeshVertices[i,columns-1].right = builtMeshVertices[i,0];
			}
		}

		meshBuilt = true;
		if (connectAnotherBuilderOnTheLeft != null) {
			if (connectAnotherBuilderOnTheLeft.MeshIsBuilt()) {
				for (int i = 0; i < levels && i < connectAnotherBuilderOnTheLeft.levels; i++) {
					builtMeshVertices[i,0].left = connectAnotherBuilderOnTheLeft.builtMeshVertices[i,connectAnotherBuilderOnTheLeft.columns-1];
					connectAnotherBuilderOnTheLeft.builtMeshVertices[i,connectAnotherBuilderOnTheLeft.columns-1].right = builtMeshVertices[i,0];
				}
			}
		}
		if (connectAnotherBuilderOnTheRight != null) {
			if (connectAnotherBuilderOnTheRight.MeshIsBuilt()) {
				for (int i = 0; i < levels && i < connectAnotherBuilderOnTheRight.levels; i++) {
					builtMeshVertices[i,columns-1].right = connectAnotherBuilderOnTheRight.builtMeshVertices[i,0];
					connectAnotherBuilderOnTheRight.builtMeshVertices[i,0].left = builtMeshVertices[i,columns-1];
				}
			}
		}

		if (enterDefaultVertexOnStart) {
			if (defaultVertex.Length == 2) {
				ViewPointMesh mesh = GetComponentInParent<ViewPointMesh>();
				mesh.defaultVertex = builtMeshVertices[defaultVertex[0],defaultVertex[1]];
				GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().ActivateVertex(mesh.defaultVertex);
			} else {
				Debug.Log ("Couldn't enter default vertex as incorrectly specified.");
			}
		}


	}


	public void DrawGizmo(bool overrideVisualise = false) {

		if (visualise || overrideVisualise) {
			
			camPositions = new Vector3[levels,columns];
			
			float phi0 = anglePhiMin;
			float dPhi = (anglePhiMax-anglePhiMin)/(levels-1);
			float theta0 = transform.localRotation.eulerAngles.y + (thetaCentredAtOrigin ? -angleTheta/2f : 0f);
			float dTheta = angleTheta / (placeColumnAtBothEnds ? columns-1 : columns);
			
			for (int i = 0; i < levels; i++) {
				for (int j = 0; j < columns; j ++) {
					camPositions[i,j] = new Vector3(
						Mathf.Cos (Mathf.Deg2Rad * (theta0 + j*dTheta)) * Mathf.Cos (Mathf.Deg2Rad * (phi0 + i*dPhi)) * transform.localScale.x,
						Mathf.Sin (Mathf.Deg2Rad * (phi0 + i*dPhi)) * transform.localScale.y,
						Mathf.Sin (Mathf.Deg2Rad * (theta0 + j*dTheta)) * Mathf.Cos (Mathf.Deg2Rad * (phi0 + i*dPhi)) * transform.localScale.z);
					//Gizmos.DrawWireSphere(transform.position+camPositions[i,j],.01f);
					DrawCameraGizmo(camPositions[i,j]);
				}
			}

			Gizmos.color = Color.yellow;

			for (int i = 0; i < levels; i++) {
				for (int j = 0; j < columns; j ++) {
					if (i < levels-1)
						Gizmos.DrawLine(transform.position+camPositions[i,j],transform.position+camPositions[i+1,j]);
					if ((j < columns-1) || (loopAround))
						Gizmos.DrawLine(transform.position+camPositions[i,j],transform.position+camPositions[i,(j+1) % columns]);
				}
			}
			for (int i = 0; i < levels; i++) {
				for (int j = 0; j < columns; j ++) {
					DrawCameraGizmo(camPositions[i,j]);
				}
			}

			Gizmos.color = new Color(1f,1f,1f,0.3f);

			int x = levels/2;
			for (int j = 0; j < columns; j ++) {
				Gizmos.DrawLine(transform.position,transform.position+camPositions[x,j]);
			}
			
			meshBuilt = true;
			if (connectAnotherBuilderOnTheLeft != null) {
				if (connectAnotherBuilderOnTheLeft.MeshIsBuilt()) {
					
					Gizmos.color = Color.yellow;
					
					
					float phi0Left = connectAnotherBuilderOnTheLeft.anglePhiMin;
					float dPhiLeft = (connectAnotherBuilderOnTheLeft.anglePhiMax-connectAnotherBuilderOnTheLeft.anglePhiMin)/(connectAnotherBuilderOnTheLeft.levels-1);
					float theta0Left = connectAnotherBuilderOnTheLeft.transform.localRotation.eulerAngles.y + (connectAnotherBuilderOnTheLeft.thetaCentredAtOrigin ? -connectAnotherBuilderOnTheLeft.angleTheta/2f : 0f);
					float dThetaLeft = connectAnotherBuilderOnTheLeft.angleTheta / (connectAnotherBuilderOnTheLeft.placeColumnAtBothEnds ? connectAnotherBuilderOnTheLeft.columns-1 : connectAnotherBuilderOnTheLeft.columns);
					
					int j = connectAnotherBuilderOnTheLeft.columns-1;
					for (int i = 0; (i < levels && i < connectAnotherBuilderOnTheLeft.levels); i++) {
						Vector3 otherPos = connectAnotherBuilderOnTheLeft.transform.position;
						otherPos += new Vector3(
							Mathf.Cos (Mathf.Deg2Rad * (theta0Left + j*dThetaLeft)) * Mathf.Cos (Mathf.Deg2Rad * (phi0Left + i*dPhiLeft)) * connectAnotherBuilderOnTheLeft.transform.localScale.x,
							Mathf.Sin (Mathf.Deg2Rad * (phi0Left + i*dPhiLeft)) * connectAnotherBuilderOnTheLeft.transform.localScale.y,
							Mathf.Sin (Mathf.Deg2Rad * (theta0Left + j*dThetaLeft)) * Mathf.Cos (Mathf.Deg2Rad * (phi0Left + i*dPhiLeft)) * connectAnotherBuilderOnTheLeft.transform.localScale.z);
						Gizmos.DrawLine(transform.position+camPositions[i,0],otherPos);
					}
				}
			}
			if (connectAnotherBuilderOnTheRight != null) {
				if (connectAnotherBuilderOnTheRight.MeshIsBuilt()) {
					
					Gizmos.color = Color.yellow;
					
					
					float phi0Right = connectAnotherBuilderOnTheRight.anglePhiMin;
					float dPhiRight = (connectAnotherBuilderOnTheRight.anglePhiMax-connectAnotherBuilderOnTheRight.anglePhiMin)/(connectAnotherBuilderOnTheRight.levels-1);
					float theta0Right = connectAnotherBuilderOnTheRight.transform.localRotation.eulerAngles.y + (connectAnotherBuilderOnTheRight.thetaCentredAtOrigin ? -connectAnotherBuilderOnTheRight.angleTheta/2f : 0f);
					float dThetaRight = connectAnotherBuilderOnTheRight.angleTheta / (connectAnotherBuilderOnTheRight.placeColumnAtBothEnds ? connectAnotherBuilderOnTheRight.columns-1 : connectAnotherBuilderOnTheRight.columns);
					
					int j = 0;
					for (int i = 0; (i < levels && i < connectAnotherBuilderOnTheRight.levels); i++) {
						Vector3 otherPos = connectAnotherBuilderOnTheRight.transform.position;
						otherPos += new Vector3(
							Mathf.Cos (Mathf.Deg2Rad * (theta0Right + j*dThetaRight)) * Mathf.Cos (Mathf.Deg2Rad * (phi0Right + i*dPhiRight)) * connectAnotherBuilderOnTheRight.transform.localScale.x,
							Mathf.Sin (Mathf.Deg2Rad * (phi0Right + i*dPhiRight)) * connectAnotherBuilderOnTheRight.transform.localScale.y,
							Mathf.Sin (Mathf.Deg2Rad * (theta0Right + j*dThetaRight)) * Mathf.Cos (Mathf.Deg2Rad * (phi0Right + i*dPhiRight)) * connectAnotherBuilderOnTheRight.transform.localScale.z);
						Gizmos.DrawLine(transform.position+camPositions[i,columns-1],otherPos);
					}
				}
			}
		}

	}

	void DrawCameraGizmo(Vector3 pos) {

		Vector3 forward = (-pos).normalized;
		Vector3 right = Vector3.Cross(forward,new Vector3(0,1,0)).normalized;
		Vector3 up = Vector3.Cross(right,forward).normalized;
		
		Gizmos.color = Color.red;
		float scale = 0.02f;
		float arg1, arg2;

		for (int i = 0; i < 4; i++) {
			arg1 = Mathf.PI*(0.5f+i)/2f;
			arg2 = Mathf.PI*(1.5f+i)/2f;
			Gizmos.DrawLine (transform.position + pos + right * 2 * scale * Mathf.Sin (arg1) + up * scale * Mathf.Cos (arg1),
			                 transform.position + pos + right * 2 * scale * Mathf.Sin (arg2) + up * scale * Mathf.Cos (arg2));
			Gizmos.DrawLine (transform.position + pos + right * 4 * scale * Mathf.Sin (arg1) + up * 2 * scale * Mathf.Cos (arg1) + forward * 5 * scale,
			                 transform.position + pos + right * 4 * scale * Mathf.Sin (arg2) + up * 2 * scale * Mathf.Cos (arg2) + forward * 5 * scale);
			Gizmos.DrawLine (transform.position + pos + right * 2 * scale * Mathf.Sin (arg1) + up * scale * Mathf.Cos (arg1),
			                 transform.position + pos + right * 4 * scale * Mathf.Sin (arg1) + up * 2 * scale * Mathf.Cos (arg1) + forward * 5 * scale);
			
		}

	}

	public ViewPointMeshVertex GetDefaultVertex() {
		if (enterDefaultVertexOnStart) {
			return builtMesh[defaultVertex[0]-1,defaultVertex[1]-1].GetComponent<ViewPointMeshVertex>();
		} else {
			return transform.parent.GetComponent<ViewPointMesh>().defaultVertex;
		}
	}

	public bool MeshIsBuilt() {
		return meshBuilt;
	}
}
