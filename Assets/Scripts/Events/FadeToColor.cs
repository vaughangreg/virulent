using UnityEngine;
using System.Collections;

public class FadeToColor : MonoBehaviour {
	public Renderer rendererTarget;
	
	public Color startColor;
	public Color endColor;
	public float timeToTake = 3;
	public float timeVariation = 1;
	
	private float m_timeTaken;
	private float m_timeToWait;
	
	
	void Start() {
		m_timeToWait = timeToTake + (Random.value * timeVariation);
		m_timeTaken = 0;
	}
	
	void Update() {
		if (m_timeTaken <= m_timeToWait) {
			m_timeTaken += Time.deltaTime;
			rendererTarget.material.color = Color.Lerp(startColor, endColor, m_timeTaken/m_timeToWait);
		}
		else Destroy(this);
	}
}

