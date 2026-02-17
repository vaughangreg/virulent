using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(Objective))]
public class ObjectiveEditor : Editor {
	
	public float numOfEnergyContainers;
	Objective m_objv;

	public override void OnInspectorGUI() {
		m_objv = (Objective)target;
		
		EditorGUILayout.Space();
		m_objv.objectiveType = (Objective.CheckType)EditorGUILayout.EnumPopup ("Objective Type", m_objv.objectiveType);
		
		switch (m_objv.objectiveType) {
		case Objective.CheckType.PopulationCount:
			DisplayPopCount();
			break;
		case Objective.CheckType.Collisions:
			DisplayCollisionCount ();
			break;
		case Objective.CheckType.TimeCounter:
			DisplayTimeCount();
			break;
		case Objective.CheckType.PopulationEquation:
			DisplayPopEquation();
			break;
		default:
			//base.OnInspectorGUI();
			break;
		}
		EditorGUILayout.Space();
		if (m_objv.objectiveType != Objective.CheckType.PopulationCount) base.OnInspectorGUI ();
	}

	void DisplayPopCount () {
		int size = EditorGUILayout.IntField("# of PopGoals", m_objv.quantity.Length);
		if (size != m_objv.quantity.Length) {
			Array.Resize<int>(ref m_objv.quantity, size);
			Array.Resize<string>(ref m_objv.watchedTag, size);
		}
		for (int i = 0; i < m_objv.watchedTag.Length; i++) {
			m_objv.watchedTag[i] = EditorGUILayout.TagField("Watched Tag", m_objv.watchedTag[i]);
			m_objv.quantity[i] = EditorGUILayout.IntField("Population Count", m_objv.quantity[i]);
			EditorGUILayout.Space();
		}
	}

	void DisplayCollisionCount () {
		
		//m_objv.watchedTag = EditorGUILayout.TextField ("Watched Tag", m_objv.watchedTag);
		m_objv.watchedTrigger = EditorGUILayout.ObjectField ("Trigger Object", m_objv.watchedTrigger, typeof(GameObject)) as GameObject;
		//m_objv.quantity = EditorGUILayout.IntField ("Quantity", m_objv.quantity);
		m_objv.achievementClip = EditorGUILayout.ObjectField("Achievement Clip", 
		                                                     m_objv.achievementClip, 
		                                                     typeof(AudioClip), null) as AudioClip;
	}
	void DisplayTimeCount() {
		//m_objv.quantity = EditorGUILayout.IntField("Seconds", m_objv.quantity);
	}
	
	void DisplayPopEquation() {
	
	}
}
