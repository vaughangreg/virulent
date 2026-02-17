using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnPointConveyor))]
public class SpawnPointConveyorEditor : Editor {
	void OnSceneGUI() {
		SpawnPointConveyor belt = (SpawnPointConveyor)target;
		
		belt.target = Handles.PositionHandle(belt.target, Quaternion.identity);
		belt.target.y = 0;
		Handles.Label(belt.target, "Target");
	}
}
