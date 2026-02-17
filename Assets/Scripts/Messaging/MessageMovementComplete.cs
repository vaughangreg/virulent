using UnityEngine;


public class MessageMovementComplete : Message {
	
	public MessageMovementComplete(GameObject incStoppedObject) 
		: base (incStoppedObject, Dispatcher.MOVE_COMPLETE) 
	{
		base.Send();
	}
}

