using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class AIFlockingSpacer : MonoBehaviour {
	protected AIFlocking m_parent;
	
	void Awake() {
		m_parent = transform.parent.GetComponent<AIFlocking>();
	}	
	
	void OnCollisionEnter(Collision other) {
		m_parent.OnCollisionEnter(other);
	}
	
	void OnCollisionExit(Collision other) {
		m_parent.OnCollisionExit(other);
	}
}
