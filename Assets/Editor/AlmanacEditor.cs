using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Almanac))]
public class AlmanacEditor : Editor
{
	private Almanac alm;
	
	public override void OnInspectorGUI()
	{
		//Do initial stuffs
		base.OnInspectorGUI();
		//Display the almanac items
		alm = (Almanac)target;
		GUILayout.Label("-------------------------");
		GUILayout.Label("Almanac Items");
		//TEAM VIRUS ITEMS
		alm.showVirus = EditorGUILayout.Foldout(alm.showVirus, "Virus Items");
		if (alm.showVirus) alm.teamVirusItems = DisplayList(alm.teamVirusItems);
		//TEAM CELL ITEMS
		alm.showCell = EditorGUILayout.Foldout(alm.showCell, "Cell Items");
		if (alm.showCell) alm.teamCellItems = DisplayList(alm.teamCellItems);
		//TEAM OTHER ITEMS
		alm.showOther = EditorGUILayout.Foldout(alm.showOther, "Other Items");
		if (alm.showOther) alm.otherItems = DisplayList(alm.otherItems);
		EditorGUILayout.Space();
	}
	
	List<AlmanacItem> DisplayList(List<AlmanacItem> list)
	{
		AlmanacItem[] array = list.ToArray();
		
		EditorGUI.indentLevel = 2;
		int size = EditorGUILayout.IntSlider("# of items",array.Length,0,12);
		if (size != array.Length) Array.Resize<AlmanacItem>(ref array, size);
		
		foreach(AlmanacItem a in array)
		{
			if (a != null) {
			EditorGUI.indentLevel = 2;
			a.open = EditorGUILayout.Foldout(a.open,a.label);
			if (a.open) {
				EditorGUI.indentLevel = 3;
				a.label = EditorGUILayout.TextField("Label",a.label);
				a.image = EditorGUILayout.ObjectField("Image", a.image,typeof(Texture2D)) as Texture2D;
				a.description = EditorGUILayout.TextField("Description", a.description);
				//-----Unit Tags------
				a.showTags = EditorGUILayout.Foldout(a.showTags, "Unit Tags");
				if (a.showTags) {
					EditorGUI.indentLevel = 4;
					size = EditorGUILayout.IntField("# of Unit Tags", a.unitTag.Length);
					if (size != a.unitTag.Length) Array.Resize<string>(ref a.unitTag, size);
					int count = 0;
					foreach(string s in  a.unitTag) {
						a.unitTag[count] = EditorGUILayout.TagField("Tag[" + count + "]",s);
						count ++;
					}
					EditorGUILayout.Space();
				}
			}
			}
		}
		EditorGUI.indentLevel = 1;
		List<AlmanacItem> temp = new List<AlmanacItem>();
		temp.AddRange(array);
		return temp;
	}
}

