using UnityEngine;
using System.Collections;

public class AlmanacViewCategoryEvent : ADAGEGameEvent {
	
	public string category;

	public AlmanacViewCategoryEvent(string category):base(){
		this.category = category;
	}
}
