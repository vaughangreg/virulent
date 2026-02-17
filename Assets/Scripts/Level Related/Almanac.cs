using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Embedded class that holds information of an item displayed in the almanac
/// </summary>
[System.Serializable]
public class AlmanacItem {
	public string label = "empty";
	//public Texture2D thumbNail;
	public Texture2D image;
	public string description;
	public string[] unitTag = new string[1];
	[HideInInspector][System.NonSerialized] public bool unlocked = false;
	[HideInInspector][System.NonSerialized] public bool open = false;
	[HideInInspector][System.NonSerialized] public bool showTags = false;
}

/// <summary>
/// A three-panel menu that shows information of units in the game; attach to main camera, k thx.
/// </summary>
public class Almanac : MonoBehaviour
{
	public enum Views { Category = 0, List = 1, Item = 2 };
	public enum Categories { Virus, Cell, Other };
	
	
	public Texture2D backgroundPanel;
	public Texture2D[] categoryIcons;
	public Texture2D nextButton;
	public Texture2D prevButton;
	public Font smallTextFont;
	public Font titleTextFont;
	
	public Texture2D itemOverlay;
	
	public GameObject pauseOverlayPrefab;
	//public GUIStyle arrowButtons;
	
	[HideInInspector] public List<AlmanacItem> teamVirusItems = new List<AlmanacItem>();
	[HideInInspector] public List<AlmanacItem> teamCellItems = new List<AlmanacItem>();
	[HideInInspector] public List<AlmanacItem> otherItems = new List<AlmanacItem>();
	[HideInInspector][System.NonSerialized] public bool showVirus = false;
	[HideInInspector][System.NonSerialized] public bool showCell = false;
	[HideInInspector][System.NonSerialized] public bool showOther = false;
	
	[HideInInspector] public static bool almanacIsOpen = false;
	private Categories m_selectedCategory;
	private Views m_displayedView;
	private int m_viewIndex;
	private int m_itemIndex;
	private bool m_gameIsPaused;
	private GameObject m_pauseOverlay;
	private GameObject m_viewRequester;
	
	//GUI RELATED
	
	//TODO: Put in proper sizes and positions
	
	//Constants
	static Vector2 CATEGORY_ICON_SIZE =     new Vector2(210,166);
	static Vector2 CATEGORY_ICON_POSITION = new Vector2(140,265);
	static Vector2 CATEGORY_ICON_SPACING =  new Vector2(52,0);
	static Vector2 ITEM_ICON_POSITION =     new Vector2(181,147);
	static Vector2 ITEM_ICON_SIZE =         new Vector2(119,94);
	static Vector2 ITEM_ICON_SPACING =      new Vector2(58,58);
	
	static float SCALE;
	
	//Category View
	private LevelGUIElement bCategory =  new LevelGUIElement("",CATEGORY_ICON_POSITION,CATEGORY_ICON_SIZE); //<< Generic, will be used many times
	//List View
	
	//Item View
	private LevelGUIElement bItemTitle = new LevelGUIElement("",new Vector2(327, 421), new Vector2(370, 50));
	private LevelGUIElement bItemPic =   new LevelGUIElement("",new Vector2(380, 176),  new Vector2(263, 210));
	private LevelGUIElement bItemInfo =  new LevelGUIElement("",new Vector2(300, 468), new Vector2(437, 117));
	private LevelGUIElement bNextItem =  new LevelGUIElement("",new Vector2(742, 246), new Vector2(46,53));
	private LevelGUIElement bPrevItem =  new LevelGUIElement("",new Vector2(234, 246), new Vector2(46,53));
	//Visible in ALL VIEWS
	private LevelGUIElement bBackground = new LevelGUIElement("",new Vector2(86,108),new Vector2(864,595));
	private LevelGUIElement bBack =       new LevelGUIElement("BACK",
	                                                          new Vector2(86 + GuiResources.feedbackButtonSize.x/2, 
	                                                                      608 + GuiResources.feedbackButtonSize.y/2),
	                                                          GuiResources.feedbackButtonSize);
	private LevelGUIElement bResumeGame = new LevelGUIElement("RESUME",
	                                                          new Vector2(665 + GuiResources.feedbackButtonSize.x/2, 
	                                                                      608 + GuiResources.feedbackButtonSize.y/2),
	                                                          GuiResources.feedbackButtonSize);
	
