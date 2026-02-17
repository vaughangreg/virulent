using UnityEngine;

public class MessageTimeChange : Message {
	public float newTimeScale;
	
	public MessageTimeChange(GameObject aSource, float aNewScale) : base(aSource, Dispatcher.TIME_CHANGE) {
		newTimeScale = aNewScale;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return newTimeScale.ToString();	
	}

}
