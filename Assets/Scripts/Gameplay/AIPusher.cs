using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPusher : MonoBehaviour {
	
	public float forceStrength = 10f;
	public string[] tagsToPush;
	public GameObject[] objectsToPush;
	public bool killOnExit = false;
	
	private List<GameObject> m_objects;
	
	void Start() {
		m_objects = new List<GameObject>();
	}
	
	void OnTriggerEnter(Collider c) { 
		foreach(GameObject g in objectsToPush) {
			if (c.gameObject == g) m_objects.Add(c.gameObject);
		}
		foreach(string s in tagsToPush) {
			if (c.gameObject.tag == s) m_objects.Add(c.gameObject);
		}
	}
	
	void OnTriggerExit(Collider c) { 
		foreach(GameObject g in objectsToPush) {
			if (c.gameObject == g) {
				m_objects.Remove(c.gameObject);
				if (killOnExit) Destroy(c.gameObject, 3);
			}
		}
		foreach(string s in tagsToPush) {
			if (c.gameObject.tag == s) {
				m_objects.Remove(c.gameObject);
				if (killOnExit) Destroy(c.gameObject, 3);
			}
		}
	}
	
	void Update() {
		//Debug.Log(m_objects.Count);
		for(int i = 0; i < m_objects.Count; i++) {
			//Debug.Log(m_objects.Count);
			if (!m_objects[i]) {
				m_objects.RemoveAt(i);
				continue;
			}
			if (m_objects[i].GetComponent<Rigidbody>()) m_objects[i].GetComponent<Rigidbody>().AddForce(transform.forward * forceStrength * Time.deltaTime);
		}
	}
}