	private GUIStyle blankStyle = new GUIStyle();
	private GUIStyle textStyle;
	private GUIStyle smallTextStyle;
	private GUIStyle titleTextStyle;

	private AudioClip sfxMenuConfirm;
	private AudioClip sfxMenuSelect;
	private AudioClip sfxMenuDeny;
	
	
	/// <summary>
	/// Setup Listeners
	/// </summary>
	void Awake()
	{
		useGUILayout = false;
		//Listen for the opening of the almanac
		Dispatcher.Listen(Dispatcher.VIEW_ALMANAC, gameObject);
		//Almanac will close if the game is unpaused
		Dispatcher.Listen(Dispatcher.PAUSE_GAME, gameObject);
	}
	void Start()
	{
#if UNITY_EDITOR
		if (!MusicManager.singleton) Instantiate(Resources.Load("Jukebox"));
#endif
		//Get the stats style for the use of labels
		textStyle = new GUIStyle(GuiResources.instance.levelInfoStyle);
		textStyle.normal.textColor = Color.black;
		textStyle.alignment = TextAnchor.MiddleCenter;
		
		smallTextStyle = new GUIStyle(GuiResources.instance.goalStyle);
		smallTextStyle.normal.textColor = Color.black;
		smallTextStyle.font = smallTextFont;
		
		titleTextStyle = new GUIStyle(textStyle);
		titleTextStyle.alignment = TextAnchor.UpperCenter;
		titleTextStyle.font = titleTextFont;
		
#if UNITY_WEBPLAYER
		smallTextStyle.font = GuiResources.instance.goalStyle.font;
		smallTextStyle.fontSize = 12;
		titleTextStyle.font = GuiResources.instance.levelInfoStyle.font;
		titleTextStyle.fontSize = 20;
		
#endif
		
		
		//Setup Item Unlocks
		SetupItemUnlocks();
		
		//Setup ScaleFactor
		SCALE = GuiResources.GetScale();
		//Setup sfxMenuConfirm
		sfxMenuConfirm = MusicManager.singleton.sFXMenuConfirm;
		sfxMenuDeny = MusicManager.singleton.sFXMenuDeny;
		sfxMenuSelect = MusicManager.singleton.sFXMenuSelect;
		
		enabled = false;
	}
	
	/// <summary>
	/// Activate the Almanac, pause the game, and set the displayed view
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageViewAlmanac"/>
	/// </param>
	void _OnViewAlmanac(MessageViewAlmanac m)
	{
		enabled = true;
		SetupItemUnlocks();
		if (!almanacIsOpen) {
			
			AlmanacStart start = new AlmanacStart();
			ADAGE.LogContext<AlmanacStart>(start);

			m_viewRequester = m.source;
			//Pause the game (if it isn't already) and open the almanac
			if (!m_gameIsPaused) {

				new MessagePauseGame(gameObject, true);
				//Scale and position a Pause Overlay
				m_pauseOverlay = Instantiate(pauseOverlayPrefab) as GameObject;
				m_pauseOverlay.transform.position = Camera.main.transform.position + new Vector3(0,2,0);
				m_pauseOverlay.transform.localScale = Vector3.one * (Camera.main.orthographicSize * 3);
			}
			almanacIsOpen = true;
			
			if (m.itemToView != null) { //Open up to the specified ITEM
				m_itemIndex = GetItemIndex(m.itemToView);
				m_selectedCategory = GetItemCategory(m.itemToView);
				SetDisplayedView(Views.Item);
			}
			else { //Set the displayed view to the message's view
				SetDisplayedView(m.viewToShow);
			}
			
		} 
	}
	void _OnPauseGame(MessagePauseGame m)
	{
		if (!m.paused && almanacIsOpen) CloseAlmanac(false);
		m_gameIsPaused = m.paused;
	}
	
