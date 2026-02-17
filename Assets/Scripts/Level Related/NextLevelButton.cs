using UnityEngine;

public class NextLevelButton : MonoBehaviour
{
	public float x_offset = 160;
	public float z = 3;
	public float width = 110;
	public float height = 37;
	
	public float colorStrength;
	
	private LevelGUIElement m_button;
	private bool m_isActive = false;
	
	void Awake()
	{
		useGUILayout = false;
		m_button = new LevelGUIElement("Next Level", new Vector2(1024 - x_offset, z), new Vector2(width, height));
	}
	
	void Start() {
		Dispatcher.Listen(Dispatcher.EARLY_LEVEL_END, gameObject);
	}
	
	void OnGUI()
	{
		GUI.color = new Color(1, Mathf.PingPong(Time.time, 1) + colorStrength, Mathf.PingPong(Time.time, 1) + colorStrength, 1);
		if (m_isActive) {
			if (GUI.Button(m_button.GetGUIRect(), m_button.text, GuiResources.instance.wideBlankStyle)) {
				new MessageGameOver(gameObject, true);
			}
		}
	}
	
	void _OnEarlyLevelEnd(Message e) {
		m_isActive = true;
		new MessagePopup(gameObject, PopupManager.PresetIDs.Early_End, 5f);
	}
}

