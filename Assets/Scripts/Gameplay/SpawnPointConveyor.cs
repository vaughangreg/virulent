using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns an object and sends it towards a target point.
/// </summary>
public class SpawnPointConveyor : MonoBehaviour {
	public GameObject prefab;			// The object you want to be spawned
	public int        spawnedMax;		// The maximum of those objects you want on screen spawned from any spawnPoint
	public int        spawnedNum      = 0;
	public int        spawnedTotal    = 0;
	public int        spawnedMaxTotal = 0;
	public int        spawnRadius     = 0;
	public int        speedChange     = 0;
	
	public string watchedTag = "Capsid";
	public int    watchedWeight = 1;	// Weight each death of the watched tag by this amount.
	
	public Vector3    target;
	
	private Transform trans;
	
	void OnEnable() {
		Dispatcher.Listen(Dispatcher.OBJECT_CREATED, this);
	}
	
	void OnDisable() {
		//Dispatcher.StopListen(Dispatcher.OBJECT_DESTROYED, gameObject);	
	}
	
	void Awake() {
		trans = transform;
		
		//Force a zero Y position on awake
		Vector3 pos = trans.position;
		pos.y = 0;
		trans.position = pos;
		Spawn();
	}
		   
	/// <summary>
	/// If an object spawned by any spawnPoint is destroyed, remove it from the list
	/// </summary>
	void _OnObjectCreated(MessageObjectCreated m) {
		if (m.createdObject.CompareTag(watchedTag)) {
			spawnedNum = Mathf.Max(0, spawnedNum - 1 * watchedWeight);
			if (spawnedNum < spawnedMax) Spawn();
		}
	}
	
	/// <summary>
	/// Instantiates a new instance of the prefab and adds it to the list
	/// </summary>
	private void Spawn() {
		Vector3 offset = Random.insideUnitCircle * spawnRadius;
		offset.z = offset.y;
		offset.y = 0;
		
		GameObject product = (GameObject)Instantiate(prefab, trans.position + offset, trans.rotation);
		++spawnedNum;
		++spawnedTotal;
		
		MovementLerped lerp = product.GetComponent<MovementLerped>();
		if (lerp) {
			if (speedChange != 0) lerp.speed += speedChange;
			
			lerp.AddWorldPoint(trans.position);
			lerp.AddWorldPoint(target);
			lerp.CalculateSpeed();
		}
		
		if (spawnedTotal >= spawnedMaxTotal) Destroy(gameObject);
		if (spawnedNum < spawnedMax) Spawn();
	}
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(0.5f, 0.5f, 1, 0.75f);
		Gizmos.DrawWireSphere(transform.position, 25.0f);
	}
}
