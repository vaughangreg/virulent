using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Selectable))]
public class ProteinReceptorV2: MonoBehaviour {
	public bool spawnReceptors = true;
	public GameObject[] proteinReceptors;
	public GameObject receptorDummy;
	public GameObject nextNode;
	public bool m_insideCell = true;  
	private List<Vector3> m_nodePositions;
	private int m_moveCounter = 0;
	private GameObject m_receptor;
	public GameObject m_targetObject;
	
	private Selectable m_selectable;
	
	// Use this for initialization
	void Awake () {
		m_selectable = GetComponent<Selectable>();
		Dispatcher.Listen(Dispatcher.MOVE_COMPLETE, this);
	}
	
	public void MoveStateChange() {
		m_moveCounter++;
		if(m_moveCounter == 1) {
			m_insideCell = !m_insideCell;
			SpawnReceptor();
		}
		if(m_moveCounter == 3) {
			KillBeacon();
		}
		if(m_moveCounter == 4) {
			m_moveCounter = 0;
			m_insideCell = !m_insideCell;
			KillReceptor();
		}
	}
		
	void KillBeacon() { // this doesnt really do shit.... it kills beacons if the PR is out and starts to move back in.  
		//Something more complex is needed to kill beacons while PR is moving back into cell 
		if(!m_receptor) return;
		ScaleV2 m_sc2 = GetComponentInChildren<ScaleV2>();
		GameObject m_beacon = null;
		if (m_sc2) m_beacon = m_sc2.gameObject;
		if (m_beacon) Destroy(m_beacon);
	}
	
	
	void KillReceptor() {
		if(!m_receptor) return;
		if (m_receptor.transform.parent == transform) Destroy(m_receptor);
		else m_receptor = null;
	}
	
	void SpawnReceptor() {
		m_receptor = (GameObject)Instantiate(proteinReceptors[Random.Range(0, proteinReceptors.Length - 1)], transform.position - new Vector3(0,20f,0), transform.rotation);
		m_receptor.transform.parent = transform;
		//if (!m_selectable) m_selectable = GetComponent<Selectable>();
		m_selectable.messageDisplay = m_receptor.tag;
	}
	
	void OnCollisionEnter(Collision c) {
		if(!m_receptor) return;
		if (c.gameObject.CompareTag(Constants.TAG_CAPSID) && c.transform.parent == null) {
			if(m_receptor.transform.parent == transform && m_receptor.CompareTag(Constants.TAG_PROTEINRECEPTOR)) {
				
				List<Vector3> m_pathPos = GetComponent<PathNodeV2>().pathPositions;
				List<Quaternion> m_pathRot = GetComponent<PathNodeV2>().pathRotations;
				GameObject dummy = (GameObject)Instantiate(receptorDummy, m_receptor.transform.position, m_receptor.transform.rotation);
				c.transform.parent = dummy.transform;
				ReceptorPivoter rp = c.gameObject.AddComponent<ReceptorPivoter>();
				rp.SetTarget(dummy.transform.Find("CapsidPosition").gameObject, new Vector3(), new Quaternion());
				c.gameObject.GetComponent<MovementLerped>().ClearPath();
				c.gameObject.layer = LayerMask.NameToLayer("Invagination");
				dummy.GetComponent<MovementLerped>().AddWorldPoint(dummy.transform.position, dummy.transform.rotation);
				for (int i = 0; i < m_pathPos.Count; i++) {
					//Debug.Log(m_pathRot[i]);
					dummy.GetComponent<MovementLerped>().AddWorldPoint(m_pathPos[i], m_pathRot[i]);
				}
				KillReceptor();
				Destroy(c.gameObject.GetComponent<Movable>());
			}
		}
	}
}
