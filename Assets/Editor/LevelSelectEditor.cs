

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelSelect))]
public class LevelSelectEditor : Editor {
/*	
	LevelSelect levelSelect;
	
	public override void OnInspectorGUI() {
		levelSelect = (LevelSelect)target;
		
		base.OnInspectorGUI ();
	}
	
	public void OnSceneGUI() {
		levelSelect = (LevelSelect)target;
		if (levelSelect.buttons != null) {
			for (int i = 0; i < levelSelect.buttons.Length; i++) {
				if (levelSelect.buttons[i] != null) {
					levelSelect.buttons[i].transform.position = levelSelect.BUTTON_POSITIONS[i];
				}
			}
		}
	}
	*/
}
