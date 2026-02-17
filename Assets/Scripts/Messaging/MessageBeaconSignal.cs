using UnityEngine;

public class MessageBeaconSignal : Message {
	public GameObject[] targetObj;
	public string[] targetTag;
	public float targetTime;
	
	/// <summary>
	/// Sends a message with the colliding/triggering objects
	/// </param>
	public MessageBeaconSignal(GameObject[] incomingGameObject, string[] incomingString, float incomingFloat) 
		: base(new GameObject(), Dispatcher.BEACON_SIGNAL) 
	{ 
		targetObj = incomingGameObject;
		targetTag = incomingString;
		targetTime = incomingFloat;
		base.Send();
	}
	
	public override string ToArgumentString() {
		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		
		builder.Append(targetTime);
		
		if (targetObj.Length > 0) {
			builder.Append(targetObj[0]);
			for (int i = 1; i < targetObj.Length; ++i) {
				builder.Append("\t");
				builder.Append(targetObj[i].name);
			}
		}
		
		if (targetTag.Length > 0) {
			builder.Append(targetTag[0]);
			for (int i = 1; i < targetTag.Length; ++i) {
				builder.Append("\t");
				builder.Append(targetTag[i]);
			}
		}
		
		return builder.ToString();
	}
}
