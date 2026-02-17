using UnityEngine;
using System.Collections;

public enum ButtonDisplayType {
	CountUnitBest,
	CountGeneral,
	TimeLongest,
}

public class LevelButton : MonoBehaviour {
	public int minLevelReached = 4;
	public bool amActive;
	
	public string textLabel;
	public string textInfo;
	
	public ButtonDisplayType displayValue;
	
	/// <summary>
	/// OnTrigger, send up a message to select the level.
	/// </summary>
	void OnTriggerEnter() {
		if (amActive) {
			SendMessageUpwards("Select", gameObject, SendMessageOptions.RequireReceiver);
		}
	}
	
	/// <summary>
	/// Resets the color so they won't be permanently changed.
	/// </summary>
	void OnDestroy() {
		GetComponent<Renderer>().sharedMaterial.color = Color.white;
	}
}
