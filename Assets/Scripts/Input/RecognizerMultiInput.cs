using UnityEngine;

public class RecognizerMultiInputWarning : IRecognizer {
	
	const float  WARNING_DELAY_SECONDS = 2.0f;	
	float m_timeDown;
	
	/// <summary>
	/// Returns true if the recognizer fired.
	/// </summary>
	/// <returns>
	/// True if recognized; false otherwise.
	/// </returns>
	/// <param name='isTouching'>
	/// Finger/mouse state.
	/// </param>
	/// <param name='atPosition'>
	/// Finger/mouse position.
	/// </param>
	public bool ProcessState(bool isTouching, Vector3 atPosition) {
			
		//if there are 2 or more touchcounts count seconds
		if (Input.touchCount >= 2) {
			m_timeDown += Time.deltaTime;
		}
		else {
			m_timeDown = 0;
		}
		
		//if count goes the 
		if (m_timeDown > WARNING_DELAY_SECONDS) {
			return true;
		}
				
		return false;
	}
	
	/// <summary>
	/// Gets or sets the sensitivity of the recognizer.
	/// </summary>
	/// <value>
	/// The epsilon in screen pixels.
	/// </value>
	public float epsilon { 
		get { return 0.0f; } 
		set { }
	}
	
	/// <summary>
	/// Where the initial touch was recorded.
	/// </summary>
	public Vector3 touchDownPosition { 
		get { return Vector3.zero; }
	}
}
