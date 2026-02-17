using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Displays information of your level playthrough at win or lose time. Also sends a message to let Level Manager know we're going to load a level
/// </summary>
public class LevelFeedback : MonoBehaviour {

	//THIS WHOLE THING IS NOT VERY MODULAR: 
	//TODO: make it modular, accepting a feedback class or similar idea

	public bool countTimeTaken     = false;
	public bool countDeaths        = true;
	public bool showBestTime       = false;
	public bool showLongestTime    = false;
	public bool showBestCount      = false;
	public string countLabel       = "Count:";
	public bool recordTimeNotUnits = false;
	public float delayToEndLevel   = 0.0f;

	public bool pauseAtStart = true;

	public DeathCount[] deathCounts;

	public GameObject transitionPrefab;
	
	private float m_timeToWait;
	private float m_timeWaited;
	private int   m_count = 0;
	private bool  m_delayedSelectableLayers;
	private LayerMask m_maskValue;

	private LevelGUIElement continueButton = new LevelGUIElement("CONTINUE", new Vector2(819, 440), GuiResources.feedbackButtonSize);
	private LevelGUIElement restartButton = new LevelGUIElement("REPLAY", new Vector2(66, 440), GuiResources.feedbackButtonSize);

	private LevelGUIElement statsPane = new LevelGUIElement("STATS", new Vector2(262, 104), new Vector2(496, 221));
	private LevelGUIElement deathsPane = new LevelGUIElement("DEATHS", new Vector2(262, 442), new Vector2(496, 221));

	private float  timeTaken;
	private float  bestTime;
	private float  longestTime;
	private int    bestCount;
	private string bestTimePrefName;
	private string longestTimePrefName;
	private string bestCountPrefName;

	private bool       levelDone = false;
	private bool       levelWon = false;
	private bool       musicDone = false;
	private bool       transitionDone = false;
	private GameObject transitionObject;

	private GUIStyle buttonStyle;
	private GUIStyle statsStyle;
	
	private const float SPACING_Y = 30.0f;

	#region Monobehaviour Methods
	void Awake() {
		useGUILayout = false;
		//Setup listeners
		if (countDeaths)    Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
		if (countTimeTaken) Dispatcher.Listen(Dispatcher.GAME_OVER, gameObject);
		if (showBestCount)  Dispatcher.Listen(Dispatcher.FEEDBACK_COUNT_CHANGED, gameObject);
		
		//Listen for music to know when to display gui
		Dispatcher.Listen(Dispatcher.AUDIO_IDLE, gameObject);
		Dispatcher.Listen(Dispatcher.MOVE_COMPLETE, gameObject);
		Dispatcher.Listen(Dispatcher.PAUSE_GAME, gameObject);
		
		//Quick show the transition from before
		CreateTransition(true);
		
		bestTimePrefName    = "BestTime " + Application.loadedLevelName;
		longestTimePrefName = "LongestTime " + Application.loadedLevelName;
		
		// For now, this is orthogonal to the Continuity manager :-(
		bestCountPrefName   = "Count" + Application.loadedLevelName;
		
		//Setup a best time for the level if it doesn't exist yet
		bestTime    = PlayerPrefs.GetFloat(bestTimePrefName, System.Single.MaxValue);
		longestTime = PlayerPrefs.GetFloat(longestTimePrefName, 0.0f);
		bestCount   = PlayerPrefs.GetInt(bestCountPrefName, 0);
	}
	
	void Start() {

		//Pause the game if it is not
		new MessagePauseGame(gameObject, true);


		//Grab the GUI style
		statsStyle = GuiResources.instance.statsStyle;
		buttonStyle = GuiResources.instance.buttonStyle;
		
		//Modify the Panes
		statsPane.padding = 25;
		deathsPane.padding = 25;
	}

