using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(AIProducer))]
public class AIProducerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//show unitshells
		AIProducer aip = (AIProducer)target;
		
		EditorGUILayout.Space();
		GUILayout.Label("Units To Produce");
		GUILayout.Label("------------------");
		Array.Resize<AIProducer.UnitShell>(ref(aip.unitsToProduce), EditorGUILayout.IntField("# of UnitTypes", aip.unitsToProduce.Length));
		
		foreach(AIProducer.UnitShell u in aip.unitsToProduce)
		{
			if (u != null) {
				EditorGUI.indentLevel = 1;
				u.show = EditorGUILayout.Foldout(u.show, u.unitType.ToString());
				if (u.show) {
					EditorGUI.indentLevel = 2;
					u.unitType = EditorHelper.DisplayUnitEnum(u.unitType); //select unit type
					u.populationRestrictor = EditorHelper.DisplayPopulationGoal(u.populationRestrictor, 2, "Pop. Restrictor");
					EditorGUI.indentLevel = 2;
					Array.Resize<PopulationGoal>(ref(u.populationGoals), 
					                                   EditorGUILayout.IntField("# of Pop Goals", u.populationGoals.Length));
					for (int i = 0; i < u.populationGoals.Length; i++) {
						//Print each Population goal to the inspector
						u.populationGoals[i] = EditorHelper.DisplayPopulationGoal(u.populationGoals[i], 2);
					}
					EditorGUILayout.Space();
				}
			}
		}
	}
}

