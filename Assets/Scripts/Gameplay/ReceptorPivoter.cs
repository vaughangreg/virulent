using UnityEngine;
using System.Collections;

public class ReceptorPivoter : MonoBehaviour {
	
	private Vector3 myLocalPos;
	private Quaternion myLocalRot;
	private Vector3 m_tarPos;
	private Quaternion m_tarRot;
	private bool setValues = false;
	private float m_counter = 0f;
	private GameObject m_parent;
	
	public GameObject targetObj;
	public Vector3 targetLoc;
	public Quaternion targetRot;
	
	// Use this for initialization
	void Start () {
		myLocalPos = transform.localPosition;
		myLocalRot = transform.localRotation;
		m_parent = transform.parent.gameObject;
	}
	
	public void SetTarget(GameObject i, Vector3 j, Quaternion k) {
		Vector3 tempVec = Vector3.zero; 
		tempVec = transform.parent.transform.InverseTransformPoint(tempVec); 
		if (i) {
			m_tarPos = i.transform.localPosition;
			m_tarRot = i.transform.localRotation;
		} else {
			m_tarPos = j;
			m_tarRot = k;
		} 
		m_tarPos.y = tempVec.y;
		setValues = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(!setValues) return;
		if(transform.parent == null || m_parent != transform.parent.gameObject) {
			Destroy(this);
			return;
		}
		m_counter += Time.deltaTime;
		transform.localPosition = Vector3.Lerp(myLocalPos, m_tarPos, m_counter);
		transform.localRotation = Quaternion.Slerp(myLocalRot, m_tarRot, m_counter);
	}
	
	void OnDestroy() {
		Destroy(this);
	}
}
