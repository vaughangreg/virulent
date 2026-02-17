using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConveyorBelt))]
public class ConveyorBeltEditor : Editor {
	void OnSceneGUI() {
		ConveyorBelt belt = (ConveyorBelt)target;
		
		belt.targetPoint = Handles.PositionHandle(belt.targetPoint, Quaternion.identity);
		belt.targetPoint.y = 0;
		Handles.Label(belt.targetPoint, "Target");
	}
}
