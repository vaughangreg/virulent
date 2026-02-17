using UnityEngine;

public class MessageObjectMoved : Message {
	/// <summary>
	/// sends the gameObject that was just moved
	/// </param>
	public MessageObjectMoved(GameObject aSource) : base(aSource, Dispatcher.OBJECT_MOVED) {
		base.Send();
	}
	
}