	/// <summary>
	/// Count the time taken to play the level, and Go to a level when this one is complete or lost
	/// </summary>
	void Update() {
		if (countTimeTaken && !levelDone) {
			timeTaken += Time.deltaTime;
			//If the level is finished, not won, and the transition is over, Go to the proper level
		}

		else if (levelDone && !levelWon && transitionDone) {
			GoToLevel();
		}
		
		//Unpause the game if that property was set
		if (!pauseAtStart) {
			new MessageLevelStart();

			pauseAtStart = true;
		}
		//Do actions for a delayed Selectable Layers change
		if (m_delayedSelectableLayers)
		{
			m_timeWaited += Time.deltaTime;
			if (m_timeWaited > m_timeToWait) {
				new MessageSetSelectableLayers(m_maskValue);
				m_timeToWait = 0;
				m_timeWaited = 0;
				m_delayedSelectableLayers = false;
			}
		}
	}
	
	System.DateTime log_start;
	private bool post_logged = false;
	/// <summary>
	/// Draw all of the GUI elements: End level stats pane, buttons, etc
	/// </summary>
	public void OnGUI() {
		//Only draw the GUI when the level is finished, won, and music is over
		if (transitionDone && levelWon) {//levelDone && levelWon && musicDone) {

			if(!post_logged){
				PostLevelStart start = new PostLevelStart (Application.loadedLevel, Application.loadedLevelName);
				ADAGE.LogData<PostLevelStart> (start);
				log_start = System.DateTime.Now;
				post_logged = true;
				
				new MessageFeedbackDisplay(gameObject);
			}
			DrawStatsPane();
			DrawDeathsPane();
			DrawButtons();
		}
	}

	#endregion

	#region Listener Methods

	/// <summary>
	/// Add a destroyed object to the death counts specified if it is part of it
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageObjectDestroyed"/>
	/// </param>
	void _OnObjectDestroyed(MessageObjectDestroyed m) {
		//Make sure the object was killed in combat...
		if (SearchCollection(Constants.COLLECTION_CELL, m.destroyer) > 0 || SearchCollection(Constants.COLLECTION_VIRUS, m.destroyer) > 0) {
			//Let the death counts check to see if it should add this object
			foreach (DeathCount dc in deathCounts) {
				dc.AddDeath(m.destroyedObject);
			}
		}
	}

	/// <summary>
	/// Let the object know that the transition has completed its movement
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageMovementComplete"/>
	/// </param>
	void _OnMovementComplete(MessageMovementComplete m) {
		if (m.source.gameObject == transitionObject && levelDone) {
			transitionDone = true;
			CheckBeforePause();
			// print ("Transition Done");
		}
		if (m.source.gameObject.CompareTag("HUD Panel")) {
			GameObject hudObject = GameObject.FindGameObjectWithTag("HUD");
			if (hudObject) {
				PauseButton aButton = hudObject.GetComponent<PauseButton>();
				aButton.enabled = true;
			}
		}
	}

	/// <summary>
	/// Let us know that the level is done
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageGameOver"/>
	/// </param>
	void _OnGameOver(MessageGameOver m) {
		if (!levelDone) {
			levelDone = true;
			levelWon = m.isGameWon;
			if (delayToEndLevel > 0) Invoke("GameOverDelay", delayToEndLevel);
			else CreateTransition(false);

			new MessageSetSelectableLayers(0);
			
			//Delete the HUD objects...
			Destroy(GameObject.Find("HUD"));
			Destroy(GameObject.Find("Event Notice Manager").gameObject);
			
			//Record best time if needed
			if (levelWon) CheckIfBestTime();
		}
	}
	
	void GameOverDelay() {
		CreateTransition(false);	
	}
	
	void _OnFeedbackCountChanged(MessageFeedbackCountChanged m) {
		m_count += m.amount;
		if (m_count > bestCount) {
			PlayerPrefs.SetInt(bestCountPrefName, m_count);
			bestCount = m_count;
		}
	}
	
