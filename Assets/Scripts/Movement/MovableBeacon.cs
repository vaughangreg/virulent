using UnityEngine;

[RequireComponent(typeof(Selectable))]
public class MovableBeacon : MonoBehaviour {
	public Movable mover;
	
	/// <summary>
	/// Starts a new path.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStart(InputManager source) {
		mover.GetComponent<Selectable>().SilentlySelect();
		source.selection = mover.gameObject;
		gameObject.active = false;
	}
}
