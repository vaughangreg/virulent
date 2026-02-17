using UnityEngine;

public class Genome : MonoBehaviour {
	Combat   m_combat;
	Producer m_producer;
	
	public GameObject LPDummyPrefab1;
	public GameObject LPDummyPrefab2;
	
	public float animationLengthSeconds;
	
	public GameObject pos1up;
	public GameObject pos2down;
	
	public AudioClip lPCollideSFX;
	public AudioClip nCollideSFX;
	
	public bool isAntiGenome = false;
	
	private float m_timing = 0f;
	protected bool m_shouldCheckArmor = false;
		
	void Start() {
		m_combat = GetComponent<Combat>();
		m_producer = GetComponent<Producer>();
		
		//listen for production upgrades
		Dispatcher.Listen(Dispatcher.OBJECT_UPGRADING,   gameObject);
		Dispatcher.Listen(Dispatcher.OBJECT_DOWNGRADING, gameObject);
		
		//disable dummy prefabs
		LPDummyPrefab1.active = false;
		LPDummyPrefab2.active = false;
		//reenable if necessary
		if (m_producer.upgradeLevel > 0) LPDummyPrefab1.active = true;
		if (m_producer.upgradeLevel > 1) LPDummyPrefab2.active = true;
	}
	
	void Update() {
		//ping pong the timing
		m_timing = Mathf.PingPong(Time.time / animationLengthSeconds, 1);
		
		LerpDummies();
		RotateDummies();
	}
	
	void LateUpdate() {
		if (m_shouldCheckArmor) CheckArmor();
	}
	
	/// <summary>
	/// Lerp the dummies between the two points
	/// </summary>
	void LerpDummies() {
		//lerp the dummies
		if (LPDummyPrefab1.active) {
			LPDummyPrefab1.transform.position = Vector3.Lerp(pos1up.transform.position, pos2down.transform.position, m_timing);
		}
		if (LPDummyPrefab2.active) {
			LPDummyPrefab2.transform.position = Vector3.Lerp(pos2down.transform.position, pos1up.transform.position, m_timing);
		}
	}
	
	/// <summary>
	/// Rotate the dummies at the proper rate.
	/// </summary>
	void RotateDummies() {
		if (LPDummyPrefab1.active) {
			LPDummyPrefab1.transform.localRotation = Quaternion.Euler(-1440 * m_timing + 90, 90, 0);
		}
		if (LPDummyPrefab2.active) {
			LPDummyPrefab2.transform.localRotation = Quaternion.Euler(1440 * m_timing + 90, 90, 0);
		}
	}
	
	/// <summary>
	/// On a collision invoke CheckArmor a half second later.
	/// </summary>
	/// <param name="collision">
	/// A <see cref="Collision"/>
	/// </param>
	//void OnCollisionEnter(Collision collision) { 
	//	Invoke("CheckArmor", 0.5f);
	//}
	
	/// <summary>
	/// Changes the genomes tag to match its armor state.  Sends 
	/// </summary>
	void CheckArmor() {
		string oldTag = gameObject.tag;
		if (m_combat.armorPoints > 0) {
			if (isAntiGenome) gameObject.tag = Constants.TAG_ARMORANTIGENOME;
			else gameObject.tag = Constants.TAG_ARMORGENOME;
			
			m_combat.thisObjectFlag = Constants.CombatFlags.GenomeArmored;
			m_combat.offenseFlag = Constants.CombatFlags.Proteasome;
		} else {
			if (isAntiGenome) gameObject.tag = Constants.TAG_ANTIGENOME; //handle antigenome case
			else gameObject.tag = Constants.TAG_GENOME; 
		
			m_combat.thisObjectFlag = Constants.CombatFlags.Genome;
			m_combat.offenseFlag = Constants.CombatFlags.Slicer;
		}
		if (oldTag != gameObject.tag) {
			new MessageTagTransition(gameObject, oldTag, gameObject.tag);
			if (gameObject.tag == Constants.TAG_ARMORGENOME) {
				MusicManager.PlaySfx(nCollideSFX);
			}
			else if (gameObject.tag == Constants.TAG_GENOME) {
				//TODO: Add in sheild take away sfx
			}
		}
		m_shouldCheckArmor = false;
	}
	/// <summary>
	/// On Upgrading Object enable the LP proteins as necessary
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageUpgradingObject"/>
	/// </param>
	void _OnUpgradingObject(MessageUpgradingObject e) {
		CheckUpgradeLevel(e);
		m_shouldCheckArmor = true;
	}
	
	void _OnDowngradingObject(MessageDowngradingObject e) {
		m_shouldCheckArmor = true;
	}
	
	void CheckUpgradeLevel(MessageUpgradingObject e) {
		if (e.upgradedObject == gameObject) {
			Invoke("DelayedCheck", .1f);
		}
	}
	
	void DelayedCheck() {
		if (m_producer.upgradeLevel > 0) {
				LPDummyPrefab1.active = true;
				MusicManager.PlaySfx(lPCollideSFX);
			}
			if (m_producer.upgradeLevel > 1) {
				LPDummyPrefab2.active = true;
				MusicManager.PlaySfx(lPCollideSFX);
			}
	}
	
	void OnCollisionEnter() {
		HandleShading();
	}
	
	void OnCollisionStay() {
		HandleShading();
	}
	
	/// <summary>
	/// Return a fraction = hitPoints divided by maxHitPoints
	/// </summary>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	float CalculateShade() {
		return ((float)m_combat.hitPoints/(float)m_combat.maxHitPoints);
	}
	
	/// <summary>
	/// Set the material color to the proper shade depending on HP.
	/// </summary>
	void HandleShading() {
		float shade = CalculateShade();
		GetComponent<Renderer>().material.color = new Color(shade, shade, shade, shade);
	}
}
