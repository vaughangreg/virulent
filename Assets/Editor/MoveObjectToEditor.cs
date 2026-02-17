using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

//This editor creates some nice positionHandles for ease of positioning MoveObjectTo positions
[CustomEditor(typeof(MoveObjectTo))]
public class MoveObjectToEditor: Editor {
	
	private MoveObjectTo mot;
	private float handleSize = 25;
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		mot = (MoveObjectTo)target;
		//Handle custom variables
		if (mot.destroyOnCompletion) {
			EditorGUI.indentLevel = 2;
			mot.isDisposableParent = EditorGUILayout.Toggle("Is Disposable Parent", mot.isDisposableParent);
			EditorGUI.indentLevel = 0;
		}
		
		//Draw the PathNode list
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Path Nodes", "");
		int size = EditorGUILayout.IntField("# of PathNodes", mot.pathNodes.Length);
		if (size != mot.pathNodes.Length) Array.Resize<MoveObjectTo.Node>(ref mot.pathNodes, size);
		int index = 0;
		for (int i = 0; i < mot.pathNodes.Length; i++) {
			MoveObjectTo.Node n = mot.pathNodes[i];
			if (n != null) {
				EditorGUI.indentLevel = 1;
				if (n.isOpen = EditorGUILayout.Foldout(n.isOpen, "Node [" + index + "]")) {
					EditorGUI.indentLevel = 3;
					if (mot.useNodeRotations) n.useDirection = EditorGUILayout.Toggle("Use Direction", n.useDirection);
					n.waitHere = EditorGUILayout.Toggle("Wait Here", n.waitHere);
				}
				index++;
			}
			else {
				mot.pathNodes[i] = new MoveObjectTo.Node();
				if (mot.pathNodes.Length > 0) {
					mot.pathNodes[i].position.x = mot.pathNodes[i-1].position.x + 40;
				}
			}
		}
		if (GUI.changed) EditorUtility.SetDirty(target);
	}
	
	public void OnSceneGUI()
	{
		mot = (MoveObjectTo)target;
		foreach(MoveObjectTo.Node n in mot.pathNodes) {
			Handles.color = Color.white;
			n.position = Handles.PositionHandle(n.position, Quaternion.identity);
			n.position.y = mot.gameObject.transform.position.y;
			//Only draw Rotation handles if option is set
			if (mot.useNodeRotations && n.useDirection) {
				n.direction = Handles.Disc(Quaternion.Euler(0,n.direction,0), n.position, Vector3.up, handleSize, false, 1).eulerAngles.y;
				Handles.color = new Color(1,1,0);
				Handles.ArrowCap(8,n.position,Quaternion.Euler(0,n.direction,0),handleSize-3);
			}
		}
	}
	
}
