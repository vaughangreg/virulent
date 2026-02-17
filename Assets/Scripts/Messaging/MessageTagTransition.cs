using UnityEngine;

/// <summary>
/// Signals that an object is transitioning to a new tag
/// </summary>
public class MessageTagTransition : Message { 
	public string tag1;
	public string tag2;
	
	/// <summary>
	/// Sends a message with two string to move from the first to the second
	/// </param>
	public MessageTagTransition(GameObject aSource, string incomingTag1, string incomingTag2) : base(aSource, Dispatcher.TAG_TRANSITION ) {
		tag1 = incomingTag1;
		tag2 = incomingTag2;
		base.Send();
	}
	
	/// <summary>
	/// Returns the tags in transition.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToArgumentString() {
		return tag1 + " transitioned into " + tag2;
	}
}
