using UnityEngine;

public class MessageUnitPath: Message {
	public Vector3 location;
	
	/// <summary>
	/// Passes the pathedObject to whoever is listening
	/// </summary>
	public MessageUnitPath(GameObject aSource, Vector3 aPath) : base(aSource, Dispatcher.UNIT_PATH) {
		location = aPath;
		base.Send();
	}
	
	/// <summary>
	/// The location of the path.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToArgumentString ()
	{
		return location.ToString();
	}
}
