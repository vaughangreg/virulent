using UnityEngine;

/// <summary>
/// Recognizes paths drawn with the input.
/// </summary>
public class RecognizerSlide : IRecognizer {
	protected bool m_startedTouching;
	protected Vector3 m_firstPosition;
	protected Vector3 m_touchDownPosition;
	protected float m_epsilon;
	
	protected int m_numRecognitions = 0;
	public bool isFirstRecognition {
		get { return m_numRecognitions == 1; }
	}
	
	protected bool m_isLastRecognition = false;
	public bool isLastRecognition {
		get {
			return m_isLastRecognition;
		}
	}
	
	public Vector3 touchDownPosition 
	{
		get { return m_touchDownPosition; }
	}
	
	public Vector3 firstPosition 
	{
		get { return m_firstPosition; }
	}
	
	public float epsilon { 
		get { return m_epsilon; }
		set { m_epsilon = value; }
	}
	
	/// <summary>
	/// Tracks slides.
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
		// We just started touching, so it could be a slide.
		if (!m_startedTouching && isTouching) {
			m_startedTouching = true;
			m_touchDownPosition = atPosition;
			m_firstPosition = atPosition;
			m_isLastRecognition = false;
			m_numRecognitions = 0;
			return false;
		}
		else if (m_startedTouching && isTouching) {
			float usedEpsilon = m_numRecognitions == 0 ? (m_epsilon * m_epsilon) : (m_epsilon * m_epsilon / 5.0f);
			
			// We are touching; have we moved enough to add a point?
			if ((atPosition - m_touchDownPosition).sqrMagnitude > usedEpsilon) {
				m_touchDownPosition = atPosition;
				++m_numRecognitions;
				return true;
			}
			// Didn't move far enough from the last point.
			return false;
		}
		// Are we no longer touching? Note that this could be confused with the tap recognizer.
		else if (m_startedTouching && !isTouching) {
			m_startedTouching = false;
			// This is how we ensure we don't get confused with the tap recognizer
			m_isLastRecognition = m_numRecognitions > 0;
			m_numRecognitions = 0;
			return m_isLastRecognition;
		}
		
		return false;
	}
}
