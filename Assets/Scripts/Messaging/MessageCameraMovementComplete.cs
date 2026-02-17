using UnityEngine;

public class MessageCameraMovementComplete : Message {
	public MessageCameraMovementComplete(GameObject aSender) : base(aSender, Dispatcher.CAMERA_MOVE_COMPLETE) { 
		base.Send();
	}
}
