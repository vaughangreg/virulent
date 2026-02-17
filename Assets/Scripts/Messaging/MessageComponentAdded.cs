using UnityEngine;
using System.Collections;

public class MessageComponentAdded : Message {
	public GameObject toThisGameObject;
	public Component thisComponentWasAdded;
	
	/// <summary>
	/// captures the component that was added and the gameObject it was added to
	/// </param>
	public MessageComponentAdded(Component incomingComponent, GameObject incomingGameObject) 
		: base(incomingGameObject, Dispatcher.COMPONENT_ADDED) 
	{
		toThisGameObject = incomingGameObject;
		thisComponentWasAdded = incomingComponent;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return thisComponentWasAdded.ToString();	
	}
}
