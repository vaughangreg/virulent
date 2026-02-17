using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathCreateEvent: ADAGEPlayerEvent {
	public string group{ get; set;}
	public int unit_id{ get; set;}
	public List<Vector3> path{ get; set;}
	
	public PathCreateEvent(string group,int unit_id):base(){
		this.group = group;
		this.unit_id = unit_id;
	}
}
