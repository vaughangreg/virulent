using UnityEngine;

public class TerrainDeath : MonoBehaviour {
	void OnDestroy() {
		AiHelper.RescanWithDelay();
	}
}
