using UnityEngine;

public class ActivateChildrenOnEnable : MonoBehaviour {
	void OnEnable() {
		gameObject.SetActiveRecursively(true);
	}
}
