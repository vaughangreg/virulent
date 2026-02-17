using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseOverlayPrefab;
	
	//Pause Menu Pane
	public LevelGUIElement pauseMenuPane = new LevelGUIElement("GAME PAUSED",new Vector2(389, 121),new Vector2(246,500));
	public Texture2D pauseMenuTexture;
	
	//Buttons
	private LevelGUIElement restartButton = new LevelGUIElement("RESTART",         new Vector2(442,145),GuiResources.feedbackButtonSize);
	private LevelGUIElement levelSelectButton = new LevelGUIElement("LEVEL SELECT",new Vector2(442,145 + 60),GuiResources.feedbackButtonSize);
	private LevelGUIElement challengesButton = new LevelGUIElement("CHALLENGES",   new Vector2(442,145 + 60*2),GuiResources.feedbackButtonSize);
	private LevelGUIElement almanacButton = new LevelGUIElement("ALMANAC",         new Vector2(442,145 + 60*3),GuiResources.feedbackButtonSize);
	private LevelGUIElement sfxButton = new LevelGUIElement("SFX ON",              new Vector2(442,145 + 60*4),GuiResources.feedbackButtonSize);
	private LevelGUIElement musicButton = new LevelGUIElement("MUSIC ON",          new Vector2(442,145 + 60*5),GuiResources.feedbackButtonSize);
	private LevelGUIElement resumeButton = new LevelGUIElement("RESUME",           new Vector2(442,145 + 60*6),GuiResources.feedbackButtonSize);
	
	
	[HideInInspector]public bool drawGUI {get {return !Almanac.almanacIsOpen;}}
	
	private Vector3 m_cameraPosition;
	private GUIStyle buttonStyle;
	private GUIStyle soundButtonStyle;
	private GUIStyle musicButtonStyle;
	
	private GameObject pauseOverlay;
	protected DataManager m_dataManager;
	
	private bool m_soundIsOn { get { if (MusicManager.GetSfxVolume() > 0) return true; else return false; }}
	private bool m_musicIsOn { get { if (MusicManager.GetMusicVolume() > 0) return true; else return false; }}
	private bool m_challengesUnlocked;
	
	
	void Awake()
	{
		useGUILayout = false;
		m_cameraPosition = Camera.main.transform.position;
		m_dataManager = Camera.main.GetComponent<DataManager>();
		useGUILayout = false;
		if (PlayerPrefs.GetInt(Constants.LEVELS[Constants.MIN_LEVEL_FOR_CHALLENGES]) == 0) {
			m_challengesUnlocked = false; 
		}
		else {
			m_challengesUnlocked = true;
		}
	}
	
	void Start()
	{
		//Pause the game
		new MessagePauseGame(gameObject,true);
		//Get the button style
		buttonStyle = GuiResources.instance.buttonStyle;
		soundButtonStyle = new GUIStyle(buttonStyle);
		soundButtonStyle.normal.textColor = Color.green;
		soundButtonStyle.hover.textColor = Color.green;
		soundButtonStyle.onHover.textColor = Color.green;
		soundButtonStyle.onNormal.textColor = Color.green;
		
		musicButtonStyle = new GUIStyle(soundButtonStyle);
		
		//Setup all of the elements
		//pauseMenuPane.position = new Vector2(Camera.mainCamera.pixelWidth/2 - pauseMenuPane.size.x/2,112);
		
		//Scale and position a Pause Overlay
		pauseOverlay = Instantiate(pauseOverlayPrefab) as GameObject;
		pauseOverlay.transform.position = m_cameraPosition + new Vector3(0,2,0);
		pauseOverlay.transform.localScale = Vector3.one * (Camera.main.orthographicSize * 15.0f);
		
		//Double check the music and sound states
		if (!m_musicIsOn) { 
			musicButton.text = "MUSIC OFF";
			musicButtonStyle.normal.textColor = Color.red;
			musicButtonStyle.hover.textColor = Color.red;
			musicButtonStyle.active.textColor = Color.red;
		}
		if (!m_soundIsOn) {
			sfxButton.text = "SOUND OFF";
			soundButtonStyle.normal.textColor = Color.red;
			soundButtonStyle.hover.textColor = Color.red;
			soundButtonStyle.active.textColor = Color.red;
		}
	}
	
	void OnGUI()
	{
		if (drawGUI)
		{
			//Draw the pane first
			GUI.Label(pauseMenuPane.GetGUIRect(),pauseMenuTexture);
			//Draw the buttons
			if (GUI.Button(almanacButton.GetGUIRect(),    almanacButton.text,buttonStyle))     ViewAlmanac();
			
			GUI.enabled = m_challengesUnlocked;
			if (GUI.Button(challengesButton.GetGUIRect(), challengesButton.text,buttonStyle))  GotoChallenges();
			GUI.enabled = true;
			
			if (GUI.Button(resumeButton.GetGUIRect(),     resumeButton.text,buttonStyle))      ResumeGame();
			if (GUI.Button(restartButton.GetGUIRect(),    restartButton.text,buttonStyle))     RestartLevel();
			if (GUI.Button(levelSelectButton.GetGUIRect(),levelSelectButton.text,buttonStyle)) GotoLevelSelect();
			
			//Handle the special color states of the Sound and Music buttons
			
			//Draw the Music button
			if (GUI.Button(musicButton.GetGUIRect(),      musicButton.text,musicButtonStyle))       SetMusic();
			if (GUI.Button(sfxButton.GetGUIRect(),        sfxButton.text,  soundButtonStyle))       SetSFX();
			
			if (Debug.isDebugBuild) {
				GUI.color = Color.red;
				GUI.Label(new Rect(32.0f, Screen.height - 32.0f, Screen.width, 32.0f), m_dataManager.guid);
			}
		}
		
		if (Debug.isDebugBuild) {
			GUI.color = Color.red;
			GUI.Label(new Rect(Screen.width - 132.0f, Screen.height - 32.0f, 100.0f, 32.0f), Application.loadedLevelName);
		}
	}
	
	void ViewAlmanac()
	{
		new MessageViewAlmanac(gameObject,Almanac.Views.Category);
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
	}
	
	void ResumeGame()
	{
		PlaySound(MusicManager.singleton.sFXMenuDeny);
		Destroy(gameObject);
	}
	void GotoChallenges()
	{
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
		new MessageLoadLevel("ChallengeSelect");
		Destroy(gameObject);
	}
	void RestartLevel()
	{
		new MessageGameOver(gameObject,false);
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
		Destroy(gameObject);
	}
	/// <summary>
	/// Toggle the sound on or off
	/// </summary>
	void SetSFX()
	{
		if (m_soundIsOn) {
			new MessageSetVolume(gameObject, 0.0f, MusicManager.VolumeType.SFX);
			sfxButton.text = "SFX OFF";
			soundButtonStyle.normal.textColor = Color.red;
			soundButtonStyle.onNormal.textColor = Color.red;
			soundButtonStyle.onHover.textColor = Color.red;
			soundButtonStyle.hover.textColor = Color.red;
			soundButtonStyle.active.textColor = Color.red;
		}
		else {
			new MessageSetVolume(gameObject, 1.0f, MusicManager.VolumeType.SFX);
			sfxButton.text = "SFX ON";
			soundButtonStyle.normal.textColor = Color.green;
			soundButtonStyle.onNormal.textColor = Color.green;
			soundButtonStyle.onHover.textColor = Color.green;
			soundButtonStyle.hover.textColor = Color.green;
			soundButtonStyle.active.textColor = Color.green;
		}
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
		print("SFX Volume: " + MusicManager.GetSfxVolume());
	}
	
	/// <summary>
	/// Toggle the music on or off
	/// </summary>
	void SetMusic()
	{
		if (m_musicIsOn) {
			new MessageSetVolume(gameObject,0.0f,MusicManager.VolumeType.Music);
			musicButton.text = "MUSIC OFF";
			musicButtonStyle.normal.textColor = Color.red;
			musicButtonStyle.onNormal.textColor = Color.red;
			musicButtonStyle.onHover.textColor = Color.red;
			musicButtonStyle.hover.textColor = Color.red;
			musicButtonStyle.active.textColor = Color.red;
		}
		else {
			new MessageSetVolume(gameObject, 1.0f,MusicManager.VolumeType.Music);
			musicButton.text = "MUSIC ON";
			musicButtonStyle.normal.textColor = Color.green;
			musicButtonStyle.onNormal.textColor = Color.green;
			musicButtonStyle.onHover.textColor = Color.green;
			musicButtonStyle.hover.textColor = Color.green;
			musicButtonStyle.active.textColor = Color.green;
		}
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
	}
	void GotoLevelSelect()
	{
		PlaySound(MusicManager.singleton.sFXMenuConfirm);
		new MessageLoadLevel("Level Select");
		Destroy(gameObject);
	}
	
	/// <summary>
	/// Get rid of extra objects
	/// </summary>
	void OnDestroy()
	{
		Destroy(pauseOverlay);
		new MessagePauseGame(gameObject, false);
	}
	
	void PlaySound(AudioClip sound)
	{
		MusicManager.PlayGuiSfx(sound);
	}
}

