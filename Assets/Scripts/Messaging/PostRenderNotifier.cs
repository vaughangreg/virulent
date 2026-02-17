using UnityEngine;

/// <summary>
/// Should attach to main camera.
/// </summary>
[RequireComponent(typeof(Camera))]
public class PostRenderNotifier : MonoBehaviour {
	/// <summary>
	/// Notify listeners about post rendering.
	/// </summary>
	void OnPostRender() {
		// new MessagePostRender();	
		Dispatcher.DoPostRender();
	}
}
