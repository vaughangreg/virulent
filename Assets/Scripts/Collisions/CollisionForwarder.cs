using UnityEngine;

public class CollisionForwarder : MonoBehaviour {
	public Combat targetCombat;
	
	void OnCollisionEnter(Collision collision) {
		targetCombat.OnCollisionEnter(collision);
	}
}
