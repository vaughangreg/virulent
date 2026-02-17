using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PostLevelEnd : ADAGEContextEnd {
	
	public float duration;
	public int level_id;

	public PostLevelEnd(int level_id,string levelname,float duration):base(){
		this.level_id = level_id;
		this.name = levelname;
		this.duration = duration;
	}
}
