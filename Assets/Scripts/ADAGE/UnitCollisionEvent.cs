using UnityEngine;
using System.Collections;

public class UnitCollisionEvent : ADAGEPlayerEvent {

	public string self;
	public int self_id;
	public string other;
	public int other_id;
	public int damage;
	
	public UnitCollisionEvent(GameObject self,GameObject other, int damage):base(){
		this.self = self.tag.ToString();
		this.self_id = self.GetInstanceID();
		this.other = other.tag.ToString();
		this.other_id = other.GetInstanceID();
		this.damage = damage;
	}
}
