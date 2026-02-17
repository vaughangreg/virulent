using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
	public Vector3 spinAxis;
	public float degreesPerSec;
	
	private Quaternion m_rotation;
	
	
	void LateUpdate()
	{
		transform.localRotation = m_rotation;
		
		transform.Rotate(spinAxis,degreesPerSec*Time.deltaTime);
		
		m_rotation = transform.localRotation;
	}
}

