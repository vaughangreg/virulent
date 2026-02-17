using UnityEngine;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour {
	public const float DEFAULT_TTL = 5.0f;
	
	[System.Serializable]
	public class Preset {
		public string label;
		public PopupManager.PresetIDs id;
		[System.Xml.Serialization.XmlIgnoreAttribute]public Texture2D image;
		public string imagePath;
		[HideInInspector]public string text {get { return "Touch to view more!"; }}
		public bool isButton = false;
		public PopupManager.ButtonActions buttonAction;
		[System.NonSerialized][HideInInspector] public float timeToLive;
		public bool isOpen;
		public Almanac.Categories categoryToView;
		public string itemToView;
	}
	public enum PresetIDs {
		EMPTY,
		Almanac_Virion,
		Almanac_Monomer,
		Almanac_BCell,
		Almanac_Antibody,
		Almanac_Slicer,
		Almanac_PRR,
		Almanac_Proteasome,
		Almanac_mRNACell,
		Almanac_Caspace,
		Almanac_Interferon,
		Almanac_DNA,
		Almanac_ER,
		Almanac_CellMembrane,
		Almanac_Ribosome,
		Almanac_Mitochondria,
		Almanac_Cytoskeleton,
		Almanac_NuclearPores,
		Almanac_PR,
		Almanac_Genome,
		Almanac_LP,
		Almanac_mRNAViral,
		Almanac_N,
		Almanac_M,
		Almanac_AntiGenome,
		Almanac_G,
		Almanac_BuddingSite,
		Almanac_Cell,
		Early_End,
		Almanac_Invagination,
		Almanac_Nucleus,
		Almanac_Golgi,
		Almanac_Executioner,
		Almanac_Vesicle,
		Almanac_Antigenome
	}
	public enum ButtonActions {
		OpenAlmanac,
		OpenAchievement
	}
	
	//Inspector elements
	public Almanac usedAlmanac;
	public GameObject popupPrefab;
	[HideInInspector]public List<Preset> Presets = new List<Preset>();
	[HideInInspector]public bool isOpen;
	[HideInInspector]public Vector2 scroller = Vector2.zero;
	
	//Private members
	private Queue<Preset> m_popupQueue = new Queue<Preset>();
	private bool m_handlingPopup = false;
	private float m_popupTime;
	private float m_popupTimeElapsed;
	private GameObject m_displayedPopup;
	private List<Preset> m_presetsToRemove = new List<Preset>();
	
	/// <summary>
	/// Setup listeners
	/// </summary>
	void Awake() {
		Dispatcher.Listen(Dispatcher.POPUP, gameObject);
		Dispatcher.Listen(Dispatcher.GAME_OVER, gameObject);
		Dispatcher.Listen(Dispatcher.UNIT_SELECT, gameObject);
	}
	
	/// <summary>
	/// If the manager is not handling any popups and there are popups to create, make one
	/// </summary>
	void Update() {
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Return)) new MessagePopup(gameObject,PresetIDs.Almanac_Virion, 5);
		if (Input.GetKeyDown(KeyCode.E))      new MessagePopup(gameObject,PresetIDs.Early_End, 5);
#endif
		//Handle popups if we aren't and we have some to make
		if (!m_handlingPopup && m_popupQueue.Count > 0) {
			m_displayedPopup = CreatePopup(m_popupQueue.Dequeue());
			//Debug.Log("Popups left in Queue: " + m_popupQueue.Count);
		}
		else if (m_handlingPopup) { //Count down and destroy popup after time is over
			m_popupTimeElapsed += Time.deltaTime;
			if (m_popupTimeElapsed > m_popupTime) {
				//Destroy the popup
				DestroyPopup(m_displayedPopup);
				m_displayedPopup = null;
				m_handlingPopup = false;
			}
		}
	}
	
	void _OnPopup(MessagePopup m) {
		//Find the correct preset from the list of presets.
		//If it exists, add it to the popup queue
		foreach (Preset p in Presets) {
			if (p.id == m.popupPreset) {
				Preset temp = p;
				temp.timeToLive = m.life;
				//Enqueue the popup
				m_popupQueue.Enqueue(temp);
				break;
			}
		}
	}
	
	void _OnGameOver(MessageGameOver m) {
		Destroy(m_displayedPopup);
		m_popupQueue.Clear();
	}
	
	/// <summary>
	/// If a unit is selected and it's the first time, give a popup notice
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageUnitSelect"/>
	/// </param>
	void _OnUnitSelect(MessageUnitSelect m)
	{
		string sourceString = "selectCount_"+m.source.tag;

		int count = PlayerPrefs.GetInt(sourceString, 0);
		// Debug.Log(count.ToString());
		
		bool isUnlocked = Almanac.GetUnitUnlocked(m.source.tag);
		
		//Create a popup if this is the first selection
		if (!isUnlocked) {
			new MessagePopup(gameObject,m.popupId,5);
		}
		
		//Add to the count of selected units
		PlayerPrefs.SetInt(sourceString, count + 1);
	}	
	
	GameObject CreatePopup(Preset presetToCreate) {
		//Create the popup from the given preset
		GameObject go = (GameObject)Instantiate(popupPrefab);
		Popup popup = go.GetComponent<Popup>();
		popup.titleName = presetToCreate.itemToView;
		popup.image = presetToCreate.image;
		popup.additionalText = presetToCreate.text;
		popup.timeToLive = presetToCreate.timeToLive;
		popup.isButton = presetToCreate.isButton;
		
		//Setup the timing for the update loop
		m_popupTime = presetToCreate.timeToLive;
		m_popupTimeElapsed = 0.0f;
		
		//Let the update loop know that we're handling a popup now
		m_handlingPopup = true;
		//Return the created popup
		return popup.gameObject;	
	}
	
	void DestroyPopup(GameObject popup) {
		Destroy(popup);
	}
	
	#region Editor Related
	public void RemovePreset(PopupManager.Preset presetToRemove) {
		m_presetsToRemove.Add(presetToRemove);
	}
	
	///Takes all of the presets from presets to remove and deletes them
	public void CleanUpPresets() {
		foreach(Preset p in m_presetsToRemove) {
			if (Presets.Contains(p)) {
				Presets.Remove(p);
			}
		}
		m_presetsToRemove.Clear();
	}
	#endregion		                
}

