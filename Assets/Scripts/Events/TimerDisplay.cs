using UnityEngine;
using System.Collections;

public class TimerDisplay : MonoBehaviour {
	
	public float timerLength = 1f;
	public float timeBetweenUpdates = 1f;
	public AudioClip soundToPlayAtFinish;
	public string preString;
	public string postString;
	public bool killOnComplete = false;
	
	private float nextTime;
	private float initialTime;
	private AudioClip nullClip = null; 
	
	// Use this for initialization
	void Start () {
		initialTime = Time.time;
		nextTime = Time.time + timeBetweenUpdates;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextTime) {
			nextTime = Mathf.Round(Time.time) + timeBetweenUpdates;
			new MessageCheckPoint(gameObject, preString + (timerLength - Mathf.Round(Time.time - initialTime)).ToString() + postString, nullClip);
		}
		if ((Time.time - initialTime > timerLength) && killOnComplete) Destroy(this);
	}
}
