using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

//This editor creates some nice positionHandles for ease of positioning MoveObjectTo positions
[CustomEditor(typeof(InvaginationSpawner))]
public class InvaginationSpawnerEditor: Editor {
	
	protected InvaginationSpawner invSpwnr;
	
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
	}
	
	public void OnSceneGUI() {
		invSpwnr = (InvaginationSpawner)target;
		
		 for (int i = 0; i < invSpwnr.spawnLocations.Length; i ++) {
			try {
			invSpwnr.spawnLocations[i] = Handles.DoPositionHandle(invSpwnr.spawnLocations[i], Quaternion.identity);
			Handles.Label(invSpwnr.spawnLocations[i], i + ":" + invSpwnr.objectsToSpawn[i].name);
				
			}
			catch (IndexOutOfRangeException) {
				Debug.LogError("Invagination Spawner: Objects to Spawn and Spawn Locations must have an equal number of elements!");
			}
		}
    }
}
