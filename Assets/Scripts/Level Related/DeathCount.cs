using UnityEngine;
using System.Collections;

[System.Serializable]
public class DeathCount {
	public string label;
	public Constants.CombatFlags watchedFlag;
	[System.NonSerialized] public int deaths;
	
	public bool AddDeath(GameObject objectToCheck)
	{
		//get the combat script, error if it doesn't have it
		Combat combat = objectToCheck.GetComponent<Combat>();
		if (!combat) {
			Debug.LogError("DeathCount could not find Combat script on " + objectToCheck.name);
			return false;
		}
		if (((int)watchedFlag & (int)combat.thisObjectFlag) > 0)
		{
			deaths += 1;
			return true;
		}
		else return false;
	}
}

