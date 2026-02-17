using UnityEngine;
using System.Collections;

public class PostLevelStart : ADAGEContextStart {


	public int level_id;
	public PostLevelStart(int level_id,string levelname):base(){
		this.level_id = level_id;
		this.name = levelname;
	}
}
