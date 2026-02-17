using UnityEngine;

/// <summary>
/// Message about a destroyed object. Currently, the object 
/// destroyed is the same as the source. 
/// </summary>
public class MessageInvaginationComplete : Message
{
	/// <summary>
	/// Sends a message indicating invagination site hasfinished its animation
	/// </param>
	public MessageInvaginationComplete (GameObject aSource) : base(aSource, Dispatcher.INVAGINATION_COMPLETE){
		base.Send ();
	}
}
