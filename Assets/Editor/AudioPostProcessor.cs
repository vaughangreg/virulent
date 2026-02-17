using UnityEngine;
using UnityEditor;

/// <summary>
/// Forces audio to 2D.
/// </summary>
public class AudioPostProcessor : AssetPostprocessor {
	/// <summary>
	/// Makes imported audio 2D.
	/// </summary>
	void OnPreprocessAudio() {
		AudioImporter importer = (AudioImporter)assetImporter;
		importer.threeD = false;
	}
}
