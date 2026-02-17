using UnityEngine;

/// <summary>
/// Fixes an object at a particular offset from a parent.
/// Note that SCALES MUST BE UNIFORM for this to work.
/// </summary>
public class FixedParentalOffset : MonoBehaviour {
	protected Vector3 m_offset;
	
	/// <summary>
	/// Store the set rotation.
	/// </summary>
	void OnStay() {
		m_offset = transform.position - transform.parent.position;
	}
	
	/// <summary>
	/// Fix rotation after everything else has acted.
	/// </summary>
	void LateUpdate() {
		transform.position = transform.parent.position + m_offset;
	}
}
