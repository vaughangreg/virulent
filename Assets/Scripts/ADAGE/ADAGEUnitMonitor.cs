using UnityEngine;
using System.Collections;

public class ADAGEUnitMonitor : MonoBehaviour {
	private string tag = "";
	private int id;

	void Awake(){
		tag = this.gameObject.tag.ToString ();
		id = this.gameObject.GetInstanceID();
	}

	void Start () {
		UnitSpawnEvent spawnevent = new UnitSpawnEvent(tag,id);
		ADAGE.UpdatePositionalContext (transform.position, transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitSpawnEvent> (spawnevent);
	}

	void OnDestroy(){
		UnitDeathEvent deathevent = new UnitDeathEvent(tag,id);
		ADAGE.UpdatePositionalContext (transform.position, transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitDeathEvent> (deathevent);
	}
}