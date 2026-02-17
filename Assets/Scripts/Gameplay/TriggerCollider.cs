using UnityEngine;

public class TriggerCollider: MonoBehaviour {
	Combat combat;
	
	void Start() {
		combat = transform.parent.gameObject.GetComponent<Combat>();
	}
	
	/// <summary>
	/// Simulates a collision with the trigger collider before a physics collision can happen.
	/// </summary>
	/// <param name="collider">
	/// A <see cref="Collider"/>
	/// </param>
	void OnTriggerEnter(Collider collider) {
		if (!collider.gameObject.GetComponent<Combat>()) return;
		combat.HandleCollision(collider.gameObject.GetComponent<Combat>());
		collider.gameObject.GetComponent<Combat>().HandleCollision(combat);
		transform.parent.SendMessage("CollisionUpgrade", collider.gameObject, SendMessageOptions.DontRequireReceiver);
		transform.parent.SendMessage("DieOnLethal", collider.gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
