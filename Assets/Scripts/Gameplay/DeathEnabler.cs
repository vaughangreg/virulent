using UnityEngine;

/// <summary>
/// When killed, it enables selected monobehaviours
/// and destroys others.
/// </summary>
public class DeathEnabler : MonoBehaviour {
	public MonoBehaviour[] toEnable;
	public MonoBehaviour[] toDestroy;
	
	/// <summary>
	/// Enable and destroy monobehaviours in the appropriate arrays.
	/// </summary>
	void DeathPrep() {		
		foreach (MonoBehaviour aBehaviour in toEnable) {
			aBehaviour.enabled = true;	
		}
		
		foreach (MonoBehaviour aBehaviour in toDestroy) {
			Destroy(aBehaviour);
		}
	}
}
