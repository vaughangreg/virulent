using UnityEngine;
using System.Collections;

public class EnergyEvent : ADAGEPlayerEvent {
	public string group{ get; set;}
	public int unit_id{ get; set;}
	public int amount{ get; set;}
	public int available{ get; set;}
	
	public EnergyEvent(string group,int unit_id,int amount,int available):base(){
		this.group = group;
		this.unit_id = unit_id;
		this.amount = amount;
		this.available = available;
	}
}
