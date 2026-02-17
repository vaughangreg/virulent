using UnityEngine;
using System.Collections;

public class MessageAudioIdle : Message { 
	public MusicManager.State fromState;
	/// <summary>
	/// Sends a message saying the audio is idle.
	/// </param>
	public MessageAudioIdle(GameObject aSource, MusicManager.State aFromState) : base(aSource, Dispatcher.AUDIO_IDLE) {
		fromState = aFromState;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return fromState.ToString();	
	}
}
