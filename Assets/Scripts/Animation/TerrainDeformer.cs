using UnityEngine;
using System.Collections;

/*
 * Component to procedurally animate border vertices of a terrain object
 * */
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("Virulent/Animation/Terrain Deformer")]
public class TerrainDeformer : MonoBehaviour
{
	// flag to specify whether debug should be drawn or not
	public bool isDebug = false;
	
	// the mesh filter component on the object
	public MeshFilter meshFilter;
	private Mesh _mesh;
	
	// parameters to tweak animation
	public float frequency = 1.5f;
	public float inverseWaveLength = 4f;
	public float amplitude = 0.01f;
	
	// vertex positions before any deformation is applied
	private Vector3[] _defaultVertexPositions;
	// vertex positions after deformation is applied
	private Vector3[] _newVertexPositions;
	
	// the border vertices that will deform
	public int[] borderVertices = new int[0];
	
	// normals corresponding to direction of movement for border vertices
	public Vector3[] borderNormals = new Vector3[0];
	
	// t-parameter values corresponding to border vertices for use in sine function
	public float[] tParameterValues = new float[0];
	
	/*
	 * Initialize
	 * */
	void Start ()
	{
		_mesh = meshFilter.mesh;
		_defaultVertexPositions = new Vector3[_mesh.vertexCount];
		_mesh.vertices.CopyTo(_defaultVertexPositions, 0);
		_newVertexPositions = new Vector3[_mesh.vertexCount];
		_mesh.vertices.CopyTo(_newVertexPositions, 0);
	}
	
	/// <summary>
	/// Resets all vertices.
	/// </summary>
	void OnDisable() {
		ResetVertices();	
	}
	
	/*
	 * Update the mesh
	 * */
	void Update ()
	{
		DeformVertices();
	}
	
	/*
	 * Deform mesh vertices using a sine function
	 * */
	void DeformVertices()
	{
		for (int i=0; i<borderVertices.Length; i++)
		{
			_newVertexPositions[borderVertices[i]] = _defaultVertexPositions[borderVertices[i]] + 
				Mathf.Sin((Time.time+tParameterValues[i]*inverseWaveLength)*frequency)*borderNormals[i]*amplitude;
		}
		meshFilter.mesh.vertices = _newVertexPositions;
	}
	
	/* Returns vertices to their default positions.
	 */
	void ResetVertices()
	{
		if (!gameObject.active) return;
		for (int i=0; i<borderVertices.Length; i++)
		{
			_newVertexPositions[borderVertices[i]] = _defaultVertexPositions[borderVertices[i]];
		}
		meshFilter.mesh.vertices = _newVertexPositions;
	}
	
	/*
	 * To be called from an editor script, finds all border vertices and sets up arrays on component
	 * */
	public void GetBorderVertices()
	{
		if (meshFilter==null) meshFilter = GetComponent<MeshFilter>();
		if (meshFilter==null) meshFilter = GetComponentInChildren<MeshFilter>();
		if (meshFilter==null) 
		{
			Debug.LogError(string.Format("Unable to find MeshFilter component on object {0}.", name), this);
			return;
		}
		Mesh m = meshFilter.sharedMesh;
		Vector3[] vertices = m.vertices;
		Vector3[] normals = m.normals;
		Vector2[] uv = m.uv;
		
		// find vertices on the side faces
		ArrayList sideVertices = new ArrayList();
		for (int i=0; i<normals.Length; i++)
			if (Vector3.Dot(normals[i], Vector3.up) < 0.99f) sideVertices.Add(i);
		
		// find border vertices doing a distance test with side-facing vertices
		Hashtable borderVerts = new Hashtable(); // key is point index, value is normal
		for (int i=0; i<vertices.Length; i++)
		{
			for (int j=0; j<sideVertices.Count; j++)
			{
				// skip if it is the same point
				if (j==i) continue;
				// skip if it does not pass threshold test
				if ((vertices[i]-vertices[(int)sideVertices[j]]).sqrMagnitude > 0.001f) continue;
				// skip if it is not a y-facing vertex
				if (Vector3.Dot(normals[i], Vector3.up) < 1f) continue;
				
				// add to the hashtable
				if (!borderVerts.ContainsKey(i))
					borderVerts.Add(i, normals[(int)sideVertices[j]]);
			}
		}
		
		// convert hashtable to arrays
		borderVertices = new int[borderVerts.Count];
		borderNormals = new Vector3[borderVertices.Length];
		tParameterValues = new float[borderVertices.Length];
		int[] keys = new int[borderVerts.Count];
		borderVerts.Keys.CopyTo(keys, 0);
		for (int i=0; i<borderVertices.Length; i++)
		{
			borderVertices[i] = keys[i];
			borderNormals[i] = (Vector3)borderVerts[borderVertices[i]];
			tParameterValues[i] = uv[borderVertices[i]].x*Mathf.PI;
		}
	}
}