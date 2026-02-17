using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PNodes {
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 localPosition;
	public Quaternion localRotation;
}

public class PathNodeV2 : MonoBehaviour {
	
	public GameObject nextNode;
	public PNodes[] myPath = new PNodes[0];
	public List<Vector3> pathPositions;
	public List<Quaternion> pathRotations;
	public bool transformsAreLocal = false;
	
	void Start() { 
		if (myPath == null) return;
		pathPositions = new List<Vector3>();
		pathRotations = new List<Quaternion>();
		
		for (int i = 0; i < myPath.Length; i++) { 
			pathPositions.Add(myPath[i].position);
			pathRotations.Add(myPath[i].rotation);
		}
		
		GameObject m_next = nextNode;
		if (!m_next) return;
		PathNodeV2 m_nextPathNode = m_next.GetComponent<PathNodeV2>();
		
		while (m_next != null) {
			m_nextPathNode = m_next.GetComponent<PathNodeV2>();
			for (int i = 0; i < m_nextPathNode.myPath.Length; i++) { 
				pathPositions.Add(m_nextPathNode.myPath[i].position);
				pathRotations.Add(m_nextPathNode.myPath[i].rotation);
			}
			m_next = m_next.GetComponent<PathNodeV2>().nextNode;
		}
	}
	
	void OnDrawGizmosSelected()
	{
		if (myPath == null || myPath.Length == 0) return;
		Gizmos.color = new Color(0.4f,1,0.4f,0.8f); 
		
		Gizmos.DrawSphere(myPath[0].position, 8);
		for (int i = 1; i < myPath.Length; i++) { 
			Gizmos.color = new Color(1,1,1,1);
			Gizmos.DrawLine(myPath[i-1].position, myPath[i].position);
			Gizmos.color = new Color(1.0f,0.4f,0.4f,0.8f);
			Gizmos.DrawSphere(myPath[i].position, 8);
		}
		
		Vector3 prevPos = myPath[myPath.Length - 1].position;
		GameObject m_next = nextNode;
		if (!m_next) return;
		PathNodeV2 m_nextPathNode = m_next.GetComponent<PathNodeV2>();
		
		while (m_next != null) {
			m_nextPathNode = m_next.GetComponent<PathNodeV2>();
			for (int i = 0; i < m_nextPathNode.myPath.Length; i++) { 
				Gizmos.color = new Color(1,1,1,1);
				Gizmos.DrawLine(prevPos, m_nextPathNode.myPath[i].position);
				Gizmos.color = new Color(1.0f,0.4f,0.4f,0.8f);
				Gizmos.DrawSphere(m_nextPathNode.myPath[i].position, 8);
				prevPos = m_nextPathNode.myPath[i].position;
			}
			
			m_next = m_next.GetComponent<PathNodeV2>().nextNode;
		}
	}
	void OnDrawGizmos()
	{
		if (myPath == null || myPath.Length == 0) return;
		if (transformsAreLocal) {
			myPath[0].position = transform.TransformPoint(myPath[0].localPosition);
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(myPath[0].position, 3);
	}
}
