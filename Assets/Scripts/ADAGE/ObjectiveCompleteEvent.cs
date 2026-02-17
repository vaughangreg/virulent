using UnityEngine;
using System.Collections;

public class ObjectiveCompleteEvent : ADAGEGameEvent {
	public string type{ get; set;}
	public ObjectiveCompleteEvent(string type = ""):base(){
		this.type = type;
	}
}
