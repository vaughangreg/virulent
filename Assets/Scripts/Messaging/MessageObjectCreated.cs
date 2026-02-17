using UnityEngine;

/// <summary>
/// Signals that an object created something else. 
/// </summary>
public class MessageObjectCreated: Message { 
	public GameObject createdObject;
	public GameObject producer;
	
	/// <summary>
	/// Sends a message with a string specified in the Event script
	/// </param>
	public MessageObjectCreated(GameObject aSource, GameObject aProducer, GameObject aCreatedObject) : base(aSource, Dispatcher.OBJECT_CREATED ) {
		producer = aProducer;
		createdObject = aCreatedObject;
		base.Send();
	}
	
	/// <summary>
	/// Returns the created object.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToArgumentString() {
		return producer.name + " produced " + createdObject.name;
	}
}
