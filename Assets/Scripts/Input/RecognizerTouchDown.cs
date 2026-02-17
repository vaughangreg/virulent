using UnityEngine;

/// <summary>
/// Recognizes input down.
/// </summary>
public class RecognizerTouchDown : IRecognizer {
	protected bool 		m_hasTouchStarted;
	protected Vector3 	m_touchDownPosition;
	
	public Vector3 touchDownPosition {
		get { return m_touchDownPosition; }
	}
	
	public float epsilon { 
		get { return 0.0f; }
		set { }
	}

	/// <summary>
	/// Recognizes a touch being pressed.
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
		if (!m_hasTouchStarted && isTouching) {
			m_hasTouchStarted = true;
			m_touchDownPosition = atPosition;
			return true;
		}
		else if (m_hasTouchStarted && !isTouching) m_hasTouchStarted = false;

		return false;
	}
}
