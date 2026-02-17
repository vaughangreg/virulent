using UnityEngine;

/// <summary>
/// Removes the attached object.
/// </summary>
public class Protein : MonoBehaviour {
	public GameObject deathPrefab;
	public AudioClip deathSound;
	
	private Combat m_combat;
	
	void Start() {
		m_combat = GetComponent(typeof(Combat)) as Combat;
		UnitSpawnEvent spawnevent = new UnitSpawnEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitSpawnEvent> (spawnevent);
	}
	
	/// <summary>
	/// Give feedback and destroy ourselves.
	/// </summary>
	void DeathPrep() {
		if (m_combat.killer != null) {
			if (m_combat.killer.tag == Constants.TAG_PROTEASOME) {
				m_combat.killer.SendMessage("EatProtein", gameObject);
			}
			else {
				if (m_combat.killer.tag == Constants.TAG_PRR) {
					m_combat.killer.SendMessage("GrabProtein", gameObject);
				}
				else {
				GameObject result = null;
				if (deathPrefab) result = (GameObject)Instantiate(deathPrefab, transform.position, transform.rotation);
				if (result && deathSound) MusicManager.PlaySfx(deathSound);
				}
			}
		}

		UnitDeathEvent deathevent = new UnitDeathEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitDeathEvent> (deathevent);
		Destroy(gameObject);
	}
}
