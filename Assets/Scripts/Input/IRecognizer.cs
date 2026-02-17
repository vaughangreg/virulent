using UnityEngine;

/// <summary>
/// Core interface for gesture recognition.
/// </summary>
public interface IRecognizer {
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
	bool ProcessState(bool isTouching, Vector3 atPosition);
	
	/// <summary>
	/// Gets or sets the sensitivity of the recognizer.
	/// </summary>
	/// <value>
	/// The epsilon in screen pixels.
	/// </value>
	float epsilon { get; set; }
	
	/// <summary>
	/// Where the initial touch was recorded.
	/// </summary>
	Vector3 touchDownPosition { get; }
}
