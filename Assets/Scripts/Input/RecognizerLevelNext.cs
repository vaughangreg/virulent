using UnityEngine;

public class RecognizerLevelNext : IRecognizer {
	public float   minSecondsRequired = 2.0f;	// Don't really need this due to the level info
												// screen; however, if it changes, this should
												// stop the advance.
	protected bool m_didRecognize = false;
	
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
		if (m_didRecognize) return false;
		
		if (Input.touchCount == 11 && Time.timeSinceLevelLoad > minSecondsRequired) {
			int currentLevelIndex = Application.loadedLevel;
			string currentLevelName = Application.loadedLevelName;
			
			if (currentLevelName == "Game Won" || currentLevelName == "Game Lost" || currentLevelName == "Level Select") {
				Application.LoadLevel(0);	
			}
			else Application.LoadLevel(currentLevelIndex + 1);
			m_didRecognize = true;
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
