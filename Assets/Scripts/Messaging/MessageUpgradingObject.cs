using UnityEngine;

/// <summary>
/// Message about a destroyed object. Currently, the object 
/// destroyed is the same as the source. 
/// </summary>
public class MessageUpgradingObject : Message {
	public GameObject upgradedObject;

	/// <summary>
	/// Sends a message with a string specified in the Event script
	/// </param>
	public MessageUpgradingObject (GameObject aUpgradedObject) : base(aUpgradedObject, Dispatcher.OBJECT_UPGRADING) {
		upgradedObject = aUpgradedObject;

		UnitUpgradeEvent upgrade = new UnitUpgradeEvent (aUpgradedObject.gameObject.tag.ToString(),aUpgradedObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (upgradedObject.transform.position, upgradedObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitUpgradeEvent> (upgrade);
	
		base.Send();
	}
}
