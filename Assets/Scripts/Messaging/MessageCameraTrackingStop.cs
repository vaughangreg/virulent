using UnityEngine;

public class MessageCameraTrackingStop : Message {
	public MessageCameraTrackingStop(GameObject aSender) : base(aSender, Dispatcher.CAMERA_FOCUS) {
		base.Send();
	}
}
