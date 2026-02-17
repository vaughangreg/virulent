using UnityEngine;

/// <summary>
/// Used for tracking GUI button clicks.
/// </summary>
public class MessageMultipleInput : Message {
	
	/// <summary>
	/// Records the clicked button.
	/// </summary>
	/// <param name="aButton">
	/// A <see cref="GameObject"/>
	/// </param>
	public MessageMultipleInput(GameObject input) : base(input, Dispatcher.MULTIPLE_INPUT) {
		base.Send();
	}
	
	/// <summary>
	/// Returns button information.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToArgumentString() {
		return "Multiple Input Touches Detected";
	}
}
 