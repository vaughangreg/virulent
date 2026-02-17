using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(CheckPointEvent))]
public class CheckPointEventEditor : Editor {
	private CheckPointEvent cpe;
	
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		//Show Population count values when needed
		cpe = (CheckPointEvent)target;
		if (cpe.messageToCauseTrigger == CheckPointEvent.eventTypes.population) {
			EditorGUILayout.Separator();
			
			EditorGUI.indentLevel = 1;
			int count = EditorGUILayout.IntField("# of Pop. Goals", cpe.populationGoals.Count);
			if (count != cpe.populationGoals.Count) cpe.populationGoals = AdjustListCount(cpe.populationGoals, count);
			
			EditorGUI.indentLevel = 2;
			for (int i = 0; i < cpe.populationGoals.Count; i++) {
				//Print each Population goal to the inspector
				EditorGUILayout.Space();
				cpe.populationGoals[i] = EditorHelper.DisplayPopulationGoal(cpe.populationGoals[i], 2);
			}
		}
	}
	
	void OnSceneGUI() {
		cpe = (CheckPointEvent)target;
		if (cpe.cameraOperateFocus)
		{
			cpe.focusMessage.targetPosition = Handles.PositionHandle(cpe.focusMessage.targetPosition, Quaternion.identity);
			Handles.Label(cpe.focusMessage.targetPosition, "Target Position");
			
			Vector3   position = cpe.focusMessage.targetPosition;
			float     ortho    = cpe.focusMessage.zoomOrthographicSize;
			float     aspect   = 1.33f * ortho;
			
			Vector3[] zoomBounds = new Vector3[4];
			zoomBounds[0] = new Vector3(position.x - aspect, position.y, position.z - ortho);
			zoomBounds[1] = new Vector3(position.x - aspect, position.y, position.z + ortho);
			zoomBounds[2] = new Vector3(position.x + aspect, position.y, position.z + ortho);
			zoomBounds[3] = new Vector3(position.x + aspect, position.y, position.z - ortho);
			
			Handles.DrawSolidRectangleWithOutline(zoomBounds, new Color(1.0f, 1.0f, 1.0f, 0.2f), Color.black);
			foreach (Vector3 aVertex in zoomBounds) {
				cpe.focusMessage.zoomOrthographicSize = Handles.ScaleValueHandle(cpe.focusMessage.zoomOrthographicSize, aVertex,
				                                                                 Quaternion.identity, HandleUtility.GetHandleSize(aVertex),
				                                                                 Handles.CubeCap, 1.0f);
			}
		}
	}
	
	public List<PopulationGoal> AdjustListCount(List<PopulationGoal> goalList, int newLength) {
		while(goalList.Count < newLength) { 
			goalList.Add(new PopulationGoal());
		}
		if (goalList.Count > newLength) {
			goalList.RemoveRange(newLength, goalList.Count - newLength);
		}
		return goalList;
	}
}

