using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour {
	
	public Color gizmoColor = Color.blue;
	public int gizmoSize = 10;
	
	void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		Gizmos.DrawSphere(transform.position, gizmoSize);
	}
}
