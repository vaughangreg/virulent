using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {
	public Texture2D pauseSymbol;
	public Texture2D playSymbol;
	public GameObject pauseMenuPrefab;
	public LevelGUIElement pauseButtonGUI = 
		new LevelGUIElement("", new Vector2(1024 - 42, 3f), new Vector2(37, 37));
	
	private GameObject pauseMenu;
	protected float m_alpha = 0.0f;
	
	private bool isGamePaused { get { return Time.timeScale <= Constants.TIME_PAUSE_THRESHOLD; } }
	
	void Awake() {
		useGUILayout = false;
	}
	
	void OnGUI()
	{
		Color color = GUI.color;
		color.a = Mathf.Min(1.0f, m_alpha);
		GUI.color = color;
		if (!isGamePaused) {
			if (GUI.Button(pauseButtonGUI.GetGUIRect(),pauseSymbol,GuiResources.instance.blankStyle)) {
				PauseGame();
			}
		}
		else {
			if (GUI.Button(pauseButtonGUI.GetGUIRect(),playSymbol,GuiResources.instance.blankStyle)) {
				UnpauseGame();
			}
		}
		m_alpha += Time.deltaTime;
	}
	
	void PauseGame()
	{
		PauseEvent pause = new PauseEvent ();
		ADAGE.LogData<PauseEvent>(pause);

		MusicManager.PlayGuiSfx(MusicManager.singleton.sFXMenuConfirm);
		pauseMenu = Instantiate(pauseMenuPrefab) as GameObject;
	}
	
	void UnpauseGame()
	{
		ResumeEvent resume = new ResumeEvent ();
		ADAGE.LogData<ResumeEvent>(resume);

		MusicManager.PlayGuiSfx(MusicManager.singleton.sFXMenuDeny);
		if (pauseMenu) Destroy(pauseMenu); 
		else new MessagePauseGame(gameObject, false);
	}
}

