using UnityEditor;
using UnityEngine;
using System.Collections;

public class ActivateDeactivateEditor : MonoBehaviour {
	
	[MenuItem("Virulent/Tools/Activate_Deactivate")] 
	static void ActivateDeactivate() { 
		foreach (Object anObject in Selection.objects) {
			if (anObject.GetType() == typeof(GameObject))
			{
				GameObject tempObj = (GameObject)anObject;
				tempObj.active = !tempObj.active;
			}
		} 
	}
	 
}
