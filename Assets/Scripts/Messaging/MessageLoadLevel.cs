using UnityEngine;
using System.Collections;

public class MessageLoadLevel : Message
{
	public string levelToLoad = null;
	
	/// <summary>
	/// Sets level to load along in the message
	/// </summary>
	/// <param name="aLevelToLoad">
	/// A <see cref="System.String"/>
	/// </param>
	public MessageLoadLevel(string aLevelToLoad) : base(null, Dispatcher.LOAD_LEVEL) {
		levelToLoad = aLevelToLoad;
		base.Send();
	}
	/// <summary>
	/// Constructor that doesn't set level. Allows reciever to decide what level to load
	/// </summary>
	public MessageLoadLevel() : base(null, Dispatcher.LOAD_LEVEL) {
		base.Send();
	}
}

