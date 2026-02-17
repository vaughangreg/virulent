using UnityEngine;
using System.Collections;

/// <summary>
/// Send a collision to your parents, you naughty boy!
/// </summary>
public class SendCollision : MonoBehaviour {
	public string methodToSend;
	
	void OnCollisionEnter(Collision other) {
		SendMessageUpwards(methodToSend, other);
	}
}
