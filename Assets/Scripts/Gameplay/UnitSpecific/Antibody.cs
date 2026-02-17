using UnityEngine;

/// <summary>
/// Antibody-specific behavior.
/// </summary>
//[RequireComponent(typeof(Combat))]
public class Antibody: MonoBehaviour {
	public Combat 		combat;
	public const string MESSAGE_DAMAGE = "TakingDamage";
	public const string ANIMATE_DAMAGE = "AnimateDamage";
	public GameObject virionIKilled;
	private GameObject targetLoc;
	public AudioClip	deathClip;
	
	protected Transform m_trans;
	
	protected enum State {
		Idle,
		Moving
	}
	protected State			m_state = State.Idle;
	protected float			m_time = 0;
	protected Vector3		m_originalPosition;
	protected Quaternion	m_originalRotation;
	
	void Awake() {
		m_trans = transform;	
	}
	
	/// <summary>
	/// Links up the combat script.
	/// </summary>
	void Start() {
		combat = GetComponent<Combat>();
		UnitSpawnEvent spawnevent = new UnitSpawnEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.LogData<UnitSpawnEvent> (spawnevent);
	}
	
	/// <summary>
	/// Handle state-based actions.
	/// </summary>
	void Update() {
		switch (m_state) {
			case State.Idle: return;
			case State.Moving: MoveToObject(); return;
			default: return;
		}
	}
	
	/// <summary>
	/// Moves towards our killer.
	/// </summary>
	void MoveToObject() {
		m_time += Time.deltaTime;
		if (!combat.killer && !virionIKilled) {
			m_state = Antibody.State.Idle;
			m_time = 0.0f;
			GetComponent<SphereCollider>().isTrigger = false;
			AIMovable mover = GetComponent<AIMovable>();
			if (mover) mover.FindClosestTarget();
			return;
		}
		else if (m_time <= 1.0f && (combat.killer || virionIKilled)) {
			if (combat.killer) targetLoc = combat.killer;
			if (virionIKilled) targetLoc = virionIKilled; 
			
			m_trans.position = Vector3.Lerp(m_originalPosition, 
		                                    targetLoc.transform.position + Vector3.up * 20.0f,
		                                    m_time);
			m_trans.rotation = Quaternion.Lerp(m_originalRotation, 
			                               	   targetLoc.transform.rotation,
			                             	   m_time);
		}
		else { 
			if (combat.killer) combat.killer.SendMessage(ANIMATE_DAMAGE);
			if (virionIKilled) virionIKilled.SendMessage(ANIMATE_DAMAGE);
			
			//transform.position = combat.killer.transform.position;
			
			if (!combat.killer) combat.killer = virionIKilled;
			SendMessage("SetDestroyer",combat.killer,SendMessageOptions.DontRequireReceiver);
			//^^ I don't like doing this, this seems really hacky or something...
			
			Destroy(gameObject);
		}
	}
	
	/// <summary>
	/// Destroys components we don't need.
	/// </summary>
	void DestroyComponents() {
		Destroy(GetComponent<AIMovable>());
		Destroy(GetComponent<MovementLerped>());
	}
	
	/// <summary>
	/// Destroy ourselves.
	/// </summary>
	void DeathPrep() {
		GameObject killer = combat.killer;
		if (killer) {
			Combat killerCombat = killer.GetComponent<Combat>();
			if (killerCombat && killerCombat.hitPoints < 0) {
				combat.hitPoints += 1;
				return;
			}
		}
		
		// Assumes that these exist; if not, check for them.
		m_originalPosition = m_trans.position;
		m_originalRotation = m_trans.rotation;
		m_state = Antibody.State.Moving;
		GetComponent<SphereCollider>().isTrigger = true;
		
		MusicManager.PlaySfx(deathClip);
		
		UnitDeathEvent deathevent = new UnitDeathEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitDeathEvent> (deathevent);
	}
	
	/// <summary>
	/// This is called when the combat script takes damage.
	/// </summary>
	void TakingDamage() { }
}
