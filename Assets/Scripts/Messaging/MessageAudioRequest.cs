using UnityEngine;

public class MessageAudioRequest : Message { 
	public MusicManager.State targetState;
	
	/// <summary>
	/// Sends a request to change to the current music state.
	/// </param>
	public MessageAudioRequest(GameObject aSource, MusicManager.State aTargetState)
		: base(aSource, Dispatcher.AUDIO_CHANGE) 
	{
		targetState = aTargetState;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return targetState.ToString();	
	}
}
