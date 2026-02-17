using UnityEngine;
using System.Collections;

public class AlmanacViewItemEvent :  ADAGEGameEvent {
	
	public string item;
	
	public AlmanacViewItemEvent(string item):base(){
		this.item = item;
	}
}
