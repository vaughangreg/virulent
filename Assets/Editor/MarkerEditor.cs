using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Marker))]
public class MarkerEditor : Editor {
	
	Marker marker;
	
	public override void OnInspectorGUI() {
		marker = (Marker)target;
		
		base.OnInspectorGUI ();
	}
	
	public void OnSceneGUI() {
		marker = (Marker)target;
		
		Handles.Label(new Vector3(marker.transform.position.x + 10, marker.transform.position.y, marker.transform.position.z + 10), marker.name);
	}
}
