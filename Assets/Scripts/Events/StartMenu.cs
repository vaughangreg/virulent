using UnityEngine;
using System.Collections.Generic;

public class StartMenu : MonoBehaviour {
	public GameObject startButton;
	public GameObject continueButton;
	public GameObject credits;
	public GameObject challenges;
	
	public Material startButtonMatOn;
	public Material startButtonMatOff;
	
	public string firstLevel;
	public string creditsLevel;
	public string levelSelectionLevel;
	public string challengesLevel;
	
	public AudioClip selectionSFX;
	
	private bool m_startingLevel = false;
	private float m_delay = 0.5f;
	private float m_loadTime;
	private string m_levelToLoad;
	
	void Start() {
		Dispatcher.Listen(Dispatcher.COLLISION_HAPPENS, gameObject);
		if (m_levelToLoad == null) {
		
			MainMenuStart menustart = new MainMenuStart ();
			ADAGE.LogContext<MainMenuStart> (menustart);
		}
		
		//Debug.Log("Constant check: " + PlayerPrefs.GetInt(Constants.LEVELS[Constants.LEVELS.Count - 1]));
		
		if (PlayerPrefs.GetInt(Constants.LEVELS[Constants.MIN_LEVEL_FOR_CHALLENGES], 0) == 0) {
			challenges.SetActiveRecursively(false); 
		}
	}
	
	void _OnCollisionEvent(MessageCollisionEvent m) {
		//Debug.Log(m.source + "\t" + m.collisionObj1 + "\t" + m.collisionObj2);
		
		GameObject hitObject = m.collisionObj1;
		
		if (hitObject == startButton) StartGame();
		if (hitObject == continueButton) Continue();
		if (hitObject == credits) Credits();
		if (hitObject == challenges) Challenges();
	}


	private void StartGame() {
		PlaySFX(selectionSFX);
		startButton.GetComponent<Renderer>().sharedMaterial = startButtonMatOn;
		m_loadTime = Time.time + m_delay;
		m_startingLevel = true;
		m_levelToLoad = firstLevel;
		PlayerPrefs.SetInt(firstLevel, 1);
		MenuExit ();
	}
	
	private void Continue() {
		PlaySFX(selectionSFX);
		continueButton.GetComponent<Renderer>().sharedMaterial = startButtonMatOn;
		m_loadTime = Time.time + m_delay;
		m_startingLevel = true;
		m_levelToLoad = levelSelectionLevel;
		MenuExit ();
	}
	
	private void Credits() {
		PlaySFX(selectionSFX);
		credits.GetComponent<Renderer>().sharedMaterial = startButtonMatOn;
		m_loadTime = Time.time + m_delay;
		m_startingLevel = true;
		m_levelToLoad = creditsLevel;
		MenuExit ();
	}
	
	private void Challenges() {
		PlaySFX(selectionSFX);
		challenges.GetComponent<Renderer>().sharedMaterial = startButtonMatOn;
		m_loadTime = Time.time + m_delay;
		m_startingLevel = true;
		m_levelToLoad = challengesLevel;
		MenuExit ();
	}

	private void MenuExit(){
		MainMenuEnd menuend = new MainMenuEnd ();
		ADAGE.LogContext<MainMenuEnd> (menuend);
	}
	
	void Update() {
		if (m_startingLevel) {
			if (Time.time > m_loadTime) Application.LoadLevel(m_levelToLoad);
		}
	}
	
	void PlaySFX(AudioClip aClip)
	{
		MusicManager.PlayGuiSfx(aClip);
	}
}
