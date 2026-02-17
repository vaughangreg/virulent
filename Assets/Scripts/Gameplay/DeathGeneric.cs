using UnityEngine;

/// <summary>
/// Removes the attached object.
/// </summary>
public class DeathGeneric : MonoBehaviour {
	public GameObject deathPrefab;
	public AudioClip  deathSound;
	public GameObject preservedChild;
	
	/// <summary>
	/// Give feedback and destroy ourselves.
	/// </summary>
	void DeathPrep() {
		GameObject result = null;
		if (deathPrefab) result = (GameObject)Instantiate(deathPrefab, transform.position, transform.rotation);
		
		// Sync death animation if needed.
		if (result != null && result.GetComponent<Animation>() != null && gameObject.GetComponent<Animation>() != null) {
			foreach (AnimationState aState in gameObject.GetComponent<Animation>()) {
				AnimationState resultState = result.GetComponent<Animation>()[aState.name];
				if (resultState != null) {
					result.GetComponent<Animation>().wrapMode = WrapMode.ClampForever;
					resultState.enabled = true;
					resultState.weight = 1.0f;
					resultState.time = aState.time;
					result.GetComponent<Animation>().Sample();
					resultState.enabled = false;
				}
			}
		}
		
		if (result && deathSound) {
			MusicManager.PlaySfx(deathSound);
		}
		if (preservedChild != null) preservedChild.transform.parent = result.transform;
		Destroy(gameObject);
	}
}
