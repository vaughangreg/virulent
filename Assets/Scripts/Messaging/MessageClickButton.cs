using UnityEngine;

/// <summary>
/// Used for tracking GUI button clicks.
/// </summary>
public class MessageClickButton : Message {
	/// <summary>
	/// Records the clicked button.
	/// </summary>
	/// <param name="aButton">
	/// A <see cref="GameObject"/>
	/// </param>
	public MessageClickButton(GameObject aButton) : base(aButton, Dispatcher.CLICK_BUTTON) {
		base.Send();
	}
	
	/// <summary>
	/// Returns button information.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToArgumentString() {
		if (source.transform.parent) {
			GameObject parent = source.transform.parent.gameObject;
			string result = System.String.Format("{0} at {1}", parent.name, parent.transform.position);
			return result;
		}
		else return "(no parent)";
	}
}
 