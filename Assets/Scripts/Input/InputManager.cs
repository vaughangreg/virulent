using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles all the input from the user.
/// </summary>
public class InputManager : MonoBehaviour {
	public GameObject selection        = null;
	public GameObject pendingSelection = null;
	public GameObject selectionIndicator;
	
	public bool         hasSelection    = false;
#if UNITY_WEBPLAYER
	public static float epsilon         = 22.5f;	// in pixels
	public float        fatFingerRadius = 16.0f; // in pixels
#else
	public static float epsilon         = 42.5f;	// in pixels
	public float        fatFingerRadius = 16.0f; // in pixels 
#endif
	
	public AudioClip selectFx;
	public AudioClip slideFx;
	public AudioClip multipleInputFX;
	
	public string multipleInputMessage;
	
	// Need to ignore IgnoreRaycast, Walls
	public const int        LAYER_SELECTABLE_DEFAULT = Physics.DefaultRaycastLayers;
	private      int        m_selectableLayers       = LAYER_SELECTABLE_DEFAULT;
	private      GameObject m_lastSelected;
	
	private      bool       m_displayMultiSelectWarning = false;
	private      float      m_multipleInputCount;
	private      float      m_multipleInputDuration = 2.0f;
	
	List<IRecognizer> m_recognizers = new List<IRecognizer>();
	
	delegate void RecognizerDelegate(IRecognizer aRecognizer);
	public delegate GameObject InputRaycastDelegate(Vector3 aPosition);
	
	public InputRaycastDelegate ObjectAtScreen;
	Dictionary<IRecognizer, RecognizerDelegate> m_recognizerDelegates = new Dictionary<IRecognizer, RecognizerDelegate>();

	/// <summary>
	/// Returns true when the mouse is down or a touch is detected.
	/// </summary>
	/// <value>
	/// <c>true</c> if is touching; otherwise, <c>false</c>.
	/// </value>
	protected bool isTouching {
		get { return  Input.GetMouseButton(0) || Input.touchCount > 0; }
	}
	
