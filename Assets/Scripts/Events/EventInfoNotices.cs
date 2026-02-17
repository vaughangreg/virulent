using UnityEngine;

/// <summary>
/// Super simple GUI setup to post the latest CheckPoint messages to a bar across the top of the screen.
/// </summary>	
public class EventInfoNotices : MonoBehaviour {
	public string latestNotice;
	
	//private Rect m_messageRect = new Rect(31.0f, 6.0f, 315.0f, 35.0f);
	private LevelGUIElement myTextBox = new LevelGUIElement("", new Vector2(31f, 6), new Vector2(315f, 35f));
	
	/// <summary>
	/// Begins listening for check point events.
	/// </summary>
	void Start() {
		useGUILayout = false;
		Dispatcher.Listen(Dispatcher.CHECK_POINT, gameObject);
		enabled = false;
	}
	
	/// <summary>
	/// Draws the current goal text.
	/// </summary>
	void OnGUI() {
		GUI.Label(myTextBox.GetGUIRect(), latestNotice, GuiResources.instance.goalStyle);
	}
	
	/// <summary>
	/// Handler for checkpoint events.
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageCheckPoint"/>
	/// </param>
	void _OnCheckPoint(MessageCheckPoint e) {
		if (e.checkPointString != null && e.checkPointString != "") {
			latestNotice = e.checkPointString;
			enabled = (latestNotice.Length > 0);
		}
		if (e.checkPointAudio != null) {
			MusicManager.PlayGoalSfx(e.checkPointAudio);
		}	
	}
	
	void OnDestroy() {
		//Remove 'Selectable' components from gameObjects
		GameObject[] obs = (GameObject[]) Object.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in obs) {
			Selectable sel = go.GetComponent<Selectable>();
			if (sel) sel.displayTag = false;
		}
	}
}
