using UnityEngine;
using System.Collections;

public class LocalMovementLerp : MonoBehaviour {
	
	private Vector3 myOriginalPosition;
	private Vector3 myTarget;
	private float moveTime;
	private bool setPosition = false; 
	private float m_counter = 0f;
	
	public void SetTarget(Vector3 targetPos, float timeToLerp) {
		myOriginalPosition = transform.localPosition;
		myTarget = targetPos;
		moveTime = timeToLerp;
		setPosition = true;
	}
	
	void Update() {
		if (!setPosition) return;
		m_counter += Time.deltaTime;
		transform.localPosition = Vector3.Lerp(myOriginalPosition, myTarget, m_counter/moveTime);
	}
	
}
