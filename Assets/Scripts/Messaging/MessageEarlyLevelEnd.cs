using UnityEngine;

/// <summary>
/// Signals that an object created something else. 
/// </summary>
public class MessageEarlyLevelEnd : Message {
	
	/// <summary>
	/// Sends a message with a string specified in the Event script
	/// </param>
	public MessageEarlyLevelEnd(GameObject aSource) : base(aSource, Dispatcher.EARLY_LEVEL_END ) {
		Debug.Log("Early End Active");
		base.Send();
	}
	
	/// <summary>
	/// Returns the created object.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToArgumentString() {
		return "EarlyEnd";
	}
}