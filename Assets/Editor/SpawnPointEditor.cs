using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnPoint))]
public class SpawnPointEditor : Editor
{
	SpawnPoint m_sp;
	public override void OnInspectorGUI ()
	{
		m_sp = (SpawnPoint)target;
		m_sp.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn",m_sp.prefab, typeof(GameObject));
		//m_sp.watchesPopulation = EditorGUILayout.Toggle("Watch Global Pop.",m_sp.watchesPopulation);
		/*if (m_sp.watchesPopulation) { //Do popup */
		int index = 0;
		if (Constants.TAGS_LIST.Contains(m_sp.watchedPopulation)) {
			index = Constants.TAGS_LIST.IndexOf(m_sp.watchedPopulation);
		}
		index = EditorGUILayout.Popup("Watched Pop.", index, Constants.TAGS_LIST.ToArray());
		m_sp.watchedPopulation = Constants.TAGS_LIST[index];
		/*
		else {
			EditorGUILayout.LabelField("Watched Pop.", "Local Population");	
		}*/
		m_sp.maxNumActive = EditorGUILayout.IntField("Max # Active", m_sp.maxNumActive);
		m_sp.frequency = EditorGUILayout.FloatField("Frequency", m_sp.frequency);
		m_sp.variance = EditorGUILayout.FloatField("Variance", m_sp.variance);
		m_sp.delay = EditorGUILayout.FloatField("Delay",m_sp.delay);
		//base.OnInspectorGUI();
	}
	
	
	public void OnSceneGUI() {
		m_sp = (SpawnPoint)target;
		Handles.color = Color.green;
		Handles.DrawSolidDisc(m_sp.transform.position, Vector3.up, 15); 
	}
}
