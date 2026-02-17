using UnityEngine;

/// <summary>
/// Arranges buttons & labels for level selection.
/// </summary>
public class LevelSelect : MonoBehaviour {
	public int playerProgression = 3;	//how far the player has progressed in the game.
	public float buttonDepth = -19f;
	public GameObject TextPrefab;		//3DText prefab
	public GameObject capsid;			//Capsid prefab
	public GameObject capsidDummy;		//CapsidDummy prefab
	public GameObject transition;		//Transition prefab
	public bool isThisTheChallengeSelectScreen = false;

	public GameObject[] buttons;		//Instances of the LevelButton prefab
	//public string[] labels;				//Title to display for levels
	//public Material[] materials;		//Materials for each of the buttons
	public Material unlockedChallengesMaterial;
	public Vector3[] newButtonPositions;
	//public string[] postText;
	//public string[] postTextSuffix;
	public float zButtonOffset = 30f;
	public Vector3 buttonTextOffset = new Vector3(0, 0f, 40f);
	public float arrangementAngleOffset = 0f;
	public float ovalWidth = 300f;
	public float ovalHeight = 225f;
	public float myTextSize = 12f;
	
	public float aEllipse = 1f;
	public float bEllipse = 1f;
	public float nEllipse = 4f;
	public float[] buttonAngles;

	public AudioClip levelSelectSFX;

	private GameObject m_dummy = null;
	private GameObject m_selectedLevelButton;
	private string m_selectedLevel;
//	private string[] subText;

	void Start() {
//		subText = new string[labels.Length];
		CalculateButtonPositions();
		capsid.transform.position = new Vector3(0f, 0f, -zButtonOffset);
		SetUpButtons();
		OpenTransition();
		
		Dispatcher.Listen(Dispatcher.MOVE_COMPLETE, gameObject);

	}

	/// <summary>
	/// Sets the positions, color (based upon how progressed the player is) and text for all buttons.  
	/// Now places text based on new button positions and button text offset.
	/// </summary>
	void SetUpButtons() {
		PlayerPrefs.SetInt(Constants.LEVELS[0], 1);
		for (int i = 0; i < buttons.Length; ++i) {
			DisplayButton(i, buttons[i]);	
		}
	}
	
	void DisplayButton(int aButtonNumber, GameObject aButton) {
		aButton.transform.position = newButtonPositions[aButtonNumber] + new Vector3(0, buttonDepth, 0);
		LevelButton buttonInfo     = aButton.GetComponent<LevelButton>();

		// If they haven't reached the min level, then disable the button.
		int minLevelRequired = PlayerPrefs.GetInt(Constants.LEVELS[buttonInfo.minLevelReached], 0);
		if (minLevelRequired == 0) {
			buttonInfo.amActive = false;
			aButton.GetComponent<Renderer>().sharedMaterial.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
		}
		else buttonInfo.amActive = true;
		
		int amount = 0;
		switch (buttonInfo.displayValue) {
			case ButtonDisplayType.CountUnitBest:
				amount = PlayerPrefs.GetInt("Continuity_" + aButton.name + "_bestUnitCount", 0);
				break;
			case ButtonDisplayType.CountGeneral:
				amount = PlayerPrefs.GetInt("Count" + aButton.name, 0);
				break;
			case ButtonDisplayType.TimeLongest:
				amount = PlayerPrefs.GetInt("LongestTime " + aButton.name, 0);
				break;
		}
		
		// Create the text labels
		string postText = string.Format(buttonInfo.textInfo, amount, (amount == 1) ? "" : "s");
		CreateText(newButtonPositions[aButtonNumber] + buttonTextOffset, buttonInfo.textLabel);
		CreateText(newButtonPositions[aButtonNumber] - Vector3.Scale(buttonTextOffset, new Vector3(1, -1, 1)), postText);
		
		if (aButtonNumber == 0 && !isThisTheChallengeSelectScreen) {
			bool areChallengesUnlocked = PlayerPrefs.GetInt(Constants.LEVELS[Constants.MIN_LEVEL_FOR_CHALLENGES], 0) != 0;
			if (areChallengesUnlocked) aButton.GetComponent<Renderer>().sharedMaterial = unlockedChallengesMaterial;
		}
	}

	void CreateText(Vector3 textPosition, string incomingString) {
		GameObject go = Instantiate(TextPrefab, textPosition, Quaternion.LookRotation(Vector3.down)) as GameObject;
		TextMesh myTextMesh = go.GetComponent<TextMesh>();
		myTextMesh.text = incomingString;
		myTextMesh.anchor = TextAnchor.MiddleCenter;
		myTextMesh.characterSize = myTextSize;
	}

