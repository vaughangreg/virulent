using UnityEngine;

public class ReportCollision : MonoBehaviour {
	void OnCollisionEnter(Collision c) {
		Debug.Log(gameObject.name + " collided with " + c.gameObject.name, c.gameObject);
	}
	
	void OnTriggerEnter(Collider c) {
		Debug.Log(gameObject.name + " collided with " + c.gameObject.name, c.gameObject);
	}
}
