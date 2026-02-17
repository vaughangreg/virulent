using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateBehaviorIfTriggered : MonoBehaviour {
	
	public MonoBehaviour someBehavior;
	public bool disableInsteadOfEnable = false;
	public bool useTagsToIdentify = false;
	public string[] tagsToIdentify;
	private List<GameObject> collidingObjects;
	public float delayBeforeDisabling = 0.5f;
	
	void Start() {
		collidingObjects = new List<GameObject>();
		Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
	}
	
	void OnTriggerEnter(Collider someCollider){
		if (someBehavior.enabled == !disableInsteadOfEnable) return;
		if (!useTagsToIdentify) {
			someBehavior.enabled = !disableInsteadOfEnable;
			collidingObjects.Add(someCollider.gameObject);
		} else {
			foreach (string i in tagsToIdentify) {
				if (someCollider.gameObject.CompareTag(i)) {
					someBehavior.enabled = !disableInsteadOfEnable;
					if (collidingObjects.IndexOf(someCollider.gameObject) == -1) collidingObjects.Add(someCollider.gameObject);
					//Debug.Log("Enabling Script");
				}
			}
		}
		
	}
	
	void OnTriggerStay(Collider someCollider){
		if (!someBehavior.enabled) OnTriggerEnter(someCollider);
	}
	
	void OnTriggerExit(Collider someCollider) {
		if (someBehavior.enabled == disableInsteadOfEnable) return;
		if (!useTagsToIdentify) {
			someBehavior.enabled = disableInsteadOfEnable;
			collidingObjects.Remove(someCollider.gameObject);
		}
		else {
			foreach (string i in tagsToIdentify) {
				if (someCollider.gameObject.CompareTag(i)) {
					collidingObjects.Remove(someCollider.gameObject);
					if (collidingObjects.Count == 0) Invoke("DisableBehavior", delayBeforeDisabling);
					//Debug.Log("Disabling Script");
					//Debug.Log(collidingObjects.Count);
				}
			}
		}
		
	}
	
	void _OnObjectDestroyed(MessageObjectDestroyed m ) {
		//Debug.Log(m.destroyedObject);
		collidingObjects.Remove(m.destroyedObject);
		if (collidingObjects.Count == 0) {
			Invoke("DisableBehavior", delayBeforeDisabling);
			//Debug.Log("Disabling Script");
		}
		//Debug.Log(collidingObjects.Count);
	}
	
	void DisableBehavior() {
		someBehavior.enabled = disableInsteadOfEnable;
	}
}