	/// <summary>
	/// Record a new best time if the player has beaten it
	/// </summary>
	void CheckIfBestTime() {
		if (timeTaken < bestTime) {
			bestTime = timeTaken;
			PlayerPrefs.SetFloat(bestTimePrefName, bestTime);
			if (recordTimeNotUnits)
				new MessageContinuity(gameObject, Mathf.FloorToInt(bestTime));
		}
		if (timeTaken > longestTime) {
			longestTime = timeTaken;
			PlayerPrefs.SetFloat(longestTimePrefName, longestTime);
			if (recordTimeNotUnits)
				new MessageContinuity(gameObject, Mathf.FloorToInt(longestTime));
		}
	}
	/// <summary>
	/// Let us know that the music is done
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageAudioIdle"/>
	/// </param>
	void _OnAudioIdle(MessageAudioIdle m) {
		if (m.fromState == MusicManager.State.FadeOut) {
			musicDone = true;
			CheckBeforePause();
		}
	}

	/// <summary>
	/// Enable or disable the game objects and Time.timeScale if the pause message is recieved
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessagePauseGame"/>
	/// </param>
	void _OnPauseGame(MessagePauseGame m) {
		if (m.paused) DisableGameObjects();
		else EnableGameObjects();
	}

	#endregion

	#region GUI Drawing

	/// <summary>
	/// Draws the
	/// </summary>
	void DrawStatsPane() {
		Rect printRegion = statsPane.GetGUIRect();
		//HEADER
		string header = "";
		for (int i = 0; i < Constants.LEVELS.Count; i++) { 
			if (Constants.LEVELS[i] == Application.loadedLevelName) header = Constants.LEVEL_NAMES[Application.loadedLevel - 1];
		}
		//Debug.Log(header);
		GUI.Label(printRegion, "LEVEL " + header + " COMPLETE!", statsStyle);
		printRegion.y += SPACING_Y;
		
		//DRAW LEFT SIDE OF INFORMATION
		if (showBestCount) {
			GUI.Label(printRegion, countLabel, statsStyle);
			printRegion.y += SPACING_Y;
			GUI.Label(printRegion, "Best " + countLabel, statsStyle);
			printRegion.y += SPACING_Y;
		}
		if (countTimeTaken) {
			GUI.Label(printRegion, "Time Taken:", statsStyle);
			printRegion.y += SPACING_Y;
		}
		if (showBestTime) {
			GUI.Label(printRegion, "Best Time:", statsStyle);
			printRegion.y += SPACING_Y;
		}
		if (showLongestTime) {
			GUI.Label(printRegion, "Best Time:", statsStyle);
			printRegion.y += SPACING_Y;
		}
		//DRAW RIGHT SIDE OF INFORMATION
		printRegion = statsPane.GetGUIRect();
		printRegion.x += 140;
		printRegion.y += SPACING_Y;
		if (showBestCount) {
			GUI.Label(printRegion, m_count.ToString(), statsStyle);
			printRegion.y += SPACING_Y;
			GUI.Label(printRegion, bestCount.ToString(), statsStyle);
			printRegion.y += SPACING_Y;
		}
		if (countTimeTaken)
			printRegion = DrawTime(printRegion, statsStyle, timeTaken);
		if (showBestTime)
			printRegion = DrawTime(printRegion, statsStyle, bestTime);
		if (showLongestTime)
			printRegion = DrawTime(printRegion, statsStyle, longestTime);
	}

	void DrawDeathsPane() {
		Rect printRegion = deathsPane.GetGUIRect();
		//HEADER
		GUI.Label(printRegion, "UNIT STATISTICS", statsStyle);
		printRegion.y += SPACING_Y;
		
		//DRAW LEFT SIDE OF INFORMATION
		foreach (DeathCount dc in deathCounts) {
			GUI.Label(printRegion, dc.label + ":", statsStyle);
			printRegion.y += SPACING_Y;
		}
		
		//SPACE OUT THE PRINT REGION
		printRegion = deathsPane.GetGUIRect();
		printRegion.x += 300;
		printRegion.y += SPACING_Y;
		
		//DRAW RIGHT SIDE OF INFORMATION
		foreach (DeathCount dc in deathCounts) {
			GUI.Label(printRegion, "" + dc.deaths, statsStyle);
			printRegion.y += SPACING_Y;
		}
	}

	protected const string TIME_FORMAT = "{0} minute{1}, {2:00.00} second{3}";

