using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//[CustomEditor (typeof (EventList))]
public class CheckPointViewer : Editor
{
	public override void OnInspectorGUI() {
		 
		//EditorGUIUtility.LookLikeControls();
		//EventList j = (EventList)target;
		
		//foreach (CheckPoint j in myCheckPoint) {
			//EditorGUI.indentLevel = 5;
			
			//j.MessageType = EditorGUILayout.TextField("MessageType", j.MessageType );
			//j.Source = EditorGUILayout.TextField("Source", j.Source );
			//EditorGUILayout.Separator();
		//}
		
		//myCheckPoint.Activate = EditorGUILayout.ObjectField("GameObj To Activate", myCheckPoint.Activate, typeof(GameObject) );
		//DrawDefaultInspector();
		
		//EditorGUIUtility.LookLikeInspector(); 
	}
    
}






/*
 * [CustomEditor (typeof (Test))]
public class TestEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Test script = (Test)target;
		
		List<ValueHolder> tempList = new List<ValueHolder>();
		
		int counter = 0;
		
		foreach(ValueHolder i in script.valueHolders)
		{	
			//EditorGUILayout.ObjectField("GameObjects", i, typeof (GameObject));
			
			EditorGUILayout.BeginVertical();
			EditorGUILayout.PrefixLabel("["+counter+"]-"+i.gameObject.name);
			if (i != null)
			{
				i.TestInt = EditorGUILayout.IntField("Test Integer",i.TestInt);
				i.TestFloat = EditorGUILayout.FloatField("Test Float",i.TestFloat);
				i.TestBool = EditorGUILayout.Toggle("Test Bool",i.TestBool);
				i.TestVector = EditorGUILayout.Vector3Field("Test Vector3",i.TestVector);
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
			
			counter ++;
		}
		
		DrawDefaultInspector();
	}
}
*/

/*[MenuItem ("Custom/Select Main Camera ^c")]
    static void Select()
    {
        if (Camera.main != null)
            Selection.activeObject = Camera.main.gameObject;
    }
    */