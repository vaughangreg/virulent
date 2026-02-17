using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Select models for the atlas in the editor before executing.
/// </summary>
/// <description>
/// Note that currently the UVs for the original model will 
/// be overwritten. In the future, we may want to create an
/// input and an output directory to preserve the originals.
/// </description>
public class TexturePacker : MonoBehaviour {
	private const string NAME_MATERIAL = "Atlas (Generated)";
	private const string NAME_SHADER = "Diffuse";
	private const string NAME_TEXTURE = "Texture Atlas (generated)";
	private const string NAME_SHADER_TEXTURE = "_MainTex";
	private const string OUTPUT_TEXTURE = "/Artwork/Textures/packed-texture.png";
	private const string OUTPUT_MATERIAL = "Assets/Artwork/Materials/Packed Material.mat";
	private const int    TEXTURE_SIZE = 1024;	// Should be 1024 or 2048
	
	/// <summary>
	/// Packs textures into an atlas and remaps UVs.
	/// </summary>
	[MenuItem("Virulent/Textures/Pack Textures")]
	public static void PackTextures() {
		Texture2D packedTexture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, false);
		
		GameObject[] selectedObjects;
		Texture2D[] selectedTextures;
		
		try {
			GetSelectedData(out selectedObjects, out selectedTextures);
			Pack(packedTexture, selectedObjects, selectedTextures);
		
			SaveTexture(packedTexture);
		}
		catch (System.Exception e) {
			Debug.LogError("Packing aborted: " + e);
		}
	}
	
	/// <summary>
	/// Creates a material and updates UVs on models.
	/// </summary>
	/// <param name="anOutputTexture">
	/// A <see cref="Texture2D"/>
	/// </param>
	/// <param name="anObjectArray">
	/// A <see cref="GameObject[]"/>
	/// </param>
	/// <param name="aTextureArray">
	/// A <see cref="Texture2D[]"/>
	/// </param>
	protected static void Pack(Texture2D anOutputTexture, GameObject[] anObjectArray, Texture2D[] aTextureArray) {
		Rect[] uvRects = anOutputTexture.PackTextures(aTextureArray, 2, TEXTURE_SIZE, false);
		anOutputTexture.name = NAME_TEXTURE;
		
		Material packedMaterial = new Material(Shader.Find(NAME_SHADER));
		packedMaterial.name = NAME_MATERIAL;
		AssetDatabase.CreateAsset(packedMaterial, OUTPUT_MATERIAL);
		
		int i = 0;
		Vector2[] oldUvs, newUvs;
		foreach (GameObject anObject in anObjectArray) {
			anObject.GetComponent<Renderer>().sharedMaterial = packedMaterial;
			anObject.GetComponent<Renderer>().sharedMaterial.SetTexture(NAME_SHADER_TEXTURE, anOutputTexture);
			MeshFilter theFilter = anObject.GetComponent<MeshFilter>();
			
			if (!theFilter) Debug.LogError("No mesh filter on " + anObject.name, anObject);
			oldUvs = theFilter.sharedMesh.uv;
			newUvs = new Vector2[oldUvs.Length];
			for (int j = 0; j < oldUvs.Length; ++j) {
				newUvs[j] = new Vector2((oldUvs[j].x * uvRects[i].width) + uvRects[i].x,
				                        (oldUvs[j].y * uvRects[i].height) + uvRects[i].y);
			}
			theFilter.sharedMesh.uv = newUvs;
			++i;
		}
	}
	
	/// <summary>
	/// Retrieves the objects and textures for the atlas.
	/// </summary>
	/// <param name="selectedObjects">
	/// A <see cref="GameObject[]"/>
	/// </param>
	/// <param name="selectedTextures">
	/// A <see cref="Texture2D[]"/>
	/// </param>
	protected static void GetSelectedData(out GameObject[] selectedObjects, out Texture2D[] selectedTextures) { 
		int length = Selection.gameObjects.Length;
		
		selectedObjects = new GameObject[length];
		selectedTextures = new Texture2D[length];
		
		Selection.gameObjects.CopyTo(selectedObjects, 0);
		int i = 0;
		foreach (GameObject anObject in selectedObjects) {
			selectedTextures[i++] = anObject.GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
		}
    }
	
	/// <summary>
	/// Saves the texture to PATH_OUTPUT.
	/// </summary>
	/// <param name="aTexture">
	/// A <see cref="Texture2D"/>
	/// </param>
	protected static void SaveTexture(Texture2D aTexture) {
		byte[] bytes = aTexture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + OUTPUT_TEXTURE, bytes);
		
		Debug.Log("Packed texture written to " + Application.dataPath + OUTPUT_TEXTURE);
	}
}