	/// <summary>
	/// Converts a float time into a label string.
	/// </summary>
	/// <param name="aTime">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	protected string TimeToString(float aTime) {
		int minutes = Mathf.FloorToInt(aTime / 60);
		float seconds = aTime % 60.0f;
		
		string result = System.String.Format(TIME_FORMAT, minutes, (minutes == 1 ? "" : "s"), seconds, (Mathf.Approximately(seconds, 0.0f) ? "" : "s"));
		return result;
	}

	Rect DrawTime(Rect position, GUIStyle guiStyle, float aTime) {
		string label = TimeToString(aTime);
		GUI.Label(position, label, guiStyle);
		position.y += SPACING_Y;
		return position;
	}

	void DrawButtons() {
		if (GUI.Button(continueButton.GetGUIRect(), continueButton.text, buttonStyle)) {

			PostLevelEnd end = new PostLevelEnd (Application.loadedLevel, Application.loadedLevelName,(float) (System.DateTime.Now - log_start).Duration().TotalSeconds);
			ADAGE.LogData<PostLevelEnd> (end);
			GoToLevel();
		}
		if (GUI.Button(restartButton.GetGUIRect(), restartButton.text, buttonStyle)) {
			PostLevelEnd end = new PostLevelEnd (Application.loadedLevel, Application.loadedLevelName,(float) (System.DateTime.Now - log_start).Duration().TotalSeconds);
			ADAGE.LogData<PostLevelEnd> (end);

			GoToLevel(Application.loadedLevelName);
		}
	}

	#endregion

	#region Action and Other Methods

	/// <summary>
	/// Methods handle a level goto, blank means simply go to next level, overload handles specified level
	/// </summary>
	void GoToLevel() {
		//Send a message (that LevelManager should get...) to go to the next level
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
		new MessageLoadLevel();
	}
	void GoToLevel(string level) {
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
		new MessageLoadLevel(level);
	}

	/// <summary>
	/// Check whether or not the music and transition are BOTH done, then pause the game
	/// </summary>
	void CheckBeforePause() {
		if (musicDone && transitionDone) {
			new MessagePauseGame(gameObject, true);
		}
	}


	/// <summary>
	/// Search the Constants.TAG_COLLECTION for given collection and tag, returning the count of occurances
	/// </summary>
	/// <param name="collectionToCheck">
	/// the collection we wish to check
	/// </param>
	/// <param name="tagToCheck">
	/// the tag we want to look for
	/// </param>
	/// <returns>
	/// number of times tag is found in collection
	/// </returns>
	int SearchCollection(string collectionToCheck, string tagToCheck) {
		int count = 0;
		foreach (string tag in Constants.TAG_COLLECTIONS[collectionToCheck]) {
			if (tag == tagToCheck) count++;
		}
		return count;
	}


	Transition CreateTransition(bool startInside) {
		Vector3 viewCenter = Camera.main.transform.position;
		viewCenter.y += Camera.main.nearClipPlane + 0.1f;
		transitionObject = GameObject.Instantiate(transitionPrefab, viewCenter, Quaternion.identity) as GameObject;
		Transition trans = transitionObject.GetComponent<Transition>();
		if (startInside) {
			trans.startInView = true;
			trans.destroyAtEnd = true;
		}

		else {
			trans.startInView = false;
			trans.destroyAtEnd = false;
		}
		return trans;
	}

	/// <summary>
	/// Sends a selectable layers message
	/// </summary>
	void DisableGameObjects() {
		new MessageSetSelectableLayers(0);
	}
	
	/// <summary>
	/// Re-enables selectable layers and resets time scale to 1
	/// </summary>
	void EnableGameObjects() {
		if (!levelDone) {
			DelaySelectableLayers(0.1f, InputManager.LAYER_SELECTABLE_DEFAULT); 
		}
	}
		
	void DelaySelectableLayers(float time, LayerMask layerMask) {
		m_delayedSelectableLayers = true;
		m_timeToWait = time;
		m_maskValue = layerMask;
	}
	
	void PlaySound(AudioClip sound) {
		MusicManager.PlaySfx(sound);
	}
	
	#endregion
}