	/// <summary>
	/// Position of the mouse or finger.
	/// </summary>
	Vector3 m_lastTouchPosition = Vector3.zero;
	public Vector3 position {
		get {	
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				if (Input.touchCount > 0) {
					Touch finger = Input.GetTouch(0);
					m_lastTouchPosition = finger.position;
					return m_lastTouchPosition;
				} else return m_lastTouchPosition;
			}
			else {
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.x = Mathf.Max(0.0f, mousePosition.x);
				mousePosition.x = Mathf.Min(Screen.width, mousePosition.x);
				mousePosition.y = Mathf.Max(0.0f, mousePosition.y);
				mousePosition.y = Mathf.Min(Screen.height, mousePosition.y);
				return mousePosition;
			}
		}
	}
	
	/// <summary>
	/// Creates the recognizers.
	/// </summary>
	void Awake() {		
		useGUILayout = false;
		
		IRecognizer recognizer = new RecognizerTap();
		recognizer.epsilon = epsilon;
		m_recognizers.Add(recognizer);
		m_recognizerDelegates.Add(recognizer, OnTap);
		
		recognizer = new RecognizerSlide();
		recognizer.epsilon = epsilon;
		m_recognizers.Add(recognizer);
		m_recognizerDelegates.Add(recognizer, OnSlide);
		
		recognizer = new RecognizerTouchDown();
		m_recognizers.Add(recognizer);
		m_recognizerDelegates.Add(recognizer, OnTouch);
		
		if (Debug.isDebugBuild) {
			recognizer = new RecognizerEmailData();
			m_recognizers.Add(recognizer);
		}
		
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			ObjectAtScreen = IosObjectAtScreen;
			if (Debug.isDebugBuild) {
				recognizer = new RecognizerLevelNext();
				m_recognizers.Add(recognizer);
			}
			
			recognizer = new RecognizerMultiInputWarning();
			m_recognizers.Add(recognizer);
			m_recognizerDelegates.Add(recognizer, OnMultipleInput);
			
			Input.multiTouchEnabled = true;
		}
		else ObjectAtScreen = DesktopObjectAtScreen;
		
		Selectable.selector = (GameObject)Instantiate(selectionIndicator);
		Selectable.selector.SetActiveRecursively(false);
		
		//Listen for the OnSetSelectableLayers Message
		Dispatcher.Listen(Dispatcher.SET_SELECTABLE_LAYERS, gameObject);
		Dispatcher.Listen(Dispatcher.GAME_OVER, gameObject);
	}
	
	/// <summary>
	/// Passes recognizers current input state.
	/// </summary>
	void Update() {
		bool touchState = isTouching;
		Vector3 positionState = position;
		bool isRecognizerActive = false;
		
		// Note that we need to process each recognizers so they can 
		// cancel state when needed.
		foreach (IRecognizer aRecognizer in m_recognizers) { 
			isRecognizerActive = aRecognizer.ProcessState(touchState, positionState); 
			if (isRecognizerActive) m_recognizerDelegates[aRecognizer](aRecognizer);
		}
	}
	
	/// <summary>
	/// Display multitouch warning.
	/// </summary>
	void OnGUI() {
		if (m_displayMultiSelectWarning) {
			m_multipleInputCount += Time.deltaTime;
			
			GUI.color = Color.red;
			GUI.Label(Selectable.m_nameRect, multipleInputMessage);
			
			if (m_multipleInputCount > m_multipleInputDuration) {
				m_displayMultiSelectWarning = false;
				m_multipleInputCount = 0f;
			}
		}
	}
	
	#region Used by other objects
	/// <summary>
	/// Selects the specified game object.
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	public void Select(GameObject go) {
		ClearSelectionIfNeeded();
		selection = go;
		if (selectFx) MusicManager.PlaySfx(selectFx);
	}
	
	public void ClearIfSelected(GameObject go) {
		//Debug.Log(go.name + " was deselected");
		if (selection == go) {
			selection.SendMessage("OnDeselect", this, SendMessageOptions.DontRequireReceiver);
			selection = null;
			if (m_lastSelected != null) {
				selection = m_lastSelected;
				m_lastSelected = null;
			}
		}
	}
	
	/// <summary>
	/// Clears the currently selected object if one is selected.
	/// </summary>
	public void ClearSelectionIfNeeded() {
		if (selection != null) {
			selection.SendMessage("OnDeselect", this, SendMessageOptions.DontRequireReceiver);
			selection = null;
			if (m_lastSelected != null) {
				selection = m_lastSelected;
				m_lastSelected = null;
			}
		}
	}
	
	/// <summary>
	/// Plays the drag sound.
	/// </summary>
	public void PlayDragSound() {
		if (slideFx) MusicManager.PlayGuiSfx(slideFx);	
	}
	#endregion
	
	protected GameObject m_touchedObject;
	/// <summary>
	/// Opens up the GUI if possible and selects the object.
	/// </summary>
	/// <param name='aRecognizer'>
	/// A recognizer.
	/// </param>
	void OnTap(IRecognizer aRecognizer) {
		new MessageClickMouse(position);
		GameObject go = ObjectAtScreen(aRecognizer.touchDownPosition);
		
		if (go) {
			go.SendMessage("OnTap", this, SendMessageOptions.DontRequireReceiver);
		}
		else ClearSelectionIfNeeded();
	}
	
	/// <summary>
	/// Sends an OnTouch message to the object below.
	/// </summary>
	/// <param name="aRecognizer">
	/// A <see cref="IRecognizer"/>
	/// </param>
	void OnTouch(IRecognizer aRecognizer) {
		GameObject go = ObjectAtScreen(aRecognizer.touchDownPosition);
		
		if (go) {
			m_touchedObject = go;
			go.SendMessage("OnTouch", this, SendMessageOptions.DontRequireReceiver);
		}
		else { // No object; we're done.
			m_touchedObject = null;
			ClearSelectionIfNeeded(); 
			return;	
		}
	}
	
	/// <summary>
	/// Defines a movement path.
	/// </summary>
	/// <param name='aRecognizer'>
	/// A recognizer.
	/// </param>
	void OnSlide(IRecognizer aRecognizer) {
		RecognizerSlide slider = (RecognizerSlide)aRecognizer;
		
		if (slider.isFirstRecognition) {
			// Try the original location first, then the current location.
			GameObject go = m_touchedObject; 
			if (!go) { // No object; we're done.
				ClearSelectionIfNeeded(); 
				return;	
			}
			
			if (selection != go) {
				if (selection != null && !go.transform.IsChildOf(selection.transform)) {
					ClearSelectionIfNeeded();
				}
				else if (selection != null && go.transform.IsChildOf(selection.transform)) {
					m_lastSelected = selection;
				} 
				selection = go;
				// Debug.Log(selection.name + "  " + go.transform.IsChildOf(selection.transform));
			}
			
			selection.SendMessage("OnSlideStart", this, SendMessageOptions.DontRequireReceiver);
		}
		else if (selection && slider.isLastRecognition) {
			selection.SendMessage("OnSlideStop", this, SendMessageOptions.DontRequireReceiver);
			ClearSelectionIfNeeded();
		}
		else if (selection) {
			selection.SendMessage("OnSlideStay", this, SendMessageOptions.DontRequireReceiver);
		}
		else ClearSelectionIfNeeded();
	}
	
	void OnMultipleInput(IRecognizer aRecognizer) {
		new MessageMultipleInput(gameObject);
		if (!m_displayMultiSelectWarning && multipleInputFX) {
			MusicManager.PlayGuiSfx(multipleInputFX);
		}
		m_displayMultiSelectWarning = true;
		m_multipleInputCount = 0f;
	}
	
	/// <summary>
	/// Set the selectable layers to the recieved message's layers integer
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageSetSelectableLayers"/>
	/// </param>
	void _OnSetSelectableLayers(MessageSetSelectableLayers m)
	{
		m_selectableLayers = m.layers;
	}
	
	void _OnGameOver(MessageGameOver m) {
		ClearSelectionIfNeeded();	
	}
	
	#region Utility methods
	/// <summary>
	///   Returns the first object hit by a raycast.
	/// </summary>
	public GameObject DesktopObjectAtScreen(Vector3 aPosition)
	{
		RaycastHit aHit;
		Ray aRay = Camera.main.ScreenPointToRay(aPosition);
		
		// We shouldn't need to SelectBestHit because the selection precision
		// should be better.
		if (Physics.Raycast(aRay, out aHit, Mathf.Infinity, (LayerMask)m_selectableLayers)) {
			GameObject target = aHit.collider.gameObject;
			Selectable selector = target.GetComponent<Selectable>();
			
			if (selector != null && selector.isSelectable) return target;
		}
		return null;
	}
	
	/// <summary>
	///   Returns the first object hit by a spherecast.
	/// </summary>
	public GameObject IosObjectAtScreen(Vector3 aPosition) {
		Ray aRay = Camera.main.ScreenPointToRay(aPosition);
		aRay.origin -= aRay.direction * fatFingerRadius;
		
		RaycastHit[] hits = Physics.SphereCastAll(aRay, fatFingerRadius, Mathf.Infinity, m_selectableLayers);
		return SelectBestHit(hits);
	}
	
	/// <summary> 
	/// We want to select the object that has the highest Z coordinate
	/// that is not on the Walls layer; if there's nothing else, then
	/// we'll select the Walls layer.
	/// </summary>
	/// <param name="aHitArray">
	/// A <see cref="RaycastHit[]"/>
	/// </param>
	/// <returns>
	/// A <see cref="GameObject"/>
	/// </returns>
	protected GameObject SelectBestHit(RaycastHit[] aHitArray) {
		GameObject highestNonWallObject = null;
		GameObject highestWallObject    = null;
		float      highestNonWall       = float.MinValue;
		float      highestWall          = float.MinValue;
		
		foreach (RaycastHit aHit in aHitArray) {
			GameObject aPotentialTarget = aHit.collider.gameObject;
			Selectable aSelector = aPotentialTarget.GetComponent<Selectable>();
			if (aSelector == null || !aSelector.isSelectable) continue;
			float testPositionZ = aPotentialTarget.transform.position.z;
			
			switch (aPotentialTarget.layer) {
				case 8: // Walls
					if (testPositionZ > highestWall) highestWallObject = aPotentialTarget;
					break;
				default:
					if (testPositionZ > highestNonWall) highestNonWallObject = aPotentialTarget;
					break;
			}
		}
		
		return (highestNonWallObject != null) ? highestNonWallObject : highestWallObject;
	}

	/// <summary>
	///   Adjusts Y to be 0.
	/// </summary>
	public Vector3 ScreenToWorld(Vector3 source) {
		source.z = Camera.main.nearClipPlane + 0.01f;
		Vector3 result = Camera.main.ScreenToWorldPoint(source);
		result.y = 0f;
		return result;
	}
	#endregion
}
