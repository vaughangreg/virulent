using UnityEngine;

public class ProteasomeChomper : MonoBehaviour {
	public Transform jointA;
	public Transform jointB;
	
	public GameObject damagePrefab;
	public GameObject root;
	
	public float openSpeed = 10.0f;
	public float openWidth = 55;

	private bool m_isClosed   = true;
	private bool m_isChomping = false;

	
	void Update() {
		if (m_isChomping) {
			if (m_isClosed) OpenMouth();
			else CloseMouth();
		}
		else {
			CloseMouth();	
		}
	}
	
	void OpenMouth() {
		jointA.localRotation = Quaternion.Slerp(jointA.localRotation, Quaternion.Euler(new Vector3(0, openWidth, 0)), Time.deltaTime * openSpeed);
		jointB.localRotation = Quaternion.Slerp(jointB.localRotation, Quaternion.Euler(new Vector3(0, -openWidth, 0)), Time.deltaTime * openSpeed);
		if (jointA.transform.localRotation.eulerAngles.y > (openWidth - 6)) m_isClosed = false;
	}
	void CloseMouth() {
		jointA.localRotation = Quaternion.Slerp(jointA.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * openSpeed);
		jointB.localRotation = Quaternion.Slerp(jointB.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * openSpeed);
		if (jointA.localRotation.eulerAngles.y < 6) m_isClosed = true;
	}
	
	public void StartChomping() {
		m_isChomping = true;
	}
	
	public void StopChomping() {
		if (m_isChomping) {
			m_isChomping = false;
			//Create a damage object to appear on top of us
			Vector3 position = transform.position + new Vector3(0,1,0);
			Quaternion rotation = transform.rotation * Quaternion.Euler(0,90,0);
			GameObject damage = Instantiate(damagePrefab,position,rotation) as GameObject;
			damage.transform.parent = gameObject.transform;
		}
	}
}
