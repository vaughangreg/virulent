using UnityEngine;

/// <summary>
/// Message about an object being downgraded (e.g., shield being removed).
/// </summary>
public class MessageDowngradingObject : Message {
	public GameObject downgradedObject;

	/// <summary>
	/// The downgrading message constructor.
	/// </param>
	public MessageDowngradingObject(GameObject aDowngradedObject) : base(aDowngradedObject, Dispatcher.OBJECT_DOWNGRADING) {
		downgradedObject = aDowngradedObject;
		base.Send();
	}
}
