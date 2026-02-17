using UnityEngine;

/// <summary>
/// Base class for messages sent through the messenger
/// </summary>
[System.Serializable]
public class Message : System.Object {
	[HideInInspector] public GameObject source;
	[HideInInspector] public string listenerType;
	[HideInInspector] public string functionName;
	
	public Message(GameObject aSource, string type) {
		source = aSource;
		listenerType = type;
		
		// function name for MessageMyMessage becomes _OnMyMessage()
		functionName = GetFunctionName();
	}
	
	public string GetFunctionName() {
		return string.Format("_On{0}", this.GetType().ToString().Substring(7));
	}
	
	/// <summary>
	/// A way to get around C#'s requirement that the base constructor instantiate first.
	/// Call base.Send() at the end of child constructors.
	/// </summary>
	protected void Send() {
		Dispatcher.Send(this);
	}
	
	/// <summary>
	/// Called by the data manager to display arguments.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public virtual string ToArgumentString() { return ""; }
}