using UnityEngine;

/// <summary>
/// Goes to the next level upon starting.
/// </summary>
public class LoadNextLevel : MonoBehaviour {
	
	/// <summary>
	/// Go to the next level.
	/// </summary>
	void Start() {
		Application.LoadLevel(Application.loadedLevel + 1);	
	}
}
