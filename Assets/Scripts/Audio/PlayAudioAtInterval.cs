using UnityEngine;
using System.Collections;

public class PlayAudioAtInterval : MonoBehaviour
{
	public AudioClip audioClipToPlay;
	public float initialDelay;
	public float timeInterval;
	
	private float m_timeToWait = 0.0f;
	private float m_timeElapsed = 0.0f;
	
	void Awake()
	{
		m_timeToWait = initialDelay;
	}
	
	void Update()
	{
		m_timeElapsed += Time.deltaTime;
		//Play the sound effect if the time elapsed meets the wait time
		if (m_timeElapsed > m_timeToWait) {
			m_timeElapsed = 0;
			m_timeToWait = timeInterval;
			MusicManager.PlaySfx(audioClipToPlay);
		}
	}
}

