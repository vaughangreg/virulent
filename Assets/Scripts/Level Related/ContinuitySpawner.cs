using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds the position and orientation of a spawn location set in editor
/// </summary>
[System.Serializable]
public class SpawnZone
{
	public Vector3 position = Vector3.zero;
	[HideInInspector]public Quaternion orientation = Quaternion.Euler(new Vector3(0,30,0));
	public float yRotation = 30.0f;

}

public class ContinuitySpawner : MonoBehaviour
{
	
	public List<SpawnZone> spawnZones = new List<SpawnZone>();
	public bool createSecondaryObjects = false;
	public List<SpawnZone> secondarySpawnZones = new List<SpawnZone>();
	
	public void SpawnObjects(GameObject type, int amount, GameObject secondaryType, int secondaryAmount)
	{
		//Cap the amount to the number of spawn zones we have
		amount = Mathf.Clamp(amount,0,spawnZones.Count);
		for (int i = 0; i < amount; i++)
		{
			Instantiate(type, spawnZones[i].position, spawnZones[i].orientation);
		}
		secondaryAmount = Mathf.Clamp(secondaryAmount,0,secondarySpawnZones.Count);
		if (createSecondaryObjects) {
			for (int i = 0; i < secondaryAmount; i++)
			{
				Instantiate(secondaryType, secondarySpawnZones[i].position, secondarySpawnZones[i].orientation);
			}
		}
	}
	
	void OnDrawGizmos()
	{
		foreach(SpawnZone sz in spawnZones)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(sz.position,5.0f);
		}
		
		if (createSecondaryObjects) {
			foreach(SpawnZone sz in secondarySpawnZones)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(sz.position,5.0f);
			}
		}
		
	}
}