	/// <summary>
	/// Gets rid of the almanac from view
	/// </summary>
	/// <param name="openPauseMenu">
	/// Should the pause menu be opened when the almanac is closed
	/// </param>
	void CloseAlmanac(bool openPauseMenu)
	{
		enabled = false;
		almanacIsOpen = false;
		
		AlmanacEnd end = new AlmanacEnd();
		ADAGE.LogContext<AlmanacEnd>(end);

		if (m_viewRequester) {
			//Do appropriate action based on the 'openPauseMenu' argument
			/*
			if (openPauseMenu){
				if (m_viewRequester.GetComponent<PauseMenu>()) {
					m_viewRequester.GetComponent<PauseMenu>().drawGUI = true;
				}
			} 
			else*/ Destroy(m_viewRequester);
		}
		if (!openPauseMenu) new MessagePauseGame(gameObject, false);
		
		//Get rid of the pause overlay
		if (m_pauseOverlay) Destroy(m_pauseOverlay);
	}
	
	void SetupItemUnlocks()
	{
		foreach(AlmanacItem item in teamVirusItems) {
			item.unlocked = GetUnitUnlocked(item.unitTag);
		}
		foreach(AlmanacItem item in teamCellItems) {
			item.unlocked = GetUnitUnlocked(item.unitTag);
		}
		foreach(AlmanacItem item in otherItems) {
			item.unlocked = GetUnitUnlocked(item.unitTag);
		}
	}
	
	#region GUI Related
	void OnGUI()
	{
		if (almanacIsOpen) {
			//Draw the elements that will ALWAYS display
			DisplayPersistantElements();
			//Show the correct 'view' of the almanac
			switch (m_displayedView)
			{
			case Views.Category:
				DisplayCategoryView(); break;
			case Views.List:
				DisplayListView(m_selectedCategory); break;
			case Views.Item:
				DisplayItemView(m_selectedCategory); break;
			}
		}
	}
	
	void DisplayCategoryView()
	{
		Rect rect;
		//Draw the 3 category buttons
		for (int i = 0; i < 3; i++)
		{
			rect = bCategory.GetGUIRect();
			rect.x += (CATEGORY_ICON_SIZE.x + CATEGORY_ICON_SPACING.x) * i * SCALE;
			if (GUI.Button(rect,categoryIcons[i],blankStyle)) {
				//Select the category we are going to use
				m_selectedCategory = IntToCategory(i);
				SetDisplayedView(Views.List);
				
				PlaySound(sfxMenuConfirm);

				AlmanacViewCategoryEvent cat = new AlmanacViewCategoryEvent(CategoryToString(IntToCategory(i)));
				ADAGE.LogData<AlmanacViewCategoryEvent>(cat);

			}
			rect.y += 118 * SCALE;
			GUI.Label(rect,CategoryToString(IntToCategory(i)),textStyle);
		}
	}
	
	void DisplayListView(Categories category)
	{
		int itemsPerRow = 4;
		int itemIndex = 0;
		Rect rect;
		//---LISTED BUTTONS
		foreach (AlmanacItem item in GetCategoryList(category)) {
			//Calculate the rectangle of the element
			rect = new Rect((ITEM_ICON_POSITION.x + (itemIndex % itemsPerRow) * (ITEM_ICON_SIZE.x + ITEM_ICON_SPACING.x)) * SCALE,
			                     (ITEM_ICON_POSITION.y + (int)(itemIndex / itemsPerRow) * (ITEM_ICON_SIZE.y + ITEM_ICON_SPACING.y)) * SCALE,
			                     (ITEM_ICON_SIZE.x) * SCALE,
			                     (ITEM_ICON_SIZE.y) * SCALE);
			if (!item.unlocked) { //Draw an overlay over it
				GUI.Label(rect,item.image,blankStyle);
				GUI.Label(rect,itemOverlay,blankStyle);
			}
			else if (GUI.Button(rect,item.image,blankStyle)) {
				//Select the item we are going to view
				m_itemIndex = itemIndex;
				//Enter the item view
				SetDisplayedView(Views.Item);
				PlaySound(sfxMenuConfirm);

				AlmanacViewItemEvent viewed_item = new AlmanacViewItemEvent(item.label);
				ADAGE.LogData<AlmanacViewItemEvent>(viewed_item);

			}
			//Position and display the name label of the item
			rect.y += 78 * SCALE;
			GUI.Label(rect,item.label,textStyle);
			itemIndex ++;
		}
	}
	
