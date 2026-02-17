using UnityEngine;
using System.Collections;

public class UnitDeathEvent : ADAGEPlayerEvent {
	public string group{ get; set;}
	public int unit_id{ get; set;}

	public UnitDeathEvent(string group, int unit_id):base(){
		this.group = group;
		this.unit_id = unit_id;
	}
	
}