	/// <summary>
	/// Creates new transition and opens it.
	/// </summary>
	void OpenTransition() { }
	
	/// <summary>
	/// Selects the level associated with the button that sent the message,
	/// destroys the capsid,
	/// creates a dummy replacement and sends it spinning into the level picture,
	/// delays a level load for one second.
	/// </summary>
	/// <param name="sendingButton">
	/// A <see cref="GameObject"/>
	/// </param>
	void Select(GameObject sendingButton) {
		//Debug.Log(sendingButton.name + " selected");
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons[i] == sendingButton) {
				m_selectedLevelButton = buttons[i];
				
				Destroy(capsid);
				Destroy(GameObject.Find("HUD"));
				CreateSpinningDummy();
				
				//Play the sound effect
				MusicManager.PlayGuiSfx(levelSelectSFX);
				
				CloseTransition();
				
				//set the level selected for use in DelayLoad
				if (!isThisTheChallengeSelectScreen){
					m_selectedLevel = Constants.LEVELS[i];
				}else{ 
					m_selectedLevel = sendingButton.name;
				//	m_selectedLevel = Constants.CHALLENGE_LEVELS[i];
				}
				//invoke delyaload with a delay long enough for the transition to snap shut.
				// NOTE: This is bad––we should be calling this after hearing a message
				// instead of an arbitrary time.
			}
		}
	}

	void _OnMovementComplete(MessageMovementComplete e) {
		new MessageLoadLevel(m_selectedLevel);
	}

	/// <summary>
	/// Creates transition and closes it over the level.
	/// </summary>
	void CloseTransition() {
		GameObject trans = Instantiate(transition) as GameObject;
		trans.GetComponent<Transition>().destroyAtEnd = true;
		trans.GetComponent<Transition>().startInView = false;
		new MessageAudioRequest(gameObject, MusicManager.State.FadeOut);
	}

	/// <summary>
	/// creates the dummy objects and applies torque.
	/// </summary>
	void CreateSpinningDummy() {
		m_dummy = Instantiate(capsidDummy, capsid.transform.position, capsid.transform.rotation) as GameObject;
		m_dummy.AddComponent(typeof(Rigidbody));
		m_dummy.GetComponent<Rigidbody>().AddTorque(0, 2000, 0);
	}

	/// <summary>
	/// If the dummy capsid has been created send it shrinking into the level.
	/// </summary>
	void Update() {
		if (m_dummy != null) {
			m_dummy.transform.position = Vector3.Lerp(m_dummy.transform.position, m_selectedLevelButton.transform.position, Time.deltaTime);
			m_dummy.transform.localScale = Vector3.Lerp(m_dummy.transform.localScale, Vector3.one, Time.deltaTime);
		}
	}

	/// <summary>
	/// Calculates the button positions automagically.  Puts them evenly spaced on an oval with a 4:3 x:y ratio
	/// </summary>
	void CalculateButtonPositions() {
		float angOffset = arrangementAngleOffset * Mathf.PI / 180f;
		
		newButtonPositions = new Vector3[buttons.Length];
		
		for (int i = 0; i < buttons.Length; i++) {
			//Debug.Log((angOffset + Mathf.PI / 2f - 2f * Mathf.PI / buttons.Length * i) * 180f / Mathf.PI);
			angOffset = buttonAngles[i] * Mathf.PI / 180f;
			//newButtonPositions[i] = new Vector3(ovalWidth * Mathf.Cos(angOffset + Mathf.PI / 2f - 2f * Mathf.PI / buttons.Length * i), 0f, ovalHeight * Mathf.Sin(angOffset + Mathf.PI / 2f - 2f * Mathf.PI / buttons.Length * i) - zButtonOffset);
			float cosTemp = Mathf.Cos(angOffset); //angOffset + Mathf.PI / 2f - 2f * Mathf.PI / buttons.Length * i);
			float xTemp = Mathf.Pow(Mathf.Abs(cosTemp), 2f/nEllipse) * aEllipse * Mathf.Sign(cosTemp);
			float sinTemp = Mathf.Sin(angOffset); //angOffset + Mathf.PI / 2f - 2f * Mathf.PI / buttons.Length * i);
			float yTemp = Mathf.Pow(Mathf.Abs(sinTemp), 2f/nEllipse) * bEllipse * Mathf.Sign(sinTemp);
			newButtonPositions[i] = new Vector3(xTemp, 0f, yTemp - zButtonOffset);
		}
	}
}
