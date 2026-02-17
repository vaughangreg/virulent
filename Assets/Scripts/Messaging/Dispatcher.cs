using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script to manage updating components when certain game events happen
/// </summary>
public class Dispatcher : System.Object
{
	public const string AUDIO_CHANGE = "AudioChange";
	public const string AUDIO_IDLE = "AudioIdle";
	public const string SET_VOLUME = "SetVolume";

	// Camera
	public const string CAMERA_FOCUS = "CameraFocus";
	public const string CAMERA_TRACKING_STOP = "CameraTrackingStop";
	public const string CAMERA_MOVE_COMPLETE = "CameraMovementComplete";

	public const string CHECK_POINT = "";
	public const string COLLISION_HAPPENS = "CollisionEvent";
	public const string COMPONENT_ADDED = "ComponentAdded";
	public const string CLICK_MOUSE = "ClickMouse";
	public const string CLICK_BUTTON = "ClickButton";

	public const string OBJECT_CREATED = "ObjectCreated";
	public const string OBJECT_DESTROYED = "ObjectDestroyed";
	public const string OBJECT_MOVED = "ObjectMoved";
	public const string OBJECTIVE_COMPLETE = "ObjectiveComplete";
	public const string OBJECT_UPGRADING = "UpgradingObject";
	public const string OBJECT_DOWNGRADING = "DowngradingObject";

	public const string RENDER_POST = "RenderPost";
	public const string UNIT_SELECT = "UnitSelect";
	public const string UNIT_PATH = "UnitPath";
	public const string MOVE_COMPLETE = "MovementComplete";

	public const string INVAGINATION_COMPLETE = "InvaginationComplete";
	public const string TIMER_COMPLETE = "TimerComplete";

	public const string PAUSE_GAME = "PauseGame";
	public const string LEVEL_START = "LevelStart";
	public const string LOAD_LEVEL = "LoadLevel";
	public const string GAME_OVER = "GameOver";

	public const string SET_SELECTABLE_LAYERS = "SetSelectableLayers";
	public const string BEACON_SIGNAL = "BeaconSignal";
	public const string BUILD_COMMAND_START = "BuildCommandStarted";
	public const string TAG_TRANSITION = "TagTransition";
	public const string VIEW_ALMANAC = "ViewAlmanac";
	public const string MULTIPLE_INPUT = "MultipleInput";
	public const string CONTINUITY_SET = "ContinuitySet";
	public const string EARLY_LEVEL_END = "EarlyLevelEnd";
	public const string POPUP = "Popup";
	public const string POPULATION_CHANGED = "PopulationChanged";
	public const string BUDDING_SITE_READY = "BuddingSiteReady";

	public const string ENERGY_RECEIVED = "EnergyRecieved";

	// Feedback
	public const string FEEDBACK_COUNT_CHANGED = "FeedbackCountChanged";

	// Time
	public const string TIME_CHANGE = "TimeChange";


	//ADAGE
	public const string FEEDBACK_DISPLAY = "FeedbackDisplay";


	private static Dictionary<string, List<GameObject>> m_listeners = new Dictionary<string, List<GameObject>> ();
	private static List<MovementLerped> m_lineDrawers = new List<MovementLerped> ();
	public static DataManager data = null;

	/// <summary>
	/// Clears all registered listeners. Should be called when a new scene is loaded, etc.
	/// </summary>
	public static void ClearAll ()
	{
		m_listeners = new Dictionary<string, List<GameObject>> ();
	}

	/// <summary>
	/// Adds a listener for a particular type of message.
	/// </summary>
	/// <param name="listenerType">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	public static void Listen (string listenerType, GameObject go)
	{
		// if there's no array for this tracking category, make a new one
		if (!m_listeners.ContainsKey (listenerType)) {
			m_listeners[listenerType] = new List<GameObject> ();
		}
		
		List<GameObject> listener = m_listeners[listenerType];
		
		// only add to the array if it isn't already being tracked
		if (!listener.Contains (go))
			listener.Add (go);
	}

	#region MovementLerped Optimizations
	public static void ListenPostRender (MovementLerped aDrawer)
	{
		m_lineDrawers.Add (aDrawer);
	}

	public static void StopListenPostRender (MovementLerped aDrawer)
	{
		m_lineDrawers.Remove (aDrawer);
	}

	protected static int SortDrawers (MovementLerped x, MovementLerped y)
	{
		bool xIsSolid = x.isSolid;
		bool yIsSolid = y.isSolid;
		if (xIsSolid && yIsSolid)
			return 0;
		// They are the same
		if (xIsSolid && !yIsSolid)
			return -1;
		// Solid is before dashed
		return 1;
		// Must be dashed.
	}

	public static void DoPostRender ()
	{
		m_lineDrawers.Sort (SortDrawers);
		
		// Do the solid lines
		MovementLerped aDrawer;
		int currentObject = 0;
		if (currentObject >= m_lineDrawers.Count)
			return;
		
		GuiResources.instance.lineMaterial.SetPass (0);
		do {
			aDrawer = m_lineDrawers[currentObject];
			if (!aDrawer.isSolid)
				break;
			
			aDrawer._OnPostRender ();
		} while (++currentObject < m_lineDrawers.Count);
		
		// Do the "dashed" lines
		if (currentObject >= m_lineDrawers.Count)
			return;
		GuiResources.instance.lineCompleteMaterial.SetPass (0);
		do {
			aDrawer = m_lineDrawers[currentObject];
			aDrawer._OnPostRender ();
		} while (++currentObject < m_lineDrawers.Count);
	}
	#endregion

	/// <summary>
	/// Register implicitly with this instead of gameObject.
	/// </summary>
	/// <param name="listenerType">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="component">
	/// A <see cref="Component"/>
	/// </param>
	public static void Listen (string listenerType, Component component)
	{
		Listen (listenerType, component.gameObject);
	}

	/// <summary>
	/// Removes a listener for the specified type of message.
	/// </summary>
	/// <param name="listenerType">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	public static void StopListen (string listenerType, GameObject go)
	{
		List<GameObject> listener = m_listeners[listenerType];
		
		if (listener != null)
			listener.Remove (go);
		m_listeners[listenerType] = listener;
	}

	/// <summary>
	/// Sends a message (calls the function denoted by methodName using value value) 
	/// to all registered listeners for the given message type.
	/// </summary>
	/// <param name="msg">
	/// A <see cref="Message"/>
	/// </param>
	public static void Send (Message msg)
	{
//		Debug.Log("Sending " + msg.functionName + " -- " + msg.ToArgumentString());
		if (data)
			data.Record (msg);
		if (!m_listeners.ContainsKey (msg.listenerType))
			return;
		
		List<GameObject> sendTo = m_listeners[msg.listenerType];
		if (sendTo != null) {
			for (int i = 0; i < sendTo.Count ; i++) {
				if (sendTo[i] != null) {
					//Debug.Log("---> " + listener.name, listener);
					sendTo[i].SendMessage (msg.functionName, msg, SendMessageOptions.DontRequireReceiver);
				}
			}
			/*foreach (GameObject listener in sendTo) {
				if (listener != null) {
					//Debug.Log("---> " + listener.name, listener);
					listener.SendMessage (msg.functionName, msg, SendMessageOptions.DontRequireReceiver);
				}
			}*/
		}
	}
}
