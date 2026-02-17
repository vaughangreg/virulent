using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ADAGELevelMonitor : MonoBehaviour {


	private Dictionary<string,int> stats;
	private int last_level_id = -1;
	private string last_level_name = "";
	private float last_load = 0.0f;

	void Awake () {
		DontDestroyOnLoad (this.gameObject);
		Dispatcher.Listen(Dispatcher.FEEDBACK_DISPLAY, gameObject);
	}
	
	private float last_active;
	private int time_out = 60;
	private bool inactivity_logged = false;
	void Update(){
		//Check for Inactivity
		if (Input.anyKey) {
			last_active = Time.time;
		}else{
			if((Time.time - last_active) > time_out){
				if(!inactivity_logged){
					SessionTimeoutEvent session_timeout = new SessionTimeoutEvent();
					ADAGE.LogData<SessionTimeoutEvent>(session_timeout);
				}
				inactivity_logged = true;
			}else{
				inactivity_logged = false;
			}
		}
	}


	private int level_attempt = 0;
	void OnLevelWasLoaded(int level) {
		if (level != last_level_id){
			level_attempt = 0;
		}else{
			level_attempt += 1;
		}

		if(last_level_id != -1){
			Debug.Log ("Level End");
			LevelEnd endlevel = new LevelEnd(last_level_id,last_level_name,Time.time - last_load,level_attempt,stats);
			ADAGE.LogContext<LevelEnd>(endlevel);
			
			stats = null;
		}
		last_load = Time.time;
		last_level_id = level;
		last_level_name = Application.loadedLevelName;

		LevelStart startlevel = new LevelStart (level, Application.loadedLevelName);
		ADAGE.LogContext<LevelStart> (startlevel);
	}

	void _OnFeedbackDisplay(MessageFeedbackDisplay m){
		LevelFeedback feedback_info = m.source.GetComponent<LevelFeedback> ();

		stats = new Dictionary<string, int> ();
		foreach (DeathCount dc in feedback_info.deathCounts) {
			stats.Add(dc.label,dc.deaths);
		}
	}
}