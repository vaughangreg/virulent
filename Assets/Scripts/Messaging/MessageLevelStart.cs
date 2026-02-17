using UnityEngine;
using System.Collections;

public class MessageLevelStart : Message
{
	public MessageLevelStart() : base(null,Dispatcher.LEVEL_START)
	{
		// Debug.Log("Starting");
		base.Send();
	}
}

