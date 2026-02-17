using UnityEngine;
using System.Collections;

/// <summary>
/// Attach to a prefab that will run at the start of the level before the transition activates
/// </summary>
public class LevelInfo : MonoBehaviour
{
	public string infoText;
	public Texture infoPic;
	public bool isTerminalPage = false;
	
	private LevelGUIElement playButton = new LevelGUIElement("PLAY",new Vector2(819, 440), GuiResources.feedbackButtonSize);
	private LevelGUIElement restartButton = new LevelGUIElement("CREDITS",new Vector2(819, 440), GuiResources.feedbackButtonSize);
	
	private LevelGUIElement infoPane = new LevelGUIElement("INFO",new Vector2(262,104),new Vector2(496,221));
	private LevelGUIElement picturePane = new LevelGUIElement("PIC",new Vector2(263,445),new Vector2(494,217));
	private float m_backgroundPadding = 20;
	
	private GUIStyle style;
	private GUIStyle buttonStyle;
	
	private int m_currentLength = 0;
	private int m_typeTime = 0;
	private const int TYPE_DELAY = 0;

	/// <summary>
	/// Setup all of the camera dependent locations
	/// </summary>
	void Awake()
	{	

		useGUILayout = false;
		//set the info pane padding
		infoPane.padding = m_backgroundPadding;
		Dispatcher.Listen(Dispatcher.LEVEL_START,gameObject);
		Dispatcher.Listen(Dispatcher.PAUSE_GAME, gameObject);

	}

	void Start()
	{
		buttonStyle = GuiResources.instance.buttonStyle;

	}
	
	/// <summary>
	/// Draw everything that displays information
	/// </summary>
	public void OnGUI()
	{
		//Draw all of the GUI elements
		DrawInfoPane();
		DrawPicturePane();
		DrawContinueButton();
	}
	
	#region GUI Draw Functions
	void DrawInfoPane()
	{
		GUI.Label(infoPane.GetGUIRect(), infoText.Substring(0, Mathf.Min(m_currentLength, infoText.Length)),
		          GuiResources.instance.levelInfoStyle);
		
#if UNITY_EDITOR
		if (!MusicManager.singleton) Instantiate(Resources.Load("Jukebox"));
#endif
		
		if (m_typeTime > TYPE_DELAY && m_currentLength < infoText.Length) {
			PlaySound(MusicManager.singleton.sFXMenuAdvanceText[(int)(MusicManager.singleton.sFXMenuAdvanceText.Length * Random.value)]);
			m_currentLength++;
			m_typeTime = 0;
		}
		m_typeTime++;
	}
	void DrawPicturePane()
	{
		GUI.Label(picturePane.GetGUIRect(),infoPic, GuiResources.instance.blankStyle);
	}
	
	/// <summary>
	/// Draw the continue button that unpauses the game when pressed
	/// </summary>
	/// <param name="position">
	/// A <see cref="Rect"/>
	/// </param>
	void DrawContinueButton()
	{
		if (isTerminalPage) {
			if (GUI.Button(playButton.GetGUIRect(), restartButton.text,buttonStyle)) {
				PlaySound(MusicManager.singleton.sFXMenuConfirm);
				Time.timeScale = 1.0f;
				Application.LoadLevel("Credits");
			}
		}
		//Force the button size value
		else {
			if (GUI.Button(playButton.GetGUIRect(),playButton.text,buttonStyle)) {
				PlaySound(MusicManager.singleton.sFXMenuConfirm);
				BeginLevel();
			}
		}
	}
	Rect CreateRect(Vector2 position, float width, float height)
	{
		return new Rect(position.x,position.y,width,height);
	}
	#endregion
	                    
	void PlaySound(AudioClip sound)
	{
		//MusicManager.singleton.audio.PlayOneShot(sound);
		MusicManager.PlayGuiSfx(sound);
	}
	
	
	/// <summary>
	/// Un-pause game time and allow the level to run its course
	/// </summary>
	void BeginLevel()
	{

		new MessageLevelStart();
	}
	
	/// <summary>
	/// When the level is started, destroy this component
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageLevelStart"/>
	/// </param>
	void _OnLevelStart(MessageLevelStart m)
	{
		PreLevelEnd end = new PreLevelEnd (Application.loadedLevel,Application.loadedLevelName,(float) (System.DateTime.Now - time_loaded).Duration().TotalSeconds);
		ADAGE.LogContext<PreLevelEnd> (end);
		// Debug.LogWarning("Stop!");
		new MessagePauseGame(gameObject, false);
		Destroy(this);
	}

	bool logged_start = false;
	System.DateTime time_loaded;
	void _OnPauseGame(){
		//Log Start of pre level since script is paused immediatly on load
		if (!logged_start) {
			//Log Level Start
			PreLevelStart start = new PreLevelStart (Application.loadedLevel, Application.loadedLevelName);
			ADAGE.LogContext<PreLevelStart> (start);

			time_loaded = System.DateTime.Now;
			logged_start = true;
		}

	}
}

