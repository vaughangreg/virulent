using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEnd : ADAGEContextEnd {
	public int level_id;
	public int attempt;
	public float duration;
	public Dictionary<string,int> stats;

	public LevelEnd(int level_id,string levelname,float time,int attempt,Dictionary<string,int> stats):base(){
		this.level_id = level_id;
		this.name = levelname;
		this.duration = time;
		this.attempt = attempt;
		this.stats = stats;
	}
}