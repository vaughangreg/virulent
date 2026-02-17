using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomizeMesh))]
public class RandomizeMeshEditor : Editor {
	
	private RandomizeMesh rand;
	private enum ArrayType { Tags, Meshes, Materials }
	
	public override void OnInspectorGUI()
	{
		rand = (RandomizeMesh)target;
		
		EditorGUI.indentLevel = 1;
		rand.changesTags = EditorGUILayout.Toggle("Changes Tags",rand.changesTags);
		
		int oldSize = rand.numberOfElements;
		rand.numberOfElements = EditorGUILayout.IntField("# of Elements",rand.numberOfElements);
		
		if (oldSize != rand.numberOfElements)
		{
			ResetArraySizes();
		}
		rand.expandMesh = DrawArray("Meshes",rand.expandMesh,ArrayType.Meshes);
		rand.expandMaterial = DrawArray("Materials",rand.expandMaterial,ArrayType.Materials);
		if (rand.changesTags) rand.expandTag = DrawArray("Tags",rand.expandTag,ArrayType.Tags);
		
		rand.selector = (Selectable)EditorGUILayout.ObjectField("Selectable", rand.selector, typeof(Selectable));
		rand.audioSet = (AudioSet)EditorGUILayout.ObjectField("Audio Set", rand.audioSet, typeof(AudioSet));
		EditorGUILayout.Separator();
	}
	
	private bool DrawArray(string label, bool isExpanded, ArrayType arrayType)
	{
		EditorGUI.indentLevel = 0;
		isExpanded = EditorGUILayout.Foldout(isExpanded,label);
		if (isExpanded)
		{
			EditorGUI.indentLevel = 3;
			//Allow the proper values to be adjusted
			for(int i = 0; i < rand.numberOfElements; i++)
			{
				switch (arrayType) {
					case RandomizeMeshEditor.ArrayType.Tags:
						rand.tagList[i] = EditorGUILayout.TagField("Element "+i,rand.tagList[i]);
						break;
					case RandomizeMeshEditor.ArrayType.Meshes:
						rand.meshList[i] = (Mesh)EditorGUILayout.ObjectField("Element "+i,rand.meshList[i],typeof(Mesh));
						break;
					case RandomizeMeshEditor.ArrayType.Materials:
						rand.materialsList[i] = (Material)EditorGUILayout.ObjectField("Element "+i,rand.materialsList[i],typeof(Material));
						break;
				}
			}
		}
		return isExpanded;
	}
	
	private void ResetArraySizes()
	{
		//Make copies of the old arrays
		Mesh[] newMeshes = new Mesh[rand.numberOfElements];
		Material[] newMaterials = new Material[rand.numberOfElements];
		string[] newTags = new string[rand.numberOfElements];
		//Reset the copied arrays as needed
		for(int i = 0; i < rand.numberOfElements; i++)
		{
			if(rand.tagList.Length > i) newTags[i] = rand.tagList[i];
			if(rand.meshList.Length > i) newMeshes[i] = rand.meshList[i];
			if(rand.materialsList.Length > i) newMaterials[i] = rand.materialsList[i];
		}
		//Set the old arrays to the new arrays
		rand.tagList = newTags;
		rand.meshList = newMeshes;
		rand.materialsList = newMaterials;
	}
}
