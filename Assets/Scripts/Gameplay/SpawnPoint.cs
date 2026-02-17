using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script which instantiates a gameObject and semi-random intervals.
/// Multiple SpawnPoints can be used.
/// One SpawnPoint is selected randomly each time a new gameObject is spawned.
/// </summary>
public class SpawnPoint : MonoBehaviour {
	public GameObject 	prefab;	//the object you want to be spawned

	[SerializeField]
	public int 			maxNumActive;	//the maximum of those objects you want on screen spawned from any spawnPoint
	[SerializeField]
	public string       watchedPopulation;  //What population is watched for spawning
	[SerializeField]
	public float 		frequency;		//if maximum isn't met, how frequently should they be spawned
	public float 		variance;		//a random value between variance and zero is added to frequency
	public float        delay = 3.0f;
	
	private GameObject m_watchedPrefab;
	
	private static 	List<GameObject> m_spawnedObjects;	//contains the gameobjects created by any spawnPoint
	//private float 	m_timeOfNextSpawn;					//the time until another thing can be spawned by this spawnPoint
	
	private float   timeUntilNextSpawn = 0.0f;
	
	void OnEnable() {
		// NOTE: This causes problems when an object is destroyed
		// that sends an OBJECT_DESTROYED message. Need to update 
		// spawners to use a global pop count instead.
		// Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
		m_spawnedObjects = new List<GameObject>();
		
		timeUntilNextSpawn = delay;
	}
	
	void Awake() {
		//Force a zero Y position on awake
		Vector3 pos = gameObject.transform.position;
		pos.y = 0;
		gameObject.transform.position = pos;
	}
	
	void Update() {
		if (frequency > 0.0f)  {
			timeUntilNextSpawn -= Time.deltaTime;
			if (timeUntilNextSpawn <= 0) {
				//Use the specified ObjectManager count
				if (ObjectManager.instance.GetCount(watchedPopulation) < maxNumActive) Spawn();
				//set timeOfNextSpawn to a random value between frequency and frequency plus variance
				timeUntilNextSpawn = frequency + (variance * Random.value);
			}
		}
		else { 
			//Debug.LogError("Must enter a frequency for SpawnPoint!"); 
		}
	}
		   
	/// <summary>
	/// If an object spawned by any spawnPoint is destroyed, remove it from the list
	/// </summary>
	void _OnObjectDestroyed(MessageObjectDestroyed m) {
		//Manage personal spawn count
		GameObject goToDelete = null;
		foreach (GameObject go in m_spawnedObjects) {
			if (go == m.destroyedObject) {
				goToDelete = go;
			}
		}
		if(goToDelete != null) m_spawnedObjects.Remove(goToDelete);
	}
	
	/// <summary>
	/// Instantiates a new instance of the prefab and adds it to the list
	/// </summary>
	private void Spawn() {
		GameObject go = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
		m_spawnedObjects.Add(go); //Manage personal spawn count
	}
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(0.5f,0.5f,1,0.75f);
		Gizmos.DrawSphere(transform.position,10);
	}
}
