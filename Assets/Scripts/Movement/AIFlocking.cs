using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class AIFlocking : MonoBehaviour {
	public GameObject targetObject;
	public string     targetTag;
	public float      maxVelocity = 45.0f;
	public float      distanceMax = 1500.0f;
	public float[]    flockWeights = new float[6] { 1.0f, 1.0f, 1.0f, 1.0f, 4.0f, 1.0f };
	
	protected List<Transform> m_obstacles = new List<Transform>();
	
	void Start() {
		Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
		AIFlockingManager.RegisterBoid(gameObject);
		targetObject = GameObject.FindGameObjectWithTag(targetTag);
	}
	
	void SetTarget(GameObject aTarget) {
		targetObject = aTarget;
	}
	
	void Update () {
		Vector2 randomDirectionPlane = Random.insideUnitCircle;
		Vector3 randomDirection = new Vector3(randomDirectionPlane.x, 0.0f, randomDirectionPlane.y) * maxVelocity;
		
		Vector3 deltaFlockCenter   = (AIFlockingManager.CenterForFlock(tag) - transform.position);
		Vector3 deltaFlockVelocity = (AIFlockingManager.VelocityForFlock(tag) - GetComponent<Rigidbody>().velocity);
		Vector3 deltaTarget        = (targetObject != null) 
										? (targetObject.transform.position - transform.position) 
										: Vector3.zero;
		if (deltaTarget.sqrMagnitude > distanceMax * distanceMax) {
			SendMessage(Combat.MESSAGE_DEATH, SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
			return;
		}
		
		Vector3 repulsiveCenter      = Vector3.zero;
		Vector3 repulsiveVelocity    = Vector3.zero;
		int     numRepulsiveVelocity = 0;
		
		if (m_obstacles.Count > 0) {
			List<Transform> toRemove = new List<Transform>();
			
			foreach (Transform aTransform in m_obstacles) {
				if (aTransform == null) {
					toRemove.Add(aTransform);
					break;
				}
				repulsiveCenter += aTransform.position;
				Rigidbody theRigidbody = aTransform.GetComponent<Rigidbody>();
				if (theRigidbody != null) {
					repulsiveVelocity += theRigidbody.velocity;
					++numRepulsiveVelocity;
				}	
			}
			
			repulsiveCenter   /= m_obstacles.Count;
			repulsiveVelocity /= Mathf.Max(1, numRepulsiveVelocity);
			foreach (Transform aTransform in toRemove) {
				m_obstacles.Remove(aTransform);	
			}
		}
		
		Vector3 force = (flockWeights[0] * deltaFlockCenter + flockWeights[1] *deltaFlockVelocity 
			+ flockWeights[2] * repulsiveCenter + flockWeights[3] * repulsiveVelocity) + flockWeights[4] * deltaTarget 
			+ flockWeights[5] * randomDirection;
		
//		Debug.Log(System.String.Format("BOID: dFC {0}\tdFV {1}\trV {2}\trC {3}\tdT {4}\trD {5}",
//		                               deltaFlockCenter, deltaFlockVelocity, repulsiveVelocity, repulsiveCenter,
//		                               deltaTarget, randomDirection), gameObject);
		
		GetComponent<Rigidbody>().velocity += force * Time.deltaTime;
		GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, maxVelocity);
	}
	
	void OnDestroy() {
		m_obstacles.Clear();	
	}
	
	public void OnCollisionEnter(Collision other) {
		if (other.collider.CompareTag(targetTag)) {
			targetObject = other.gameObject;	
		}
		else if (!other.collider.CompareTag(tag)) m_obstacles.Add(other.transform);	
	}
	
	public void OnCollisionExit(Collision other) {
		m_obstacles.Remove(other.transform);	
	}
	
	void _OnObjectDestroyed(MessageObjectDestroyed m) {
		if (targetObject == m.destroyedObject) targetObject = null;
		m_obstacles.Remove(m.destroyedObject.transform);
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().velocity);
	}
}
