using UnityEngine;

/// <summary>
/// Hack for 2D arrays of Audio Clips.
/// </summary>
public class AudioSet : MonoBehaviour {
	public AudioClip[] clips;
	public int clipsPerGroup;
	
	/// <summary>
	/// Retrieves a random audio clip from the specified group.
	/// </summary>
	/// <param name="inGroup">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="AudioClip"/>
	/// </returns>
	public AudioClip RandomClip(int inGroup) {
		int baseIndex = clipsPerGroup * inGroup;
		int offsetIndex = Random.Range(0, clipsPerGroup - 1);
		
		return clips[baseIndex + offsetIndex];
	}
}
