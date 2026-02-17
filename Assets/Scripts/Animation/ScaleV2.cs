using UnityEngine;
using System.Collections;

public class ScaleV2 : MonoBehaviour {

	public Vector3 myOriginalScale;
	
	public float speed = 1f;
	//public float lengthOfScale = 1f;
	public bool shouldPingPong = false;
	public bool shouldReverseMotion = false;
	public bool shouldExpDecay = false;
	public float expDecay = 0.98f;
	public bool shouldAddDelay = false;
	public float delayLength = 0.5f;
	
	
	private float m_time = 0f;
	private int m_counter = 0;
	private float m_delay = 0f;
	
	// Use this for initialization
	void Start () {
		myOriginalScale = transform.localScale;
		for(int i = 0; i < 3; i++) {
			myOriginalScale[i] = Mathf.Abs(myOriginalScale[i]);
		}
		//if (shouldAddDelay) m_delay = delayLength; 
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.enabled || !gameObject.active) return;
		if (shouldAddDelay) {
			if (CheckDelay()) return;
		}
		GetTime();
		ChangeScale();
	}
	
	bool CheckDelay() {
		m_delay -= Time.deltaTime;
		if (m_delay < 0) {
			return false;
			//m_time = -m_delay;
		} else {
			return true;
		}
	}
	
	void GetTime() {
		if (shouldPingPong) {
			m_time = Mathf.PingPong(Time.time, 1f);
		} else {
			m_time += speed * Time.deltaTime;
			if (m_time > 1f) {
				m_time = 0f;
				m_delay = delayLength;
			}
		}
		CalculateCounter();
	}
	
	void CalculateCounter() {
		m_counter = (int)(m_time * 100f);
		//if (m_counter == 0) m_counter = 1;
	}
	
	void ChangeScale() {
		if (shouldAddDelay && m_delay > 0f) {
			
		} else {
			if (!shouldExpDecay) {
				if (shouldReverseMotion) transform.localScale = ((float)m_counter + 1) / 101f * myOriginalScale;
				else transform.localScale = (100f - (float)m_counter) / 101f * myOriginalScale;
			} else {
				if (shouldReverseMotion) transform.localScale = Mathf.Pow(expDecay, 100 - m_counter) * myOriginalScale;
				else transform.localScale = Mathf.Pow(expDecay, m_counter) * myOriginalScale;
			}
		}
		
	}
}
