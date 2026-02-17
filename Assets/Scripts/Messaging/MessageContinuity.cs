using UnityEngine;

public class MessageContinuity : Message { 
	public int amountToTransfer;
	
	/// <summary>
	/// Sends a message saying the audio is idle.
	/// </param>
	public MessageContinuity(GameObject aSource, int anAmount) : base(aSource, Dispatcher.CONTINUITY_SET) {
		amountToTransfer = anAmount;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return amountToTransfer.ToString();	
	}
}
