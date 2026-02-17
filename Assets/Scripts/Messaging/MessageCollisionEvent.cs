using UnityEngine;

public class MessageCollisionEvent : Message {
	public GameObject collisionObj1;
	public GameObject collisionObj2;
	
	/// <summary>
	/// Sends a message with the colliding/triggering objects
	/// </param>
	public MessageCollisionEvent(GameObject incomingGameObject1, GameObject incomingGameObject2) 
		: base(incomingGameObject1, Dispatcher.COLLISION_HAPPENS) 
	{
		collisionObj1 = incomingGameObject1;
		collisionObj2 = incomingGameObject2;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return collisionObj2.name;	
	}
}
