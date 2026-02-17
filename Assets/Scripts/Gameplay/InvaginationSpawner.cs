using UnityEngine;
using System.Collections;

public class InvaginationSpawner : MonoBehaviour {
	
	public GameObject[] 	objectsToSpawn;
	public Vector3[]		spawnLocations;
	
	void Start() {
		Dispatcher.Listen(Dispatcher.INVAGINATION_COMPLETE, gameObject);
		//SpawnObjects();
	}
	
	void _OnInvaginationComplete(MessageInvaginationComplete m) {
		//Debug.Log("MSG RECIEVED");
		SpawnObjects();
	}
	
	private Quaternion RandomAngle() {
		return Quaternion.identity;
		//return Quaternion.Euler(new Vector3(0.0f, Random.Range(0, 360), 0.0f));
		//return Quaternion.identity;
	}
	
	private void SpawnObjects() {
		for(int i = 0; i < objectsToSpawn.Length; i++){
			//Debug.Log("Spawning object: " + i);
			Instantiate(objectsToSpawn[i], spawnLocations[i], Quaternion.Euler(0f,0f,0f)); //RandomAngle());
		}
	}
}
