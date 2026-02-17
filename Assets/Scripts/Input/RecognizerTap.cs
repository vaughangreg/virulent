using UnityEngine;

/// <summary>
/// Recognizes a tap in one location.
/// </summary>
public class RecognizerTap : IRecognizer {
	protected bool m_startedTouching = false;
	protected Vector3 m_touchDownPosition;
	protected float m_epsilon;
	
	public Vector3 touchDownPosition 
	{
		get { return m_touchDownPosition; }
	}
	
	public float epsilon { 
		get { return m_epsilon; }
		set { m_epsilon = value; }
	}
	
	/// <summary>
	/// Tracks taps (down, up) with movement < epsilon.
	/// </summary>
	/// <param name="isTouching">
	/// A <see cref="System.Boolean"/>
	/// </param>
	/// <param name="atPosition">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool ProcessState(bool isTouching, Vector3 atPosition) { 
		// Starting to touch.
		if (!m_startedTouching && isTouching) {
			m_startedTouching = true;
			m_touchDownPosition = atPosition;
		}
		// Stopped touching. How far did we move?
		else if (m_startedTouching && !isTouching) {
			m_startedTouching = false; 
			if ((atPosition - m_touchDownPosition).sqrMagnitude < m_epsilon * m_epsilon) {
				return true;
			}
		}
		return false;
	}
}