	void DisplayItemView(Categories currentCategory)
	{
		List<AlmanacItem> list = GetCategoryList(currentCategory);
		//---TITLE
		GUI.Label(bItemTitle.GetGUIRect(), list[m_itemIndex].label, titleTextStyle);
		
		//---PICTURE PANEL
		GUI.Label(bItemPic.GetGUIRect(), list[m_itemIndex].image,blankStyle);
		//---INFO PANEL
		GUI.Label(bItemInfo.GetGUIRect(), list[m_itemIndex].description, smallTextStyle);
		if (GetUnlockCount(currentCategory) > 1) {
			//---NEXT BUTTON
			if (GUI.Button(bNextItem.GetGUIRect(),"", GuiResources.instance.almanacArrowRight)) {
				m_itemIndex = GetNextItem(currentCategory, m_itemIndex);
				PlaySound(sfxMenuSelect);
					
				AlmanacViewItemEvent viewed_item = new AlmanacViewItemEvent(list[m_itemIndex].label);
				ADAGE.LogData<AlmanacViewItemEvent>(viewed_item);
			}
			//---PREV BUTTON
			if (GUI.Button(bPrevItem.GetGUIRect(),"",  GuiResources.instance.almanacArrowLeft)) {
				m_itemIndex = GetPreviousItem(currentCategory, m_itemIndex);
				PlaySound(sfxMenuSelect);
				
				AlmanacViewItemEvent viewed_item = new AlmanacViewItemEvent(list[m_itemIndex].label);
				ADAGE.LogData<AlmanacViewItemEvent>(viewed_item);
			}
		}
		
	}
	
	/// <summary>
	/// Draw Gui items that will always be shown when the almanac is open
	/// </summary>
	void DisplayPersistantElements()
	{
		//---BACKGROUND PANEL
		GUI.Label(bBackground.GetGUIRect(),backgroundPanel);
		
		//---FOOTER TEXT
		//TODO: Put in footer text
		
		//---BACK BUTTON
		if (GUI.Button(bBack.GetGUIRect(), bBack.text, GuiResources.instance.buttonStyle)) {
			SetDisplayedView(m_viewIndex - 1);
			PlaySound(sfxMenuDeny);
		}
		//---RESUME GAME BUTTON
		if (GUI.Button(bResumeGame.GetGUIRect(), bResumeGame.text, GuiResources.instance.buttonStyle)) {
			CloseAlmanac(false);
			PlaySound(sfxMenuDeny);
		}
	}
	
	#endregion
	
	#region Utility Methods
	public int GetItemIndex(string itemName)
	{
		List<AlmanacItem> list;
		list = GetCategoryList(Categories.Virus);
		for (int i=0; i< list.Count; i++) {
			if (list[i].label == itemName) return i;
		}
		list = GetCategoryList(Categories.Cell);
		for (int i=0; i< list.Count; i++) {
			if (list[i].label == itemName) return i;
		}
		list = GetCategoryList(Categories.Other);
		for (int i=0; i< list.Count; i++) {
			if (list[i].label == itemName) return i;
		}
		//if we didn't find it, return -1
		Debug.LogError("Specified Almanac Item name '" + itemName +"' does not exist");
		return -1;
	}
	public int GetItemIndex(string itemName, Categories category)
	{
		List<AlmanacItem> list;
		list = GetCategoryList(category);
		for (int i=0; i< list.Count; i++) {
			if (list[i].label == itemName) return i;
		}
		//if we didn't find it, return -1
		Debug.LogError("Specified Almanac Item name '" + itemName +"' does not exist");
		return -1;
	}
	Categories GetItemCategory(string itemName)
	{
		List<AlmanacItem> list;
		list = GetCategoryList(Categories.Virus);
		for (int i=0; i< list.Count; i++) {
			if (list[i].label == itemName) return Categories.Virus;
		}
		list = GetCategoryList(Categories.Cell);
		for (int i=0; i< list.Count; i++) {
			if (list[i].label == itemName) return Categories.Cell;
		}
		list = GetCategoryList(Categories.Other);
		for (int i=0; i< list.Count; i++) {
			if (list[i].label == itemName) return Categories.Other;
		}
		//if we didn't find it, return -1
		Debug.LogError("Specified Almanac Item name '" + itemName +"' does not exist");
		return Categories.Virus;
	}
	
