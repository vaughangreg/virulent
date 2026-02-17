using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AIMovable))]
public class AIMovableEditor : Editor
{
	void OnSceneGUI()
	{
		AIMovable aim = (AIMovable)target;
		//Draw the pNodes
		foreach(PNodes p in aim.patrolPath)
		{
			p.position = Handles.PositionHandle(p.position,Quaternion.identity);
			p.position.y = 0; //<--- Lock the y position
			//Neglect the rotation handle...
		}
	}
}

