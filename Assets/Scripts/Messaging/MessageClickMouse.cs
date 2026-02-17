using UnityEngine;
using System.Collections;

/// <summary>
/// Used for tracking mouse clicks.
/// </summary>
public class MessageClickMouse : Message {
	public Vector3 clickPosition;
	
	/// <summary>
	/// Where the mouse was clicked.
	/// </summary>
	/// <param name="whereClicked">
	/// A <see cref="Vector3"/>
	/// </param>
	public MessageClickMouse(Vector3 whereClicked) : base(null, Dispatcher.CLICK_MOUSE) {
		clickPosition = whereClicked;
		base.Send();
	}
	
	/// <summary>
	/// Returns click coordinates.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToArgumentString() {
		return clickPosition.ToString();	
	}
}
 