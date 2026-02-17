using UnityEngine;
using System.Collections;

public class TriggerObject : MonoBehaviour {
	public bool destroyAfterTrigger = true;
	public bool killCollisions = false;
	public string watchedTag = "";
	public GameObject watchedObj;
	
	/// <summary>
	/// Triggers a message to be sent
	/// </summary>
	/// <param name="someCollider">
	/// A <see cref="Collider"/>
	/// </param>
	void OnTriggerEnter(Collider someCollider) {
		if (someCollider.gameObject == watchedObj || someCollider.gameObject.tag == watchedTag) {
			new MessageCollisionEvent(gameObject, someCollider.gameObject);
			if (killCollisions){
				Destroy(someCollider.gameObject);
			}
			if (destroyAfterTrigger){
				Destroy(gameObject);
			}
		}
	}
}
