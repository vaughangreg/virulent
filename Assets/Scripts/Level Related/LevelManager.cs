using UnityEngine;

public class LevelManager : MonoBehaviour {
	public Objective[] victoryObjectives;
	public Objective[] defeatObjectives;
	public bool        orDefeatObjectives = false;
	public Objective[] earlyEndObjectives;
	
	public string winningScene;
	public string losingScene;

	public bool m_isGameWon;
	public bool m_isGameEnding = false;
	public bool m_musicFaded;
	
	public Texture[] preloadedTextures;
	
	private bool m_IsEarlyEndActive;
	private bool m_LoadedLevelAlready = false;
	
	private delegate bool CheckDelegate(Objective[] anObjectivesArray);
	private CheckDelegate CheckVictory;
	private CheckDelegate CheckDefeat;
	private CheckDelegate CheckEarlyEnd;

	void Start() {
		Dispatcher.Listen(Dispatcher.OBJECTIVE_COMPLETE, gameObject);
		Dispatcher.Listen(Dispatcher.GAME_OVER, gameObject);
		Dispatcher.Listen(Dispatcher.AUDIO_IDLE, gameObject);
		Dispatcher.Listen(Dispatcher.LOAD_LEVEL, gameObject);
		PreloadTextures();
		
		CheckVictory  = CheckWithAnd;
		if (orDefeatObjectives) CheckDefeat = CheckWithOr;
		else CheckDefeat = CheckWithAnd;
		CheckEarlyEnd = CheckWithAnd;
	}
	
	/// <summary>
	/// See http://forum.unity3d.com/threads/69730-Texture.AwakeFromLoad for why
	/// we might need this.
	/// </summary>
	void PreloadTextures() {
		foreach (Texture aTexture in preloadedTextures) {
			int th = aTexture.height;
			//Debug.Log(aTexture.name + " height: " + th, aTexture);
		}
	}

	/// <summary>
	/// when games is over, fade out the music and set m_isGameWon
	/// </summary>
	void _OnGameOver(MessageGameOver m) {
		new MessageAudioRequest(gameObject, MusicManager.State.FadeOut);
		m_isGameWon = m.isGameWon;
		m_isGameEnding = true;
	}

	/// <summary>
	/// when audio is finished, let the manager know
	/// </summary>
	void _OnAudioIdle (MessageAudioIdle m) {
		if (m.fromState == MusicManager.State.FadeOut) m_musicFaded = true;
	}




	void _OnLoadLevel(MessageLoadLevel m) { 


		if (m.levelToLoad == null){ //Load the inspector levels
			if (m_isGameEnding) {
				if (m_isGameWon) {
					PlayerPrefs.SetInt(winningScene, 1);
					Application.LoadLevel(winningScene);
					//Debug.Log("Loading winning level.");
				}
				else {
					Application.LoadLevel(losingScene);
					//Debug.Log("Loading losing level.");
				}
			}
		}
		else { //Load the specified level
			if (!m_LoadedLevelAlready) {
				m_LoadedLevelAlready = true;
				//Debug.Log("Loading selected level: " + m.levelToLoad);
				Application.LoadLevel(m.levelToLoad);
			}
		}
	}

	/// <summary>
	/// on ObjectiveComplete check to see if victory or defeat conditions have been met
	/// </summary>
	void _OnObjectiveComplete (MessageObjectiveComplete m) {
		bool isVictoryObjectivesComplete = CheckVictory(victoryObjectives);
		bool isDefeatObjectivesComplete  = CheckDefeat(defeatObjectives);
		bool isEarlyEndObjectives        = CheckEarlyEnd(earlyEndObjectives);

		ObjectiveCompleteEvent objcmp = new ObjectiveCompleteEvent ();

		//if either sets of objectives have been completed, change scenes
		if (isVictoryObjectivesComplete) {
			objcmp.type = "Victory";
			new MessageGameOver(gameObject, true);
			return;
		}
		if (isDefeatObjectivesComplete) {
			if (isEarlyEndObjectives) {
				objcmp.type = "EarlyVictory";
				new MessageGameOver(gameObject, true);
			}
			else{
				objcmp.type = "Failure";
				new MessageGameOver(gameObject, false);
			}
		}
		
		if (isEarlyEndObjectives) {
			if (!m_IsEarlyEndActive) {
				objcmp.type = "EarlyEnd";
				new MessageEarlyLevelEnd(gameObject);
			}
			m_IsEarlyEndActive = true;
		}
		ADAGE.LogData<ObjectiveCompleteEvent> (objcmp);
	}
	
	/// <summary>
	/// Returns true if ALL of the objectives are completed.
	/// </summary>
	/// <param name="anObjectivesArray">
	/// A <see cref="Objective[]"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	bool CheckWithAnd(Objective[] anObjectivesArray) {
		bool isComplete = anObjectivesArray.Length > 0;
		
		foreach (Objective anObjective in anObjectivesArray) {
			if (!anObjective.isComplete) {
				isComplete = false;
				break;
			}
		}
		
		return isComplete;
	}
	
	/// <summary>
	/// Returns true if ANY of the objectives are completed.
	/// </summary>
	/// <param name="anObjectivesArray">
	/// A <see cref="Objective[]"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	bool CheckWithOr(Objective[] anObjectivesArray) {
		bool isComplete = false;
		
		foreach (Objective anObjective in anObjectivesArray) {
			if (anObjective.isComplete) {
				isComplete = true;
				break;
			}
		}
		
		return isComplete;
	}
}
