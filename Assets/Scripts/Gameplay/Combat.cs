using UnityEngine;

/// <summary>
/// Handles damage dealt and taken. Note that healing
/// and damage currently use the same amount.
/// 
/// Sends a "DeathPrep" message to self when killed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Combat : MonoBehaviour {
	public int hitPoints      = 1;		// Death at 0.
	public int maxHitPoints   = 1;		// Max HP.
	public int armorPoints    = 0;
	public int maxArmorPoints = 0;		// Max armor, which prevents damage to hit points

	public int changeAmount = 1;		// How many HP or AP should change?
	
	public Constants.CombatFlags thisObjectFlag;	// Flag indicating what we are.
	public Constants.CombatFlags offenseFlag;		// We damage these objects.
	public Constants.CombatFlags armorIncreaseFlag;	// We heal these objects.
	public Constants.CombatFlags armorDecreaseFlag; // We remove armor from these objects.
	
	public GameObject killer;						// The gameObject that dealt the final blow when killed.
	public GameObject attacker;						// The gameObject that attacked us (which may or may not kill us).

	public GameObject shield;						// The object to display when shields are up.

	public const string MESSAGE_DEATH        = "DeathPrep";
	public const string MESSAGE_DAMAGE       = "TakingDamage";
	public const string MESSAGE_SETDESTROYER = "SetDestroyer";

	/// <summary>
	/// Passes on the collision to Collide.
	/// </summary>
	/// <param name="collision">
	/// A <see cref="Collision"/>
	/// </param>
	public void OnCollisionEnter(Collision collision) { Collide(collision); }
	
	/// <summary>
	/// Passes on the collision to Collide.
	/// </summary>
	/// <param name="collision">
	/// A <see cref="Collision"/>
	/// </param>
	void OnCollisionStay(Collision collision) { Collide(collision); }

	/// <summary>
	/// Finds a combat script in the object or recursively checks in its parent.
	/// </summary>
	/// <param name="current">
	/// A <see cref="Transform"/>
	/// </param>
	/// <returns>
	/// A <see cref="Combat"/>
	/// </returns>
	public Combat FindParentCombat(Transform current) {
		GameObject currentObject = current.gameObject;
		Combat c = currentObject.GetComponent<Combat>();
		
		if (c) return c;
		if (current.parent == null) return null;
		return FindParentCombat(current.parent);
	}

	/// <summary>
	/// Deals damage or heals as needed.
	/// </summary>
	/// <param name="collision">
	/// A <see cref="Collision"/>
	/// </param>
	protected void Collide(Collision collision) {
		new MessageCollisionEvent(gameObject, collision.gameObject);
		Combat c = FindParentCombat(collision.transform);
		if (c == null) {
			return;
		}

		HandleCollision(c);
	}

	/// <summary>
	/// Deals damage or heals as needed.
	/// </summary>
	/// <param name="combat">
	/// A <see cref="Combat"/>
	/// </param>
	public void HandleCollision(Combat c) {
		// Deal with armor first, then handle damage. This way, if an N and a slicer
		// hit simultaneously, the genome gets saved and the player should be happy.
		
		if ((thisObjectFlag & c.armorIncreaseFlag) > 0 && armorPoints < maxArmorPoints) {
			// Increases armor, up to the max
			if (armorPoints < 1 && c.changeAmount > 0) ShowShield();
			armorPoints = Mathf.Min(armorPoints + c.changeAmount, maxArmorPoints);
			
			// We can't use the OnCollisionDestroy script because
			// we don't want to ALWAYS destroy the object. We should
			// only do so when it actually increases the armor.
			Destroy(c.gameObject);	
			new MessageUpgradingObject(gameObject);
		}
		
		if ((thisObjectFlag & c.armorDecreaseFlag) > 0 && armorPoints > 0) {
			// Decreases armor, down to 0
			armorPoints = Mathf.Max(armorPoints - c.changeAmount, 0);
			if (armorPoints <= 0) HideShield();
			new MessageDowngradingObject(gameObject);
		}
		
		if ((thisObjectFlag & c.offenseFlag) > 0) {
			// NOTE: This differs from the original version where
			// we only did damage IFF c.armorPoints was also <= 0.
			// Not really certain of a good rationale for that…just
			// guessing that cell units don't have armor, so it was
			// never an issue.
			if (armorPoints <= 0) {

				UnitCollisionEvent col = new UnitCollisionEvent(gameObject,c.gameObject,c.changeAmount);
				
				ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
				ADAGE.LogData<UnitCollisionEvent>(col);

				// Debug.Log("Hit by " + c.gameObject.name + " for " + c.changeAmount);
				hitPoints -= c.changeAmount;
				attacker   = c.gameObject;
				SendMessage(MESSAGE_DAMAGE, SendMessageOptions.DontRequireReceiver);
				//if my hitpoints hit zero, I die!
				if (hitPoints <= 0) {
					killer = c.gameObject;
					
					// Only send this message if the CountedObject component is attached
					if (gameObject.GetComponent<CountedObject>() != null) {
						SendMessage(MESSAGE_SETDESTROYER, killer, SendMessageOptions.DontRequireReceiver);
					}
					SendMessage(MESSAGE_DEATH, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	#region Shield/armor management
	public float shieldTransitionSecs = 1.0f;

	protected Vector3        m_realScale;										// The original shield scale.
	protected static Vector3 m_minimalScale = new Vector3(0.01f, 0.01f, 0.01f);	// Something close to zero.
	protected float          m_direction = 1.0f;
	protected float          m_currentTime = 0.0f;
	
	/// <summary>
	/// Sets up the shield, disabling Combat if needed.
	/// </summary>
	protected void Start() {
		if (!shield) {
			enabled = false;
			return;
		}
		if (armorPoints == 0) {
			shield.active = false;
			enabled = false;
		}
		else {
			shield.active = true;
			enabled = true;
			m_currentTime = 1;
		}
		m_realScale                 = shield.transform.localScale;
		shield.transform.localScale = m_minimalScale;
		
	}
	
	/// <summary>
	/// Scales the shield, growing or shrinking as necessary.
	/// </summary>
	protected void Update() {
		if (!shield) {
			enabled = false;
			return;
		}
		m_currentTime += Time.deltaTime / shieldTransitionSecs * m_direction;
		m_currentTime  = Mathf.Clamp01(m_currentTime);
		
		shield.transform.localScale = Vector3.Slerp(m_minimalScale, m_realScale, m_currentTime);
		
		if (m_direction < 0.0f && m_currentTime == 0) {
			// We only disable ourselves if the shield is gone.
			shield.active = false;
			enabled       = false;
		}
	}
	
	/// <summary>
	/// Enables combat to grow the shield.
	/// </summary>
	protected void ShowShield() {
		if (!shield) return;
		if (enabled) return;
		
		m_direction   = 1.0f;
		m_currentTime = 0.0f;
		shield.active = true;
		enabled       = true;
	}
	
	/// <summary>
	/// Disables combat to shrink the shield.
	/// </summary>
	protected void HideShield() {
		if (!shield) return;
		m_direction   = -1.0f;
		m_currentTime = 1.0f;
		enabled       = true;
	}
	#endregion
}
