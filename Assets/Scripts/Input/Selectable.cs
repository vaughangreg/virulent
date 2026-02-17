using UnityEngine;

/// <summary>
/// Allows players to interact with this object.
/// </summary>
public class Selectable : MonoBehaviour {
	public bool isSelectable		= true;
	public float distanceInPixels	= 100.0f;
	public bool displayTag			= true;
	public bool ignoreTap			= false;
	public AudioClip[] selectionClips;
	public string messageDisplay	= null;
	
	public PopupManager.PresetIDs popupPreset = PopupManager.PresetIDs.EMPTY;
	
	public static GameObject selector;
	public TerrainDeformer deformer;
	public bool handleDeformerEnabled = true;
	public bool handleDeformerFrequency = false;
	public float deformerChange = 2.0f;
	public float frequencyChange = 6.0f;
	
	//protected Rect m_nameRect;
	protected bool m_isSelected		 = false;
	private LevelGUIElement m_selectableRect = new LevelGUIElement("", new Vector2(409f, 4f), new Vector2(202f, 35f));
	public static Rect m_nameRect = new Rect(409.0f, 4.0f, 202.0f, 35.0f);
	protected float m_deformerAmplitude;
	protected float m_deformerFrequency;
	
	protected float m_yPosition;
	
	void Awake() {
//#if UNITY_WEBPLAYER
		if (deformerChange > 7f) deformerChange = 7f;
//#endif
		useGUILayout = false;
		Dispatcher.Listen(Dispatcher.MULTIPLE_INPUT, gameObject);
		if (deformer == null) deformer = GetComponent<TerrainDeformer>();
		if (deformer) {
			m_deformerAmplitude = deformer.amplitude;
			m_deformerFrequency = deformer.frequency;
		}
		m_yPosition = Camera.main.transform.position.y - Camera.main.nearClipPlane - 0.5f;
		enabled = false;
	}
	
	#region Selection Handling
	protected void ShowSelector() {
		selector.transform.parent = transform;
		Vector3 newPosition = new Vector3(transform.position.x,
		                                  m_yPosition,
		                                  transform.position.z);
		newPosition = GuiResources.ClampWorldToScreen(newPosition);
		selector.transform.position = newPosition;
		
		selector.SetActiveRecursively(true);
		if (deformer) {
			deformer.amplitude = m_deformerAmplitude * deformerChange;
			if (handleDeformerEnabled) deformer.enabled = true;
			if (handleDeformerFrequency) deformer.frequency = m_deformerFrequency * frequencyChange;
		}
	}
	
	/// <summary>
	/// Our current selection state.
	/// </summary>
	public bool isSelected 
	{
		get { return m_isSelected; }
		set {
			m_isSelected = value;
			enabled = value;
			if (m_isSelected) {
				displayTag = true;
				ShowSelector();
				if (selectionClips.Length > 0) {
					int clipIndex = (int)(Random.value * selectionClips.Length);

					Debug.Log(selectionClips[clipIndex]);
					MusicManager.PlaySfx(selectionClips[clipIndex]);
				}
				if (popupPreset != PopupManager.PresetIDs.EMPTY) {
					new MessageUnitSelect(gameObject,popupPreset);
				}
				else new MessageUnitSelect(gameObject);
			}
			else {
				if (selector) selector.SetActiveRecursively(false);
			}
			//Debug.Log("" + gameObject.name + " was " + (m_isSelected ? "" : "DE") + "selected.", gameObject);
		}
	}
	
	/// <summary>
	/// Select the object without sending a selection message
	/// or playing an audio clip.
	/// </summary>
	public void SilentlySelect() {
		ShowSelector();
		gameObject.layer = LayerMask.NameToLayer("Default");
		m_isSelected = true;
		enabled = true;
	}
	#endregion
	
	#region InputManager messages
	
	/// <summary>
	/// Clears the selection if we're not ignoring it.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	public void OnTap(InputManager source) {
		if (!ignoreTap) {
			// We need to deselect the first object 
			// before selecting this one; otherwise,
			// the selection ring won't work.
			if (!isSelected) {
				source.Select(gameObject);
				isSelected = true;
			}
			else {
				source.ClearSelectionIfNeeded();
			}
		}
	}
	
	/// <summary>
	/// Deselect this if needed.  Also sets to the selector parent to null in case it dies before something else is selected
	/// 
	/// </summary>
	public void OnDeselect() {
		if (ignoreTap) return;
		if (isSelected) {
			isSelected = false;
			HideSelector();
		}
	}
	
	public void HideSelector() {
		selector.transform.parent = null;
		selector.transform.position = Vector3.zero;
		selector.SetActiveRecursively(false);
		if (deformer) {
			deformer.amplitude = m_deformerAmplitude;
			if (handleDeformerEnabled) deformer.enabled = false;
			if (handleDeformerFrequency) deformer.frequency = m_deformerFrequency;
		}
	}
	#endregion
	
	#region GUI
	/// <summary>
	/// Draws the tag if selected.
	/// </summary>
	void OnGUI() {
		if (!m_isSelected || !displayTag) return;
		
		//Vector3 screenPosition = GuiResources.instance.GetScreenPosition(transform.position, distanceInPixels);
		
		//Rect nameRect = GuiResources.instance.CenterAtGuiPoint(m_nameRect, screenPosition);
		GUI.Label(m_selectableRect.GetGUIRect(), messageDisplay != "" ? messageDisplay : tag,
		          GuiResources.instance.commandTooltipStyle);
	}
	
	void _OnMultipleInput(MessageMultipleInput e) {
		displayTag = false;
	}
	
	#endregion
	
	/// <summary>
	/// Raises the disable event.  Sends it to the OnDestroy event, this way the selection ring is never destroyed or disabled with an object, it is always unparented first
	/// </summary>
	public void OnDisable() {
		OnDestroy();
	}
	
	/// <summary>
	/// If the object is destroyed while selected, it sets the selector to have a null parent and deselects itself
	/// </summary>
	public void OnDestroy() {
		if (selector && selector.transform.parent == transform) {
			selector.transform.parent = null;
			selector.transform.position = Vector3.zero;
			selector.SetActiveRecursively(false); 
		}
	}
}
