using UnityEngine;
using System.Collections; 

public class Timer : MonoBehaviour {
	
	private float timeToWatch = 0f;
	private float counter = 0f;
	private GameObject callingGameObject;
	private bool timerSet = false;
	
	public void SetTimer(float timerInSeconds, GameObject starterGameObject) { 
		timeToWatch = timerInSeconds;
		callingGameObject = starterGameObject; 
		timerSet = true;
	}
	
	void Update () {
		if (!timerSet) return;
		counter += Time.deltaTime; 
		if (counter > timeToWatch) {
			new MessageTimerComplete(callingGameObject);
			Destroy(this);
		}
	}
}
