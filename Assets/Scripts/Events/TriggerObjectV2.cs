using UnityEngine;
using System.Collections;

public class TriggerObjectV2 : MonoBehaviour {
	public bool destroyAfterTrigger = true;
	public bool killCollisions = false;
	public string[] watchedTags;
	public GameObject[] watchedObjs;
	
	/// <summary>
	/// Triggers a message to be sent
	/// </summary>
	/// <param name="someCollider">
	/// A <see cref="Collider"/>
	/// </param>
	void OnTriggerEnter(Collider someCollider) {
		foreach(string i in watchedTags) {
			if (someCollider.gameObject.tag == i) {
				new MessageCollisionEvent(gameObject, someCollider.gameObject);
				if (killCollisions){
					someCollider.gameObject.SendMessage("SetDestroyer", gameObject);
					Destroy(someCollider.gameObject);
				}
				if (destroyAfterTrigger){
					Destroy(gameObject);
				}
			}
		}
		
		
		foreach(GameObject i in watchedObjs) {
			if (someCollider.gameObject == i) {
				new MessageCollisionEvent(gameObject, someCollider.gameObject);
				if (killCollisions){
					someCollider.gameObject.SendMessage("SetDestroyer", gameObject);
					Destroy(someCollider.gameObject);
				}
				if (destroyAfterTrigger){
					Destroy(gameObject);
				}
			}
		} 
	}
	
	void OnTriggerStay(Collider other) {
		if (killCollisions) OnTriggerEnter(other);	
	}
}
