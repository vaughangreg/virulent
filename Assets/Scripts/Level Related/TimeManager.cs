using UnityEngine;

public class TimeManager : MonoBehaviour {
	public float timeScaleUnpaused = 1.0f;
	public float secondsPerTransition = 1.0f;
	
	protected float m_timeScaleOriginal;
	protected float m_timeScaleTarget;
	protected float m_secondsAtTransition;
	
	void Start() {
		Dispatcher.Listen(Dispatcher.PAUSE_GAME, gameObject);
		Dispatcher.Listen(Dispatcher.TIME_CHANGE, gameObject);
		enabled = false;
	}
	
	/// <summary>
	/// Moves towards the new unpaused time scale.
	/// </summary>
	void Update() {
		float delta = Time.realtimeSinceStartup - m_secondsAtTransition;
		float t = delta / secondsPerTransition;
		
		Time.timeScale = Virulent.Math.SmoothLerp(m_timeScaleOriginal, m_timeScaleTarget, t);
		if (t >= 1.0f) {
			enabled = false;
			timeScaleUnpaused = m_timeScaleTarget;
		}
	}
	
	/// <summary>
	/// Change the time scale to pause the game.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessagePauseGame"/>
	/// </param>
	void _OnPauseGame(MessagePauseGame m) {
		if (m.paused) Time.timeScale = Constants.TIME_MIN;
		else          Time.timeScale = timeScaleUnpaused;
	}
	
	/// <summary>
	/// Set a new unpaused time scale.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageTimeChange"/>
	/// </param>
	void _OnTimeChange(MessageTimeChange m) {
		m_secondsAtTransition = Time.realtimeSinceStartup;
		m_timeScaleOriginal   = Time.timeScale;
		m_timeScaleTarget     = m.newTimeScale;
		enabled = true;
	}
}
