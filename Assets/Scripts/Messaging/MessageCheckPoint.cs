using UnityEngine;
using System.Collections;

public class MessageCheckPoint : Message { 
	public string checkPointString;
	public AudioClip checkPointAudio;
	
	/// <summary>
	/// Sends a message with a string specified in the CheckPointEvent script
	/// </param>
	public MessageCheckPoint(GameObject aSource, string incomingString, AudioClip incomingClip)
		: base(aSource, Dispatcher.CHECK_POINT) 
	{
		checkPointString = incomingString; 
		checkPointAudio = incomingClip;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return System.String.Format("{0}\t{1}", checkPointString, checkPointAudio);	
	}
}
