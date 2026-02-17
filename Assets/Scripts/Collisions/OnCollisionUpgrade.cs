using UnityEngine;

/// <summary>
/// Upgrades an object by one if possible.
/// </summary>
[RequireComponent(typeof(Collider))]
public class OnCollisionUpgrade : MonoBehaviour {
	public Constants.CombatFlags upgradableObjects;
	
	/// <summary>
	/// Upgrades other.gameObject upon collision if possible.
	/// </summary>
	/// <param name="other">
	/// A <see cref="Collision"/>
	/// </param>
	void OnCollisionEnter(Collision other) {
		CollisionUpgrade(other);
	}
	
	/// <summary>
	/// Checks combat flags and upgrades if they match.
	/// </summary>
	/// <param name="other">
	/// A <see cref="Collision"/>
	/// </param>
	void CollisionUpgrade(Collision other) {
		Combat c = FindParentCombat(other.transform);
		if (c == null) return;
		Producer theProducer = c.GetComponent<Producer>();
		if (!theProducer) return;
		if ((c.thisObjectFlag & upgradableObjects) > 0) {
			if (theProducer.upgradeLevel < theProducer.upgradeLevelMax) {
				new MessageUpgradingObject(other.gameObject);
				theProducer.upgradeLevel += 1;
				Destroy(gameObject);
			}
		}
	}
	
	/// <summary>
	/// Checks combat flags and upgrades if they match.
	/// </summary>
	/// <param name="other">
	/// A <see cref="GameObject"/>
	/// </param>
	void CollisionUpgrade(GameObject other) {
		Combat c = FindParentCombat(other.transform);
		if (c == null) return;
		Producer theProducer = c.GetComponent<Producer>();
		if (!theProducer) return;
		if ((c.thisObjectFlag & upgradableObjects) > 0 && theProducer.upgradeLevel < theProducer.upgradeLevelMax) {
			new MessageUpgradingObject(other.gameObject);
			theProducer.upgradeLevel += 1;
			Destroy(gameObject);
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
