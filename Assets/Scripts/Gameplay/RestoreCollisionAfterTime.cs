using UnityEngine;
using System.Collections;

public class RestoreCollisionAfterTime : MonoBehaviour {
	public Collider colliderToRestore;
	public float timeToWait;
	public int   layerToRestore;
	
	private float m_timeWaited;
	
	// Update is called once per frame
	void Update () {
		m_timeWaited += Time.deltaTime;
		if (m_timeWaited >= timeToWait) {
			///----
			//Get the Layer to change back to according to the object tag
			///----
			//!!!OVERRIDES THE ACTUAL LAYER TO RESTORE!!!
			if (Constants.TAG_COLLECTIONS[Constants.COLLECTION_VIRUS].Contains(gameObject.tag)) layerToRestore = LayerMask.NameToLayer("Default");
			else layerToRestore = LayerMask.NameToLayer("Enemies"); 
			gameObject.layer = layerToRestore;
			
			if (gameObject.GetComponent<Collider>() != null && colliderToRestore != null) {
				Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), colliderToRestore, false);
				//gameObject.layer = layerToRestore;  // Moved this down....
			} 
			Destroy(this);
		}
	}
	
	/// <summary>
	/// Call after attaching the component to set the collision ignoring and timing
	/// </summary>
	/// <param name="collider">
	/// Collider to ignore then restore
	/// </param>
	/// <param name="time">
	/// Time in seconds before restoration
	/// </param>
	public void Setup(Collider collider, float time, int toRestore) {
		colliderToRestore = collider;
		timeToWait        = time;
		layerToRestore    = toRestore;
		Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), colliderToRestore);
	}
	
	void OnDestroy() {
		//gameObject.layer = layerToRestore;
		if (Constants.TAG_COLLECTIONS[Constants.COLLECTION_VIRUS].Contains(gameObject.tag)) layerToRestore = LayerMask.NameToLayer("Default");
		else layerToRestore = LayerMask.NameToLayer("Enemies");
		gameObject.layer = layerToRestore;
		//Debug.Log(gameObject + "  " + gameObject.layer + "   " + LayerMask.LayerToName(gameObject.layer));
	}
}

