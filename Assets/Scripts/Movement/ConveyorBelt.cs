using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ConveyorBelt : MonoBehaviour {
	public Vector3 targetPoint;
	
	protected Transform trans;
	
	private List<MovementLerped> m_itemsConveyed;
	
	void Start() { 
		trans = transform;
		m_itemsConveyed = new List<MovementLerped>();
	}
	
	void OnTriggerEnter(Collider other) {
		Movable mover = other.gameObject.GetComponent<Movable>();
		if (!mover) return;
		
		MovementLerped lerped = other.gameObject.GetComponent<MovementLerped>();
		Vector3 sourcePoint = trans.position;
		sourcePoint.y = 0;
		lerped.ClearPath();
		lerped.AddWorldPoint(other.transform.position); //sourcePoint);
		lerped.AddWorldPoint(targetPoint);
		lerped.CalculateSpeed();
		m_itemsConveyed.Add(lerped);
		
		Selectable selector   = other.gameObject.GetComponent<Selectable>(); 
		Camera.main.GetComponent<InputManager>().ClearIfSelected(other.gameObject);
		selector.isSelectable = false;
		Physics.IgnoreCollision(other, GetComponent<Collider>());
		
		StartCoroutine(ReenableSelection(lerped, selector, other, lerped.timeScale));
	}
	
	System.Collections.IEnumerator ReenableSelection(MovementLerped aLerper, Selectable aSelector, 
	                                                 Collider aCollider, float aDuration) 
	{
		while (aDuration >= 0.0f) {
			aDuration -= Time.deltaTime;
			//Debug.Log(aDuration);
			yield return null;
		}
		if (aLerper != null) {
			m_itemsConveyed.Remove(aLerper);
			aLerper.ClearPath();
			aSelector.isSelectable = true;
			Physics.IgnoreCollision(aCollider, GetComponent<Collider>(), false);
		}
	}
	
	void OnDisable() {
		OnDestroy();
	}
	
	void OnDestroy() {
		foreach (MovementLerped i in m_itemsConveyed) {
			if (i != null) {
				i.ClearPath();
				i.gameObject.GetComponent<Selectable>().enabled = true;
				i.gameObject.GetComponent<Selectable>().isSelectable = true;
			}
		}
	}
}
