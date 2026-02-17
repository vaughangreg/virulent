using UnityEngine;
using System.Collections;

public class PreLevelStart : ADAGEContextStart {

	public int level_id;
	public PreLevelStart(int level_id,string levelname):base(){
		this.level_id = level_id;
		this.name = levelname;
	}
}
