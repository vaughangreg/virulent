using UnityEditor;
using UnityEngine;
using System.Collections;

public class DeletePlayerPrefs : MonoBehaviour {
	
	[MenuItem("Virulent/Tools/DeleteMyPlayerPrefs")] 
	static void DeleteMyPlayerPrefs() { 
		PlayerPrefs.DeleteAll();
	} 
}
