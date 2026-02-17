using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles audio transitions, etc. Currently handles music.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {
	public float fadeSpeed = 1.0f;
	public float volumeMax = 0.3f;

	protected float m_sfxVolumeCache;
	public float sfxVolume {
		get { return m_sfxVolumeCache; }
		set {
			sfxPlayer.GetComponent<AudioSource>().volume = value;
			m_sfxVolumeCache = value;
		}
	}
	public float musicVolume {
		get { return GetComponent<AudioSource>().volume; }
		set {
			GetComponent<AudioSource>().volume = value;
			if (value <= 0.0f) {
				GetComponent<AudioSource>().Stop();
				enabled = false;
			}
			if (value > 0.0f && !GetComponent<AudioSource>().isPlaying)
				GetComponent<AudioSource>().Play();
		}
	}

	public GameObject sfxPlayer;

	public AudioClip sFXMenuSelect;
	public AudioClip sFXMenuConfirm;
	public AudioClip sFXMenuDeny;
	public AudioClip[] sFXMenuAdvanceText;
	public AudioClip sFXStartButtonCollision;
	public AudioClip sFXBeacon;

	public AudioClip[] music;
	public int[] levelForMusicId;
	protected AudioClip m_pendingMusic;

	protected float m_timeSpent = 0;

	//TODO: possibly find a better class to put the time stuff in
	private float currentTime;
	private float previousTime;
	private float deltaTime {
		get { return currentTime - previousTime; }
	}

	public enum State {
		FadeIn,
		Idle,
		FadeOut
	}
	public enum VolumeType {
		Music,
		SFX
	}

	protected const float IN = 1.0f;
	protected const float OUT = -1.0f;

	State m_currentState = State.Idle;
	public static MusicManager singleton;

	protected Dictionary<AudioClip, float> timeHash = new Dictionary<AudioClip, float>();

	/// <summary>
	/// Listen for audio change requests.
	/// </summary>
	void Awake() {
		Dispatcher.Listen(Dispatcher.AUDIO_CHANGE, gameObject);
		Dispatcher.Listen(Dispatcher.SET_VOLUME, gameObject);
		
		//Setup the delta time stuff (to avoid timeScale issues)
		previousTime = Time.realtimeSinceStartup;
		currentTime = Time.realtimeSinceStartup;
		if (singleton == null)
			singleton = this;
		
		singleton.musicVolume = PlayerPrefs.GetFloat("MusicVolume", volumeMax);
		singleton.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
	}

	void OnLevelWasLoaded(int levelLoaded) {
		m_pendingMusic = null;
		AudioClip newMusic = music[levelForMusicId[levelLoaded]];
		if (GetComponent<AudioSource>().clip != newMusic) {
			if (musicVolume > 0.0f) {
				m_currentState = State.FadeOut;
				enabled = true;
				m_pendingMusic = newMusic;
			}

			else {
				GetComponent<AudioSource>().clip = newMusic;
				m_currentState = MusicManager.State.Idle;
			}
		}
	}

	/// <summary>
	/// Perform a fade transition or idles.
	/// </summary>
	void Update() {
		bool shouldEnable = false;
		
		//Do state actions
		switch (m_currentState) {
			case State.FadeIn:
				m_currentState = Transition(IN);
				shouldEnable = true;
				break;
			case State.FadeOut:
				m_currentState = Transition(OUT);
				shouldEnable = true;
				break;
			case State.Idle:
				break;
		}
		
		List<AudioClip> keys = new List<AudioClip>(timeHash.Keys);
		foreach (AudioClip aKey in keys) {
			timeHash[aKey] -= Time.deltaTime;
			if (timeHash[aKey] > 0)
				shouldEnable = true;
		}
		
		enabled = shouldEnable;
	}

	/// <summary>
	/// Changes audio volume based on direction; sends idle message when needed.
	/// </summary>
	/// <param name="direction">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// A <see cref="State"/>
	/// </returns>
	State Transition(float direction) {
		m_timeSpent += (Time.deltaTime * fadeSpeed);
		
		musicVolume = volumeMax * ((direction < 0) ? (1.0f - m_timeSpent) : m_timeSpent);
		if (m_timeSpent >= 1.0f) {
			m_timeSpent = 0.0f;
			if (direction < 0) {
				musicVolume = 0.0f;
				GetComponent<AudioSource>().clip = m_pendingMusic;
				return State.FadeIn;
			}
			// new MessageAudioIdle(gameObject, m_currentState);
			return State.Idle;
		}
		enabled = true;
		return m_currentState;
	}

	/// <summary>
	/// Change to a new transition type.
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageAudioRequest"/>
	/// </param>
	void _OnAudioRequest(MessageAudioRequest e) {
		if (e.targetState == MusicManager.State.FadeOut) {
			//Debug.Log("---Invoking DelayIdle in 1.5s ---");
			Invoke("DelayIdleNotification", 1.5f);
		}
		
		// We're ignoring state requests made
		// outside the manager now.
		// m_currentState = e.targetState;
		// enabled = true;
	}

	void DelayIdleNotification() {
		new MessageAudioIdle(gameObject, State.FadeOut);
	}

	/// <summary>
	/// Apply the new volume to the manager
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageSetVolume"/>
	/// </param>
	void _OnSetVolume(MessageSetVolume m) {
		Debug.Log(m.ToString());
		if (m.volumeType == MusicManager.VolumeType.SFX) {
			singleton.sfxVolume = m.volume;
			PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
		}
		if (m.volumeType == MusicManager.VolumeType.Music) {
			musicVolume = Mathf.Clamp(m.volume, 0, volumeMax);
			PlayerPrefs.SetFloat("MusicVolume", musicVolume);
			new MessageAudioIdle(gameObject, m_currentState);
		}
	}

	protected bool m_isGoalSfx = false;
	public static void PlaySfx(AudioClip aClip) {
		AudioSource sfxPlayerAudio = singleton.sfxPlayer.GetComponent<AudioSource>();
		
		float targetVolume = 1.0f;
		
		// Don't interrupt important text.
		if (singleton.m_isGoalSfx && sfxPlayerAudio.isPlaying)
			targetVolume = 0.15f;
		else if (singleton.m_isGoalSfx) {
			singleton.m_isGoalSfx = false;
		}
		
		if (singleton.sfxVolume > 0.0f) {
			if (!singleton.timeHash.ContainsKey(aClip))
				singleton.timeHash.Add(aClip, -1.0f);
			
			if (singleton.timeHash[aClip] < 0) {
				singleton.timeHash[aClip] = aClip.length;
				sfxPlayerAudio.GetComponent<AudioSource>().PlayOneShot(aClip, targetVolume);
			}
		}
		singleton.enabled = true;
	}

	public static void PlayGuiSfx(AudioClip aClip) {
		AudioSource sfxPlayerAudio = singleton.sfxPlayer.GetComponent<AudioSource>();
		if (singleton.sfxVolume > 0.0f) {
			sfxPlayerAudio.GetComponent<AudioSource>().PlayOneShot(aClip, 1.0f);
		}
	}

	public static void PlayGoalSfx(AudioClip aClip) {
		AudioSource sfxPlayerAudio = singleton.sfxPlayer.GetComponent<AudioSource>();
		
		if (singleton.sfxVolume > 0.0f) {
			if (sfxPlayerAudio.isPlaying) {
				sfxPlayerAudio.Stop();
			}
			sfxPlayerAudio.clip = aClip;
			sfxPlayerAudio.Play();
			singleton.m_isGoalSfx = true;
		}
	}

	public static bool IsSoundPlaying(AudioClip aClip) {
		if (singleton.timeHash.ContainsKey(aClip)) {
			if (singleton.timeHash[aClip] <= 0) return false;
			else return true;
		}

		else return false;
	}

	/// <summary>
	/// Returns the current volume of the SFX played
	/// </summary>
	/// <returns>
	/// Volume between 0 and 1
	/// </returns>
	public static float GetSfxVolume() {
		return singleton.sfxVolume;
	}

	/// <summary>
	/// Returns the current volume 'max' of the Music
	/// </summary>
	/// <returns>
	/// Volume of music between 0 and 1
	/// </returns>
	public static float GetMusicVolume() {
		return singleton.musicVolume;
	}

	public void OnDestroy() {
		CancelInvoke();
	}
}
