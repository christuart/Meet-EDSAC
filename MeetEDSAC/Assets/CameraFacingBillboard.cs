using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
	public Camera m_Camera;
	
	void Update()
	{
		Vector3 camBack = m_Camera.transform.rotation * Vector3.back;
		transform.LookAt(transform.position + camBack,
		                 Vector3.up);
	}
}