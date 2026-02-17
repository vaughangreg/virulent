using UnityEngine;

public class MessageFeedbackCountChanged : Message { 
	public int amount = 0;
	
	/// <summary>
	/// Sends a message to the feedback panel to update a misc count.
	/// </param>
	public MessageFeedbackCountChanged(GameObject aSource, int amount) : base(aSource, Dispatcher.FEEDBACK_COUNT_CHANGED) {
		this.amount = amount;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return amount.ToString();	
	}
}
