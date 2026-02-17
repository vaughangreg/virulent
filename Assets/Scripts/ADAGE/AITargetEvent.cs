using UnityEngine;
using System.Collections;

public class AITargetEvent : ADAGEPlayerEvent {
	public string group{ get; set;}
	public int unit_id{ get; set;}
	public int target_id{ get; set;}
	
	public AITargetEvent(string group,int unit_id,int target_id):base(){
		this.group = group;
		this.unit_id = unit_id;
		this.target_id = target_id;
	}
}