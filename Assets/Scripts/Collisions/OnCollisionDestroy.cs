using UnityEngine;

/// <summary>
/// Destroys self upon collision with a lethal object.
/// </summary>
[RequireComponent(typeof(Collider))]
public class OnCollisionDestroy : MonoBehaviour {
	public Constants.CombatFlags lethalObjects;
	public float delay = 0.0f;
	
	/// <summary>
	/// Destroys gameObject upon collision with a lethal object.
	/// </summary>
	/// <description>
	/// May need to add Combat.MESSAGE_DEATH before the
	/// new MessageObjectDestroy.
	/// </description>
	/// <param name="other">
	/// A <see cref="Collision"/>
	/// </param>
	void OnCollisionEnter(Collision other) {
		DieOnLethal(other);		
	}
	
	/// <summary>
	/// Destroys ourselves if we hit a lethal object.
	/// </summary>
	/// <param name="other">
	/// A <see cref="Collider"/>
	/// </param>
	void OnTriggerEnter(Collider other) {
		DieOnLethal(other.gameObject);	
	}
	
	/// <summary>
	/// Checks to see if the object it collided with will kill it and destroys itself if so.
	/// </summary>
	/// <param name="other">
	/// A <see cref="Collision"/>
	/// </param>
	void DieOnLethal(Collision other) {
		Combat c = FindParentCombat(other.transform);
		if (c == null) return;
		
		if (((int)c.thisObjectFlag & (int)lethalObjects) > 0) {
			Destroy(gameObject, delay);	
		}
	}
	
	/// <summary>
	/// Checks to see if the object it collided with will kill it and destroys itself if so.
	/// </summary>
	/// <param name="other">
	/// A <see cref="GameObject"/>
	/// </param>
	void DieOnLethal(GameObject other) {
		Combat c = FindParentCombat(other.transform);
		if (c == null) return;
		
		if (((int)c.thisObjectFlag & (int)lethalObjects) > 0) {
			Destroy(gameObject, delay);	
		}
	}
	
	/// <summary>
	/// Finds the combat script attached to the object or the closest parent.
	/// </summary>
	/// <param name="current">
	/// A <see cref="Transform"/>
	/// </param>
	/// <returns>
	/// A <see cref="Combat"/>
	/// </returns>
	protected Combat FindParentCombat(Transform current) {
		GameObject currentObject = current.gameObject;
		Combat c = currentObject.GetComponent<Combat>();
		
		if (c) return c;
		if (current.parent == null) return null;
		return FindParentCombat(current.parent);
	}
}
