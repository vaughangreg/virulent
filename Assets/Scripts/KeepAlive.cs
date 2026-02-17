using UnityEngine;

/// <summary>
/// Makes an object permanently exist.
/// </summary>
public class KeepAlive : MonoBehaviour {
	/// <summary>
	/// Prevent the object from being destroyed each scene.
	/// </summary>
	void Awake() {
		DontDestroyOnLoad(gameObject);	
	}
}
