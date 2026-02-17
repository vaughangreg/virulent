using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Proteasome behavior.
/// 1. Seek all proteins and genome upgraded by N proteins.
/// 2. Return to golgi with any number of kidnapped N Protein.
/// 3. Denature when 3 of any protein are eaten.
/// </summary>
[RequireComponent(typeof(Combat))]
public class Proteasome : MonoBehaviour {
	public GameObject[] chompers;   // three chomper GameObjects that control chomping
	public GameObject   body;       // child object which rotates to face mouths to their targets
	
	float[]    m_offsets = new float[]{ -30, 90, -150 };
	Quaternion m_offsetRotation;
	
	GameObject m_closestChomper;						// the chomper currently closest to the target
	Vector3    m_targetPosition;						// the position of the target currently being seeked
	Quaternion m_bodyRotation;							// rotation of body object - used to nullify parent rotations
	
	public GameObject dummyPrefab; 
	
	List<GameObject> m_availableChompers;				// Chompers currently in use.
	
	/// <summary>
	/// Creates the list of chompers still available.
	/// </summary>
	void Awake() {
		m_availableChompers = new List<GameObject>(chompers);
	}
	
	/// <summary>
	/// Assign a chomper to m_closestChomper and a quat to m_bodyRotation to prevent null references
	/// </summary>
	void Start() {
		m_bodyRotation = Quaternion.identity;
		StartChomping();

		
		UnitSpawnEvent spawnevent = new UnitSpawnEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitSpawnEvent> (spawnevent);
	}

	/// <summary>
	/// Sets the target and calls FindClosestChomper.  Called from AIMOvable in a SendMessage.
	/// </summary>
	/// <param name="target">
	/// A <see cref="Vector3"/>
	/// </param>
	void SetTargetPosition(Vector3 target) {
		m_targetPosition = target;
		FindClosestChomper();
	}
	
	/// <summary>
	/// Displays the number of hitpoints left as the number of chomping chompers.
	/// </summary>
	void StartChomping() {
		foreach (GameObject aChomper in chompers) {
			aChomper.SendMessage("StartChomping");
		}
	}
	
	/// <summary>
	/// Finds which mouth is closest to the target and starts it chomping
	/// </summary>
	void FindClosestChomper() {
		float bestToTarget = float.MaxValue;
		
		foreach (GameObject aChomper in m_availableChompers) {
			float chomperToTarget = (m_targetPosition - aChomper.transform.position).sqrMagnitude;
			if (chomperToTarget < bestToTarget) {
				m_closestChomper = aChomper;	
			}
		}
		
		for (int i = 0; i < chompers.Length; ++i) {
			if (chompers[i] == m_closestChomper) {
				m_offsetRotation = Quaternion.Euler(new Vector3(0, m_offsets[i], 0));
				break;
			}
		}
	}
	
	/// <summary>
	/// Identify the protein type, create a dummy and invoke a destroy on the dummy to take place a moment later.
	/// </summary>
	/// <param name="protein">
	/// A <see cref="GameObject"/>
	/// </param>
	void EatProtein(GameObject protein) {
		m_closestChomper.SendMessage("StopChomping");
		m_availableChompers.Remove(m_closestChomper);
	}
	
	void TakingDamage() {
		m_closestChomper.SendMessage("StopChomping");
		m_availableChompers.Remove(m_closestChomper);
	}
	 
	///<summary>
	/// A simple death for the proteasome
	/// TODO: Needs a better death.
	///</summary>
	void DeathPrep() {
		GetComponent<Rigidbody>().useGravity = true;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		Instantiate(dummyPrefab, body.transform.position, body.transform.rotation);

		
		UnitDeathEvent deathevent = new UnitDeathEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitDeathEvent> (deathevent);

		Destroy(gameObject);
	}

	/// <summary>
	/// Nullify parent rotations and rotate mouth towards target.
	/// </summary>
	void LateUpdate() {
		body.transform.rotation = m_bodyRotation;
		
		Quaternion rotation = m_offsetRotation * Quaternion.LookRotation(m_targetPosition - body.transform.position);
		body.transform.rotation = Quaternion.Slerp(body.transform.rotation, rotation, Time.deltaTime * 5.0f);
		m_bodyRotation = body.transform.rotation;
	}
	
	/// <summary>
	/// Draws a sphere by the active mouth, a line to the active mouth and a line to its target.
	/// </summary>
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		if (m_closestChomper) Gizmos.DrawSphere(m_closestChomper.transform.position, 5);
		if (m_closestChomper) Gizmos.DrawLine(m_closestChomper.transform.position, transform.position);
		Gizmos.DrawLine(m_targetPosition, transform.position);
	}
}
