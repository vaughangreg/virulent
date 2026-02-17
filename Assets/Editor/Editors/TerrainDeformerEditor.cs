using UnityEditor;
using UnityEngine;
using System.Collections;

/*
 * Editor scripts for setting up TerrainDeformer component
 * */
public class TerrainDeformerEditorScripts : ScriptableObject
{
	/*
	 * Set up any TerrainDeformer objects in the selection
	 * */
	[MenuItem("Virulent/Animation/Set Up Terrain Deformers")]
	public static void SetupTerrainDeformers()
	{
		foreach (GameObject go in Selection.gameObjects)
		{
			TerrainDeformer td = go.GetComponent<TerrainDeformer>();
			if (td==null) continue;
			td.GetBorderVertices();
		}
	}
}

/*
 * Custom editor for TerrainDeformer to verify results
 * */
[CustomEditor(typeof(TerrainDeformer))]
public class TerrainDeformerEditor : Editor
{
	static TerrainDeformer _td = null;
	
	/*
	 * Set up selection
	 * */
	void OnEnable()
	{
		_td = (TerrainDeformer) target;
	}
	
	/*
	 * Render debug
	 * */
	void OnSceneGUI()
	{
		if (!_td.isDebug || _td.meshFilter==null) return;
		
		Handles.color = Color.magenta;
		for (int i=0; i<_td.borderVertices.Length; i++)
		{
			int vertex = _td.borderVertices[i];
			Handles.DrawLine(
				_td.transform.TransformPoint(_td.meshFilter.sharedMesh.vertices[vertex]),
				_td.transform.TransformPoint(_td.meshFilter.sharedMesh.vertices[vertex] + _td.transform.TransformDirection(_td.borderNormals[i])*_td.tParameterValues[i])
			);
		}
	}
}