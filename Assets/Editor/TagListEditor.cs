using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameTag))]
public class TagListEditor : Editor {
	
	GameTag m_t;
	public override void OnInspectorGUI ()
	{
		Debug.Log("hi");
		m_t = (GameTag)target;
		int index = 0;
		index = EditorGUILayout.Popup("Watched Tag", index, Constants.TAGS_LIST.ToArray());
		m_t.myTag = Constants.TAGS_LIST[index];
	}
	
}
