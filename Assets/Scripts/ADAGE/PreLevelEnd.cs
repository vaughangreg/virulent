using UnityEngine;
using System.Collections;

public class PreLevelEnd : ADAGEContextEnd {
	
	public float duration;
	public int level_id;
	public PreLevelEnd(int level_id,string levelname,float duration):base(){
		this.level_id = level_id;
		this.name = levelname;
		this.duration = duration;
	}
}
