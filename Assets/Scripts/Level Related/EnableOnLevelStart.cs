using UnityEngine;

public class EnableOnLevelStart : MonoBehaviour {
	public bool forceDisableOnAwake = true;
	
	void Awake() {
		EnableManager.AddObject(gameObject, forceDisableOnAwake);
	}
}

