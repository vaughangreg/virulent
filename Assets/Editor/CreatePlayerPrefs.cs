using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreatePlayerPrefs : EditorWindow {
	int targetLevel = 0;
	
	[MenuItem("Virulent/Tools/CreateSomePlayerPrefs")] 
	static void Init() { 
		EditorWindow.GetWindow(typeof(CreatePlayerPrefs));
	} 
	
	void OnGUI () {
		targetLevel = EditorGUILayout.IntField("Level to create playerPrefs for:", targetLevel);
		if (GUILayout.Button("Create Prefs")) {
			PlayerPrefs.SetInt("Level " + (targetLevel) , 1);
			Debug.Log("player prefs created for level :" + targetLevel);
		}
		
	}
	
}
