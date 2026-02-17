using UnityEngine;
using System.Collections;

public class LevelStart : ADAGEContextStart {
	public int level_id;
	public LevelStart(int level_id,string levelname):base(){
		this.level_id = level_id;
		this.name = levelname;
	}
}