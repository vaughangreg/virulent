using UnityEngine;
using System.Collections;

public class Scale : MonoBehaviour
{
	public Vector3 scaleAxis;
	public float growth; 
	
	public bool shouldPingPong;
	public float speed = 1f;
	public bool shouldExponentialDecay = false;
	public float decay = 0.0001f;
	public bool reverseScale = false;
	public bool useDelay = true;
	public float delayBetweenDecays = 1f;
	private float delayTimer ;
	
	public float maxScale = 100f;
	public float minScale = 0.01f;
	
	private int counter = 0;
	private float myTime = 1f;
	private float baseGrowth { get { return growth; } }
	private float multiplier = 1f;
	private Vector3 originalScale; 
	private float currentTime;
	private float scaleFactor = 1;
	
	
	void Start()
	{ 
		originalScale = transform.localScale;
		for(int i = 0; i < 3; i++) {
			originalScale[i] = Mathf.Abs(originalScale[i]);
		}
		delayTimer = delayBetweenDecays;
		
		//decay = decay * Mathf.Abs(100f * transform.localScale.x);
		//Debug.Log(decay);
		Debug.Log(originalScale.x + "  " + transform.localScale.x);
		decay = Mathf.Abs(originalScale.x / speed);
		Debug.Log(decay);
		maxScale = Mathf.Abs(100f * transform.localScale.x);
		minScale = Mathf.Abs(transform.localScale.x / 10f);
	}
	
	void Update()
	{
		
		GetTime();
		ScaleMe();  
		if(transform.localScale.x < minScale) transform.localScale = minScale * Vector3.one;
		if(transform.localScale.x > maxScale) transform.localScale = maxScale * Vector3.one;
	}
	
	void ScaleMe() {
		if (delayTimer > 0) return;
		if (shouldExponentialDecay) {
			scaleFactor = Mathf.Pow(decay, counter); 
			transform.localScale = originalScale * scaleFactor;
		} else { 
			scaleFactor = decay * counter;
			transform.localScale = originalScale - multiplier * scaleFactor * originalScale;
		}
		
	}
	
	void GetTime() { 
		if (delayTimer > 0 && useDelay) {
			delayTimer -= Time.deltaTime;
			return;
		}
		if (shouldPingPong) {
			myTime = speed * Mathf.PingPong(Time.time,1f / speed);
		} else {
			myTime -= Time.deltaTime * speed;
			if(myTime < 0) {
				myTime = 1f; 
				if (useDelay) {
					delayTimer = delayBetweenDecays;
					transform.localScale = minScale * Vector3.one;
				}
			} 
		}
		
		
		if (reverseScale) {
			counter = (int)(myTime * 100f);
		} else {
			counter = 100 - (int)(myTime * 100f);
		}
		
		
		/*if (falling) {
			myTime -= Time.deltaTime / speed;
			counter++;
		}
		else {
			myTime += Time.deltaTime / speed;
			counter--;
		}
		if (myTime < 0f) {
			if (shouldPingPong) {
				falling = false;
				myTime = 0f; 
				multiplier = -1f;
			} else {
				myTime = 1f;
			}
		}
		if (myTime > 1f) {
			falling = true;
			myTime = 1f;
			multiplier = 1f;
		}*/
		//Debug.Log(myTime + "   " + counter + "    " + transform.localScale);
	}
	
}