	public List<string> GetItemNames(Categories category)
	{
		List<string> itemNames = new List<string>();
		foreach(AlmanacItem item in GetCategoryList(category)) {
			itemNames.Add(item.label);
		}
		return itemNames;
	}
	
	int GetNextItem(Categories category, int currentIndex)
	{
		int listLength = GetListLength(category);
		//Return the proper index
		currentIndex ++;
		if (currentIndex >= listLength) currentIndex = 0;
		while (!GetCategoryList(category)[currentIndex].unlocked) {
			currentIndex ++;
			if (currentIndex >= listLength) currentIndex = 0;
		}
		if (currentIndex > listLength) return 0;
		else return currentIndex;
	}
	int GetPreviousItem(Categories category, int currentIndex)
	{
		//Return the proper index
		currentIndex --;
		if (currentIndex < 0) currentIndex = GetListLength(category)-1;
		
		while (!GetCategoryList(category)[currentIndex].unlocked) {
			currentIndex --;
			if (currentIndex < 0) currentIndex = GetListLength(category)-1;
		}
		if (currentIndex < 0) return GetListLength(category);
		else return currentIndex;
	}
	int GetListLength(Categories category)
	{
		//Get the list length of the category we're using
		return GetCategoryList(category).Count;
	}
	/// <summary>
	/// Returns the number of unlocked items in the category
	/// </summary>
	/// <param name="category">
	/// Category to check
	/// </param>
	/// <returns>
	/// number of unlocked items
	/// </returns>
	int GetUnlockCount(Categories category)
	{
		int count = 0;
		foreach(AlmanacItem item in GetCategoryList(category)) {
			if (item.unlocked) count += 1;
		}
		return count;
	}
	/// <summary>
	/// Returns the given category's list of almanac items
	/// </summary>
	/// <param name="category">
	/// The category to look at
	/// </param>
	/// <returns>
	/// A list of almanac items from given category
	/// </returns>
	List<AlmanacItem> GetCategoryList(Categories category)
	{
		switch (category) {
		case Categories.Virus:
			return teamVirusItems;
		case Categories.Cell:
			return teamCellItems;
		case Categories.Other:
			return otherItems;
		default:
			return null;
		}
	}
	public void SetDisplayedView(Views newView)
	{
		m_displayedView = newView;
		m_viewIndex = (int)newView;
	}
	public void SetDisplayedView(int newViewIndex)
	{
		m_displayedView = (Views)newViewIndex;
		m_viewIndex = newViewIndex;
		//Check to see if the almanac needs to close, and do so if so
		if (m_viewIndex < 0) {
			CloseAlmanac(true);
		}
	}
	Categories IntToCategory(int anInt) {
		return (Categories)anInt;
	}
	int CategoryToInt(Categories aCategory) {
		return (int)aCategory;
	}
	string CategoryToString(Categories aCategory) {
		switch (aCategory) {
		case Categories.Virus: return "Virus";
		case Categories.Cell: return "Cell/Immune System";
		case Categories.Other: return "Other Objects";
		default: return "";
		}
	}
	
	public static bool GetLevelUnlocked(int levelIndex)
	{
		if (levelIndex == -1 || levelIndex >= Constants.LEVELS.Count) return false;
		
		//Check to see if teh number of units that have made it through that level are not zero
		if (PlayerPrefs.GetInt(Constants.LEVELS[levelIndex], 0) > 0) {
			return true;
		}
		else return false;
	}
	public static bool GetUnitUnlocked(string unitTag)
	{		
		if (PlayerPrefs.GetInt("selectCount_"+unitTag,0) > 0) {
			return true;
		}
		else return false;
	}
	public static bool GetUnitUnlocked(string[] unitTag)
	{
		foreach(string s in unitTag) {
			if (PlayerPrefs.GetInt("selectCount_"+s,0) > 0) {
				return true;
			}
		}
		//We found nothing
		return false;
	}
	
	
	void PlaySound(AudioClip aClip)
	{
		//This component only plays GuiSFX
		MusicManager.PlayGuiSfx(aClip);
	}
	#endregion

}

