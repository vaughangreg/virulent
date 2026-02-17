using UnityEngine;

/// <summary>
/// Plays a sound when hit.
/// </summary>
[RequireComponent(typeof(Collider))]
public class OnCollisionPlaySfx : MonoBehaviour {
	public AudioClip[] clips;
	
	/// <summary>
	/// Selects the sound and plays it.
	/// </summary>
	void OnCollisionEnter() {
		int index = Mathf.FloorToInt(clips.Length * Random.value);
		MusicManager.PlaySfx(clips[index]);
	}
}
