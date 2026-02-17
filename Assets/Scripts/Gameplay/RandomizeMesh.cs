using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Allows you to set a MeshFilter's mesh to a random mesh from a list
/// </summary>
[RequireComponent(typeof(AudioSet))]
public class RandomizeMesh : MonoBehaviour {
	public bool changesTags = true;
	
	public int        numberOfElements = 0;
	public Mesh[]     meshList;
	public Material[] materialsList;
	public bool       tagExpand = true;
	public string[]   tagList;
	
	public bool expandTag        = false;
	public bool expandMesh       = false;
	public bool expandMaterial   = false;
	public bool expandSelections = false;
	
	public AudioSet audioSet;
	public Selectable selector;
	
	private Mesh m_mesh;
	
	/// <summary>
	/// Check the length and whether the arrays are null...
	/// </summary>
	void Awake()
	{
		if (meshList.Length != materialsList.Length) {
			Debug.LogError("MeshList and MaterialsList do not have the same amount of elements",gameObject);
		}
		if (meshList.Length < 1) Debug.LogError("MeshList has no values.",gameObject);
		if (materialsList.Length < 1) Debug.LogError("MaterialsList has no values.",gameObject);
		if (selector.selectionClips.Length != 1) Debug.LogError("Selection clips should be one.", gameObject);
	}
	
	/// <summary>
	/// Picks a mesh from the array 'meshList' and sets given MeshFilter's mesh to it
	/// </summary>
	/// <param name="meshFilter">
	/// MeshFilter to change
	/// </param>
	/// <returns>
	/// The mesh that was chosen
	/// </returns>
	public Mesh Randomize(MeshFilter meshFilter)
	{
		//Pick a random mesh from the list, set it to the given filter, and return it
		int rand = Mathf.FloorToInt(Random.Range(0.0f,meshList.Length));
		if (meshList[rand] != null) {
			m_mesh = meshList[rand];
			selector.selectionClips[0] = audioSet.RandomClip(rand);
			meshFilter.sharedMesh = m_mesh;
			return m_mesh;
		}
		else {
			Debug.LogError("Mesh List has an empty element.", gameObject);
			return null;
		}
	}
	/// <summary>
	/// Picks a mesh from the array 'meshList' and sets given MeshFilter's mesh to it. 
	/// Also swaps corresponding material of given renderer.
	/// </summary>
	/// <param name="go">
	/// GameObject to change tags of if 'changeTags' is true
	/// </para>
	/// <param name="meshFilter">
	/// MeshFilter to change mesh of
	/// </param>
	/// <param name="meshRenderer">
	/// MeshRender to change material of
	/// </param>
	/// <returns>
	/// New mesh used
	/// </returns>
	public Mesh Randomize(GameObject go, MeshFilter meshFilter, MeshRenderer meshRenderer)
	{
		//Pick a random mesh from the list, set it to the given filter, and return it
		int rand = Mathf.FloorToInt(Random.Range(0.0f,meshList.Length));
		if (meshList[rand] != null && materialsList[rand] != null) {
			m_mesh = meshList[rand];
			selector.selectionClips[0] = audioSet.RandomClip(rand);
			meshFilter.sharedMesh = m_mesh;
			meshRenderer.sharedMaterial = materialsList[rand];
			if (changesTags)
			{	
				if (tagList[rand] == null) Debug.LogError("You have not set tags to change to but are requesting them.",go);
				else go.tag = tagList[rand];
			}
			return m_mesh;
		}
		else {
			Debug.LogError("MeshList or MaterialsList has an empty element.", gameObject);
			return null;
		}
	}
}
