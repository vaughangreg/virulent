using UnityEngine;
using System.Collections;

public class UnitUpgradeEvent : ADAGEPlayerEvent {
	public string group{ get; set;}
	public int unit_id{ get; set;}
	
	public UnitUpgradeEvent(string group,int unit_id):base(){
		this.group = group;
		this.unit_id = unit_id;
	}
}
