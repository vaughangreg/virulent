using UnityEngine;

public class Slicer : MonoBehaviour {
	
	public GameObject DummyPrefab;
	public AudioClip deathSFX;
	
	private Combat m_combat;
	private Renderer m_renderer;
	
	public GameObject mySpinningModel;
	public GameObject[] myDamagePieces;
	
	void Start() {
		m_combat = GetComponent<Combat>();
		if (m_combat.maxHitPoints != myDamagePieces.Length) Debug.Log("You don't have the same damage items as maxHitpoints set up for the Slicer");
		m_renderer = GetComponentInChildren<Renderer>();

		
		UnitSpawnEvent spawnevent = new UnitSpawnEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitSpawnEvent> (spawnevent);
	}
	
	void TakingDamage() {
		if (m_combat.maxHitPoints - m_combat.hitPoints - 1 < 0) return;
		GameObject someOfMyDamage = (GameObject)Instantiate(myDamagePieces[Mathf.Clamp(m_combat.maxHitPoints - m_combat.hitPoints - 1, 0, myDamagePieces.Length)],
			mySpinningModel.transform.position, mySpinningModel.transform.rotation);
		someOfMyDamage.transform.parent = mySpinningModel.transform;
	}
	
	void OnCollisionEnter() {
		//HandleShading();
	}
	
	void OnCollisionStay() {
		//HandleShading();
	}
	
	void HandleShading() {
		float shade = CalculateShade();
		m_renderer.material.color = new Color(shade, shade, shade, shade);
	}
	
	/// <summary>
	/// Return a fraction = hitPoints divided by maxHitPoints
	/// </summary>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	float CalculateShade() {
		float baseValue = 0.3f;
		float range = 0.7f;
		
		return baseValue + range * ((float)m_combat.hitPoints/(float)m_combat.maxHitPoints);
	}
	
	void DeathPrep() {
		Instantiate(DummyPrefab, transform.position, transform.rotation);
		//tempObj.renderer.material.color = new Color(.25f, .25f, .25f, .25f);
		MusicManager.PlaySfx(deathSFX);
		
		UnitDeathEvent deathevent = new UnitDeathEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitDeathEvent> (deathevent);

		Destroy(gameObject);
	}
}
