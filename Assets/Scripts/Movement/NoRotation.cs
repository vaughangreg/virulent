using UnityEngine;
using System.Collections;

/// <summary>
/// Halts rotation. Not needed if a rigidbody is present.
/// Note that SCALES MUST BE UNIFORM for this to work.
/// </summary>
public class NoRotation : MonoBehaviour {
	protected Quaternion m_fixedRotation;
	
	/// <summary>
	/// Fix rotation after everything else has acted.
	/// </summary>
	void LateUpdate() {
		//transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		transform.rotation = Quaternion.identity;
	}
}
