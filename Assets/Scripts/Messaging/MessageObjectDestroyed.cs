using UnityEngine;

/// <summary>
/// Message about a destroyed object. Currently, the object 
/// destroyed is the same as the source. 
/// </summary>
public class MessageObjectDestroyed : Message {
	public GameObject destroyedObject;
	public string destroyer;

	/// <summary>
	///Sends a message with a string specified in the Event script 
	/// </summary>
	/// <param name="Source">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="Destroyed Object">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="Destroying Object">
	/// A <see cref="GameObject"/>
	/// </param>
	public MessageObjectDestroyed (GameObject aSource, string aDestroyer, GameObject aDestroyedObject) : base(aSource, Dispatcher.OBJECT_DESTROYED)
	{ 
		destroyedObject = aDestroyedObject;
		destroyer = aDestroyer;
		base.Send();
	}
}
