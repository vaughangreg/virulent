using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

[CustomEditor(typeof(PopupManager))]
public class PopupManagerEditor : Editor
{
	private PopupManager pm;
	private PopupManager.Preset selectedPreset = null;
	private GUILayoutOption[] scrollerOptions;
	
	private static string filePath = Application.dataPath + "/PrefabXML/PopupManager.xml";
	
	public override void OnInspectorGUI()
	{
		scrollerOptions = new GUILayoutOption[1] {
			GUILayout.Height(200)
		};
		pm = (PopupManager)target;
		EditorGUI.indentLevel = 0;
		base.OnInspectorGUI();
		
		//Make the labels corret
		foreach(PopupManager.Preset p in pm.Presets) {
			p.label = p.id.ToString();
		}
		
		pm.isOpen = EditorGUILayout.Foldout(pm.isOpen,"Popup Presets");
		if (pm.isOpen) {
			if (pm.usedAlmanac) {
				UtilityButtons();
				PresetRegion();
			}
			else {
				GUILayout.Label("'Used Almanac' must be set to edit Presets");
			}
		}
	}
	
	void UtilityButtons()
	{
		EditorGUI.indentLevel = 1;
		//Static Buttons ( if editor is off)
		if (!Application.isPlaying) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Save Presets", GUILayout.Width(140))) {
				SavePresets(pm.Presets);
			}
			if (GUILayout.Button("Load Presets", GUILayout.Width(140))) {
				pm.Presets = LoadPresets();
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add New Preset", GUILayout.Width(140))) { //Add new preset
				foreach (PopupManager.Preset preset in pm.Presets) {
					preset.isOpen = false;
				}
				PopupManager.Preset p = new PopupManager.Preset();
				p.isOpen = true;
				pm.Presets.Add(p);
				selectedPreset = p;
			}
			if (selectedPreset != null) {
				if (GUILayout.Button("Delete Preset")) {
				pm.RemovePreset(selectedPreset);
				selectedPreset = null;
				pm.CleanUpPresets();
				}
			}
			else GUILayout.Label("");
			EditorGUILayout.EndHorizontal();
		}
	}
	void PresetRegion()
	{
		pm.scroller = EditorGUILayout.BeginScrollView(pm.scroller,scrollerOptions);
		foreach (PopupManager.Preset p in pm.Presets) {
			EditorGUI.indentLevel = 1;
			bool fold = EditorGUILayout.Foldout(p.isOpen,p.label);
			if (p.isOpen != fold) { //Close all other preset folds
				foreach(PopupManager.Preset q in pm.Presets) q.isOpen = false;
				p.isOpen = fold;
			}
			EditorGUI.indentLevel = 2;
			if (p.isOpen) //The preset tab is open
			{
				selectedPreset = p;
				p.id = (PopupManager.PresetIDs)EditorGUILayout.EnumPopup("ID",p.id);
				p.image = EditorGUILayout.ObjectField("Image", p.image,typeof(Texture2D)) as Texture2D;
				//p.text = EditorGUILayout.TextField("Text", p.text);
				
				p.isButton = EditorGUILayout.Toggle("Is Button",p.isButton);
				//Display more options if 'isButton'
				if (p.isButton) {
					EditorGUI.indentLevel = 3;
					p.buttonAction = (PopupManager.ButtonActions)EditorGUILayout.EnumPopup("Button Action", p.buttonAction);
					//Do special cases dependant on button action type
					switch (p.buttonAction) {
						//----------OPEN ACHIEVEMENT------------
					case PopupManager.ButtonActions.OpenAchievement:
						break;
						//------------OPEN ALMANAC--------------
					case PopupManager.ButtonActions.OpenAlmanac:
						//Pick your category
						p.categoryToView = (Almanac.Categories)EditorGUILayout.EnumPopup("Category",p.categoryToView);
						//make a temp list of all available almanac items
						List<string> selectableItems = pm.usedAlmanac.GetItemNames(p.categoryToView);
						if (selectableItems != null && selectableItems.Count > 0) { //select from it if you can
							if (!selectableItems.Contains(p.itemToView)) p.itemToView = selectableItems[0];
							int index = EditorGUILayout.Popup("Item",pm.usedAlmanac.GetItemIndex(p.itemToView,p.categoryToView),selectableItems.ToArray());
							p.itemToView = selectableItems[index];
						}
						else EditorGUILayout.LabelField("Item","Category has no items");
						break;
					}
				}
			}
		}
		EditorGUILayout.EndScrollView();
	}
	
	void SavePresets(List<PopupManager.Preset> presetsToSave)
	{
		//Record the asset paths
		foreach(PopupManager.Preset p in presetsToSave)
		{
			p.imagePath = AssetDatabase.GetAssetPath(p.image);
		}
		//Serialize
		XmlSerializer serializer = new XmlSerializer(typeof(List<PopupManager.Preset>));
		FileStream stream = File.Create(filePath);
		serializer.Serialize(stream, presetsToSave);
		stream.Close();
	}
	List<PopupManager.Preset> LoadPresets()
	{
		List<PopupManager.Preset> list = new List<PopupManager.Preset>();
		
		XmlSerializer serializer = new XmlSerializer(typeof(List<PopupManager.Preset>));
		TextReader reader = new StreamReader(filePath);
		list =  serializer.Deserialize(reader) as List<PopupManager.Preset>;
		//Put the textures in the list... SINCE THEY DON'T SERIALIZE >:(
		foreach(PopupManager.Preset p in list)
		{
			p.image = AssetDatabase.LoadAssetAtPath(p.imagePath,typeof(Texture2D)) as Texture2D;
		}
		return list;
	}
}

