using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Capsid-specific behaviors.
/// </summary>
public class Capsid : MonoBehaviour {
	public GameObject prefabExplode;
	public GameObject prefabDummy;
	public GameObject prefabDeadDummy;
	public Combat combat;
	public bool isDead = false;
	private bool timeToDie = false; 
	public GameObject[] activateOnFirstDamage;
	public GameObject[] activateOnSecondDamage;
	private string ON_SLOW = "OnSlow";
	private GameObject newVirion;
	private GameObject firstAntibodyToHitMe;
	public List<GameObject> listOfAttackingVirions;
	
	private Transform m_trans;
	
	// Both of the following lines were commented out because they were not used by any code besides the reference below
	//private Transform m_lockedTransform;  //locked local position and orientation on invagination
	//private bool m_isTransformLocked = false;
	
	void Awake() {
		m_trans = transform;	
	}
	
	void Start() { 
		listOfAttackingVirions = new List<GameObject>();
		
		UnitSpawnEvent spawnevent = new UnitSpawnEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitSpawnEvent> (spawnevent);
	}
	
	void TakingDamage() {  
		listOfAttackingVirions.Add(combat.attacker);
		SendMessage(ON_SLOW);
	}
	
	/// <summary>
	/// Activates children at different points in taking damage.
	/// </summary>
	void AnimateDamage() {
		for (int i = 0; i < activateOnFirstDamage.Length; ++i) {
			GameObject tempObj = (GameObject)Instantiate(activateOnFirstDamage[i], 
			                                             m_trans.position + Vector3.up, 
			                                             m_trans.rotation);
			tempObj.transform.parent = m_trans;
		}
	}
	
	/// <summary>
	/// Destroys ourself if a monomer.  Creates a dummy object if the final pentamer hits.
	/// </summary>
	void DeathPrep() {
		if (!isDead) {
			if (combat.killer != null) {
				if (combat.killer.CompareTag(Constants.TAG_MONOMER)) {
					//Create a dummy object to get sucked into the B Cell
					GameObject instance = GameObject.Instantiate(prefabDummy,m_trans.position,m_trans.rotation) as GameObject;
					instance.transform.parent = combat.killer.transform;
					
					//destroy myself
					Destroy(gameObject);
				}
				else { 
					// creates a dummy capsid that has final animation on it
					newVirion = Instantiate(prefabDeadDummy, m_trans.position, m_trans.rotation) as GameObject;
					timeToDie = true;
				}
			}
			else Destroy(gameObject);
			isDead = true;
		}
	}

	void OnDestroy(){
		UnitDeathEvent deathevent = new UnitDeathEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitDeathEvent> (deathevent);
	}
	
	void LateUpdate() {
		if (!timeToDie) return;
		
		// I now destroy myself 
		foreach(GameObject i in listOfAttackingVirions) {
			if (i != null) i.GetComponent<Antibody>().virionIKilled = newVirion;
		} 
		Destroy(gameObject);
	}
	
	/// <summary>
	/// Locks the local orientation and position of capsid to avoid bumping from physics
	/// </summary>
	void LockTransform() {
		GetComponent<Rigidbody>().isKinematic = true;
	}
	
	void UnlockTransform() {
		GetComponent<Rigidbody>().isKinematic = false;
	}
}
