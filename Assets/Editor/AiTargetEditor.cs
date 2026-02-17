using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AiTargets))]
public class AiTargetEditor : Editor {
	
	AiTargets m_at;
	
	public void OnSceneGUI() {
		m_at = (AiTargets)target ;
		for (int i = 0; i < m_at.myAiTargets.Length; i++) {
			Handles.color = Color.green;
			m_at.myAiTargets[i] = Handles.PositionHandle(m_at.myAiTargets[i], Quaternion.Euler(0,0,0)); 
		} 
		
	}
	
	
}
