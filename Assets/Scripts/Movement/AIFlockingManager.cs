using UnityEngine;
using System.Collections.Generic;

public class AIFlockingManager : MonoBehaviour {
	protected static Dictionary<string, List<GameObject>> m_flocks     = new Dictionary<string, List<GameObject>>();
	protected static Dictionary<string, Vector3>          m_centers    = new Dictionary<string, Vector3>();
	protected static Dictionary<string, Vector3>          m_velocities = new Dictionary<string, Vector3>();
	
	public static void RegisterBoid(GameObject aBoid) {
		if (!m_flocks.ContainsKey(aBoid.tag)) {
			m_flocks[aBoid.tag] = new List<GameObject>();
			m_centers[aBoid.tag] = Vector3.zero;
			m_velocities[aBoid.tag] = Vector3.zero;
		}
	
		List<GameObject> flock = m_flocks[aBoid.tag];
		if (!flock.Contains(aBoid)) flock.Add(aBoid);
	}
	
	public static Vector3 CenterForFlock(string aTag) {
		if (m_centers.ContainsKey(aTag)) {
			return m_centers[aTag];
		}
		Debug.LogWarning("No center found for " + aTag);
		return Vector3.zero;
	}
	
	public static Vector3 VelocityForFlock(string aTag) {
		if (m_velocities.ContainsKey(aTag)) {
			return m_velocities[aTag];
		}
		Debug.LogWarning("No velocity found for " + aTag);
		return Vector3.zero;
	}
	
	void Start() {
		Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);	
	}
	
	void Update() {
		foreach (KeyValuePair<string, List<GameObject>> aFlock in m_flocks) {
			Vector3 center   = Vector3.zero;
			Vector3 velocity = Vector3.zero;
		
			foreach (GameObject aBoid in aFlock.Value) {
				center   += aBoid.transform.position;
				velocity += aBoid.GetComponent<Rigidbody>().velocity;
			}
			center   /= aFlock.Value.Count;
			velocity /= aFlock.Value.Count;
			
			// Debug.Log(aFlock.Key + " Center: " + center + "\tVelocity: " + velocity);
			m_centers[aFlock.Key]    = center;
			m_velocities[aFlock.Key] = velocity;
		}
	}
	
	void _OnObjectDestroyed(MessageObjectDestroyed m) {
		if (m_flocks.ContainsKey(m.destroyedObject.tag)) {
			m_flocks[m.destroyedObject.tag].Remove(m.destroyedObject);
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		foreach (KeyValuePair<string, List<GameObject>> aFlock in m_flocks) {
			Vector3 center   = m_centers[aFlock.Key];
			Vector3 velocity = m_velocities[aFlock.Key];
			
			Gizmos.DrawWireSphere(center, 10.0f);
			Gizmos.DrawLine(center, center + velocity);
		}
	}
}
