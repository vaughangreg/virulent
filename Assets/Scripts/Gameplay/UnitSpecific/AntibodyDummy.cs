using UnityEngine;

/// <summary>
/// Kills the parent upon animation completion.
/// </summary>
[RequireComponent(typeof(Animation))]
public class AntibodyDummy : MonoBehaviour {
	public int		maxChildren = 2;
	protected bool	m_hasStarted = false;
	
	/// <summary>
	/// Changes the wrapmode.
	/// </summary>
	void Awake() {
		GetComponent<Animation>().wrapMode = WrapMode.Once;
	}
	
	/// <summary>
	/// Destroys the parent when we hit the target count.
	/// </summary>
	void Update () {
		if (!GetComponent<Animation>().isPlaying && m_hasStarted) {
			if (transform.parent.GetChildCount() == maxChildren) {
				Destroy(transform.parent.gameObject);
			}
		}
		else if (GetComponent<Animation>().isPlaying) m_hasStarted = true;
	}
}
