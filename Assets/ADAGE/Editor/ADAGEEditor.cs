#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using LitJson; 

[CustomEditor(typeof(ADAGE))]
public class ADAGEEditor : Editor
{
	public static ADAGEGameVersionInfo VersionInfo;

	private bool loaded = false;

	private bool showingInputControl = false;
	private bool showingLocalLogging = false;
	private bool showingLoginControl = false;

	private bool showingProductionSettings = false;
	private bool showingDevelopmentSettings = false;
	private bool showingStagingSettings = false;

	private int index = -1;
	private int prevIndex;
	private string[] paths;

	private bool enableGameHandlesLogin = true;
	private bool enableGuestLogin = true;
	private bool enableFacebookLogin = true;
	private bool enableAutomaticLogin = true;
	private bool enableDefaultLogin = true;
	private bool enableLastUser = true;
	private bool enableGuestAccount = true;
	private bool enableNextLevel = true;

	static ADAGEEditor()
	{
	}

	static void ADAGEContextInfoToJSON(Dictionary<string, ADAGEContextInfo> info, JsonWriter writer)
	{
		writer.WriteArrayStart();
		{
			foreach(KeyValuePair<string, ADAGEContextInfo> item in info)
			{
				writer.Write (item.Key);
			}
		}
		writer.WriteArrayEnd();
	}

	/*private void CheckCompile() 
	{
		if(EditorApplication.isCompiling)
		{
			compiling = true; 
		}
		else
		{
			if(compiling)
			{
				compiling = false;
				Refresh();
			}
		}
	}*/

	public void OnEnable()
	{
		if(!loaded)
		{
			loaded = true;
		}

		SaveLevelList();
	}

	public void OnFocus()
	{
		index = (target as ADAGE).GetNextLevel();
	}

	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		ADAGE curTarget = (target as ADAGE);

		showingProductionSettings = EditorGUILayout.Foldout(showingProductionSettings, "Production Settings");
		if(showingProductionSettings)
		{
			EditorGUI.indentLevel += 2;
			{
				curTarget.productionServer = EditorGUILayout.TextField("Server", curTarget.productionServer);
				curTarget.appToken = EditorGUILayout.TextField("Token", curTarget.appToken);
				curTarget.appSecret = EditorGUILayout.TextField("Secret", curTarget.appSecret);
				GUI.enabled = !curTarget.forceDevelopment && !curTarget.forceStaging;
					curTarget.forceProduction = EditorGUILayout.Toggle("Force Connect", curTarget.forceProduction);
				GUI.enabled = true;
			}
			EditorGUI.indentLevel -= 2;
		}

		showingDevelopmentSettings = EditorGUILayout.Foldout(showingDevelopmentSettings, "Development Settings");
		if(showingDevelopmentSettings)
		{
			EditorGUI.indentLevel += 2;
			{
				curTarget.developmentServer = EditorGUILayout.TextField("Server", curTarget.developmentServer);
				curTarget.devToken = EditorGUILayout.TextField("Token", curTarget.devToken);
				curTarget.devSecret = EditorGUILayout.TextField("Secret", curTarget.devSecret);
				GUI.enabled = !curTarget.forceProduction && !curTarget.forceStaging;
					curTarget.forceDevelopment = EditorGUILayout.Toggle("Force Connect", curTarget.forceDevelopment);
				GUI.enabled = true;
			}
			EditorGUI.indentLevel -= 2;
		}
		
		showingStagingSettings = EditorGUILayout.Foldout(showingStagingSettings, "Staging Settings");
		if(showingStagingSettings)
		{
			EditorGUI.indentLevel += 2;
			{
				curTarget.staging = EditorGUILayout.Toggle("Enable", curTarget.staging);
				curTarget.stagingServer = EditorGUILayout.TextField("Server", curTarget.stagingServer);
				curTarget.appToken = EditorGUILayout.TextField("Token", curTarget.appToken);
				curTarget.appSecret = EditorGUILayout.TextField("Secret", curTarget.appSecret);
				GUI.enabled = !curTarget.forceProduction && !curTarget.forceDevelopment;
					curTarget.forceStaging = EditorGUILayout.Toggle("Force Connect", curTarget.forceStaging);
				GUI.enabled = true;
			}
			EditorGUI.indentLevel -= 2;
		}

		showingLoginControl = EditorGUILayout.Foldout(showingLoginControl, "Login Settings");
		
		if(showingLoginControl)
		{
			EditorGUI.indentLevel += 2;

			GUI.enabled = enableGameHandlesLogin;
				curTarget.gameHandlesLogin = EditorGUILayout.Toggle("Game Handles Login", curTarget.gameHandlesLogin);
			GUI.enabled = enableGuestLogin;
				curTarget.allowGuestLogin = EditorGUILayout.Toggle("Enable Guest Login", curTarget.allowGuestLogin);
			GUI.enabled = enableFacebookLogin;
				curTarget.allowFacebook = EditorGUILayout.Toggle("Enable Facebook Login", curTarget.allowFacebook);

			GUI.enabled = enableAutomaticLogin;
			curTarget.automaticLogin = EditorGUILayout.Toggle("Automatically Login", curTarget.automaticLogin);

			EditorGUI.indentLevel += 2;
			{
				GUI.enabled = enableDefaultLogin;
				curTarget.forceLogin = EditorGUILayout.Toggle("Use Default Login", curTarget.forceLogin);
				GUI.enabled = curTarget.forceLogin;
				EditorGUI.indentLevel += 2;
					curTarget.forcePlayer = EditorGUILayout.TextField("Username", curTarget.forcePlayer);
					curTarget.forcePassword = EditorGUILayout.TextField("Password", curTarget.forcePassword);
				EditorGUI.indentLevel -= 2;

				GUI.enabled = enableLastUser;
				curTarget.cacheLastUser = EditorGUILayout.Toggle("Last User", curTarget.cacheLastUser);

				GUI.enabled = enableGuestAccount;
				curTarget.autoGuestLogin = EditorGUILayout.Toggle("Guest", curTarget.autoGuestLogin);

				GUI.enabled = enableNextLevel;
				index = EditorGUILayout.Popup("Next Level", index, paths);
				if(index != -1 && index != prevIndex)
				{
					curTarget.SetNextLevel(index);
					prevIndex = index;
				}
			}
			EditorGUI.indentLevel -= 2;
			GUI.enabled = true;

			EditorGUI.indentLevel -= 2;
		}

		showingInputControl = EditorGUILayout.Foldout(showingInputControl, "Input Logging");

		if(showingInputControl)
		{
			EditorGUI.indentLevel += 2;
			curTarget.enableKeyboardCapture = EditorGUILayout.Toggle("Enable Keyboard Capture", curTarget.enableKeyboardCapture);

			GUI.enabled = curTarget.enableKeyboardCapture;
			EditorGUI.indentLevel += 2;
			curTarget.autoCaptureKeyboard = EditorGUILayout.Toggle("Auto Capture Keyboard", curTarget.autoCaptureKeyboard);
			EditorGUI.indentLevel -= 2;
			GUI.enabled = true;
			
			curTarget.enableMouseCapture = EditorGUILayout.Toggle("Enable Mouse Capture", curTarget.enableMouseCapture);
			
			GUI.enabled = curTarget.enableMouseCapture;
			EditorGUI.indentLevel += 2;
			curTarget.autoCaptureMouse = EditorGUILayout.Toggle("Auto Capture Mouse", curTarget.autoCaptureMouse);
			EditorGUI.indentLevel -= 2;
			GUI.enabled = true;
			EditorGUI.indentLevel -= 2;
		}

		if(ADAGEEditor.VersionInfo != null && ADAGEEditor.VersionInfo.dirty)
		{
			ADAGEEditor.VersionInfo.dirty = false;
			Debug.Log ("merging");
			MergeVersionInfo(target);
			EditorUtility.SetDirty(curTarget);
		}

		showingLocalLogging = EditorGUILayout.Foldout(showingLocalLogging, "Local Logging");
		if(showingLocalLogging)
		{
			EditorGUI.indentLevel += 2;
			curTarget.enableLogLocal = EditorGUILayout.Toggle("Enable", curTarget.enableLogLocal);
			GUI.enabled = curTarget.enableLogLocal;
				GUILayout.BeginHorizontal();
				{
					EditorGUILayout.TextField("Data Path", (target as ADAGE).dataPath);
					if(GUILayout.Button("Select"))
					{
						string oldPath = curTarget.dataPath;
						curTarget.dataPath = EditorUtility.OpenFolderPanel("Select ADAGE Data Directory", "", "");
						
						string appDataPath = Application.dataPath;
						bool longer = (curTarget.dataPath.Length >= appDataPath.Length);
						if(!longer || (longer && curTarget.dataPath.Substring(0,appDataPath.Length) != appDataPath))
						{
							EditorUtility.DisplayDialog("ADAGE Data Path Error", "You must select a path that is in the project path", "OK");
							curTarget.dataPath = oldPath;
						}
						else
						{
							curTarget.dataPath = curTarget.dataPath.Substring(appDataPath.Length);	
						}
					}
				}
				GUILayout.EndHorizontal();
			GUI.enabled = true;
			EditorGUI.indentLevel -= 2;
		}

		GUILayout.Space(15);

		if(GUILayout.Button("Version Control"))
		{
			ADAGEVersionEditor.Init(curTarget);
			//ADAGEVersionEditor myCustomWindow = (ADAGEVersionEditor) EditorWindow.GetWindow(typeof(ADAGEVersionEditor) ,false, "ADAGE Version");
		}
		
		GUILayout.Space(15);

		if(GUI.changed)
		{
			CheckSettings(curTarget);
			EditorUtility.SetDirty(curTarget);
		}
	}

	private void SaveLevelList()
	{
		ADAGE tar = (target as ADAGE);
		int next = tar.GetNextLevel();

		int sceneCount = 0;
		for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			if(EditorBuildSettings.scenes[i].enabled)
				sceneCount++;
		}

		paths = new string[sceneCount];

		int pathIndex = 0;
		for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			if(EditorBuildSettings.scenes[i].enabled)
			{
				paths[pathIndex] = EditorBuildSettings.scenes[i].path;
				pathIndex++;
			}
		}

		if(paths.Length - 1 > next)
		{
			index = next;
		}
		
		prevIndex = index;
	}

	private void CheckSettings(ADAGE curTarget)
	{
		if(curTarget.allowGuestLogin || curTarget.allowFacebook || curTarget.cacheLastUser || curTarget.autoGuestLogin)
			curTarget.forceLogin = false;

		if(!curTarget.automaticLogin)
		{
			curTarget.forceLogin = false;
			curTarget.cacheLastUser = false;
			curTarget.autoGuestLogin = false;
		}

		if(curTarget.forceLogin)
		{
			curTarget.allowGuestLogin = false;
			curTarget.allowFacebook = false;
			curTarget.cacheLastUser = false;
		}

		//Enable and disable fields
		enableGameHandlesLogin = !curTarget.automaticLogin;
		enableGuestLogin = !curTarget.forceLogin;
		enableFacebookLogin = !curTarget.forceLogin && !curTarget.autoGuestLogin;
		enableAutomaticLogin = !curTarget.gameHandlesLogin;
		enableDefaultLogin = curTarget.automaticLogin && !curTarget.allowGuestLogin;
		enableLastUser = curTarget.automaticLogin && !curTarget.forceLogin && !curTarget.autoGuestLogin;
		enableGuestAccount = curTarget.automaticLogin && !curTarget.forceLogin && curTarget.allowGuestLogin;
		enableNextLevel = curTarget.automaticLogin;

		if(curTarget.productionServer.Trim() == "")
			curTarget.productionServer = ADAGE.productionURL;
		if(curTarget.developmentServer.Trim() == "")
			curTarget.developmentServer = ADAGE.developmentURL;
		if(curTarget.stagingServer.Trim() == "")
			curTarget.stagingServer = ADAGE.stagingURL;
	}

	void MergeVersionInfo(UnityEngine.Object target)
	{
		ADAGE currentTarget = (target as ADAGE);

		if(currentTarget.versionInfo == null)
		{
			Debug.Log ("Clearing version info");  
			currentTarget.versionInfo = ADAGEEditor.VersionInfo;
			return;
		}

		List<string> currentObjects = new List<string>();

		//Start Context Merge
		foreach(KeyValuePair<string, ADAGEEventInfo> item in ADAGEEditor.VersionInfo.context)
		{
			currentObjects.Add(item.Key);

			if(!currentTarget.versionInfo.context.ContainsKey(item.Key)) //If this event is new
			{
				currentTarget.versionInfo.AddContext(item.Key, item.Value);
			}
			else //If we already have an event like this
			{
				ADAGEEventInfo curEvent = item.Value;
				foreach(KeyValuePair<string, ADAGEDataPropertyInfo> prop in curEvent.properties)
				{
					if(!currentTarget.versionInfo.context[item.Key].properties.ContainsKey(prop.Key))
					{
						currentTarget.versionInfo.context[item.Key].properties.Add(prop.Key, prop.Value);
					}
				}
				
				foreach(KeyValuePair<string, ADAGEDataPropertyInfo> prop in currentTarget.versionInfo.context[item.Key].properties)
				{
					if(!curEvent.properties.ContainsKey(prop.Key))
					{
						currentTarget.versionInfo.context[item.Key].properties.Remove(prop.Key);
					}
				}
			}
		}

		//Look for any outdated context, as in they are no longer present in the folder
		foreach(KeyValuePair<string, ADAGEEventInfo> savedEvent in currentTarget.versionInfo.context)
		{
			if(!currentObjects.Contains(savedEvent.Key))
			{
				currentTarget.versionInfo.RemoveContext(savedEvent.Key);
			}
		}

		currentObjects = new List<string>();

		//Start Event Merge
		foreach(KeyValuePair<string, ADAGEEventInfo> item in ADAGEEditor.VersionInfo.eventTypes)
		{
			currentObjects.Add(item.Key);
			
			if(!currentTarget.versionInfo.eventTypes.ContainsKey(item.Key)) //If this event is new
			{
				currentTarget.versionInfo.AddEvent(item.Key, item.Value);
			}
			else //If we already have an event like this
			{
				ADAGEEventInfo curEvent = item.Value;
				foreach(KeyValuePair<string, ADAGEDataPropertyInfo> prop in curEvent.properties)
				{
					if(!currentTarget.versionInfo.eventTypes[item.Key].properties.ContainsKey(prop.Key))
					{
						currentTarget.versionInfo.eventTypes[item.Key].properties.Add(prop.Key, prop.Value);
					}
				}
				
				foreach(KeyValuePair<string, ADAGEDataPropertyInfo> prop in currentTarget.versionInfo.eventTypes[item.Key].properties)
				{
					if(!curEvent.properties.ContainsKey(prop.Key))
					{
						currentTarget.versionInfo.eventTypes[item.Key].properties.Remove(prop.Key);
					}
				}
			}
		}
		
		//Look for any outdated context, as in they are no longer present in the folder
		foreach(KeyValuePair<string, ADAGEEventInfo> savedEvent in currentTarget.versionInfo.eventTypes)
		{
			if(!currentObjects.Contains(savedEvent.Key))
			{
				currentTarget.versionInfo.RemoveEvent(savedEvent.Key);
			}
		}
	}

	/*void Refresh()
	{
		MonoScript current = MonoScript.FromScriptableObject(this);

		ADAGEEditor.ADAGEBasePath = "";
		string[] path = AssetDatabase.GetAssetPath(current).Split('/');
		for(int i = 0; i < path.Length - 2; i++)
			ADAGEEditor.ADAGEBasePath += path[i] + "/";

		if(ADAGEEditor.VersionInfo == null)
			ADAGEEditor.VersionInfo = new ADAGEGameVersionInfo();

		DirectoryInfo info = new DirectoryInfo(ADAGEBasePath + "Custom/Events");
		FileInfo[] fileInfo = info.GetFiles();

		if(fileInfo.Length > 0)
		{		
			foreach (FileInfo file in fileInfo) 
			{
				if(file.Name.Contains(".meta") || (file.Extension != ".cs" && file.Extension != ".js"))
					continue;
				
				string className = file.Name.Substring(0,file.Name.Length - file.Extension.Length);
				Type newType = ReflectionUtils.FindType(className);

				if(ReflectionUtils.CompareType(newType, typeof(ADAGEData)))
				{
					ADAGEEditor.VersionInfo.AddEvent(className, GetEventInfo(newType));
				}
			}
		}

		info = new DirectoryInfo(ADAGEBasePath + "Custom/Context");
		fileInfo = info.GetFiles();

		if(fileInfo.Length > 0)
		{	
			foreach (FileInfo file in fileInfo) 
			{
				if(file.Name.Contains(".meta") || (file.Extension != ".cs" && file.Extension != ".js"))
					continue;
				
				string className = file.Name.Substring(0,file.Name.Length - file.Extension.Length);
				Type newType = ReflectionUtils.FindType(className);
				
				if(ReflectionUtils.CompareType(newType, typeof(ADAGEContext)))
				{
					ADAGEEditor.VersionInfo.AddContext(className, GetEventInfo(newType));;
				}
			}
		}
	}*/
	
	/*private ADAGEEventInfo GetEventInfo(Type type)
	{
		ADAGEEventInfo newEvent = new ADAGEEventInfo();
		
		bool inherit = false;
		
		//Get properties for current type
		PropertyInfo[] propsInfo;
		if(inherit)
			propsInfo = type.GetProperties();
		else
			propsInfo = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		
		foreach (PropertyInfo p_info in propsInfo) 
		{	
			bool skip = false;
			System.Object[] attrs = p_info.GetCustomAttributes(false);
			if(attrs.Length > 0)
			{
				for(int j = 0; (j < attrs.Length && !skip); j++)
				{
					skip = (attrs[j].GetType() == typeof(LitJson.SkipSerialization));
				}
			}

			if (p_info.Name == "Item")
				continue;
			
			ADAGEDataPropertyInfo propertyInfo = null;
			
			if(p_info.PropertyType == typeof(int) || p_info.PropertyType == typeof(float))
			{
				propertyInfo = new ADAGENumericInfo(p_info.PropertyType);
			}
			else if(p_info.PropertyType == typeof(bool))
			{
				propertyInfo = new ADAGEBooleanInfo();
			}
			else if(p_info.PropertyType == typeof(string))
			{
				propertyInfo = new ADAGEStringInfo();
			}
			
			newEvent.properties.Add(p_info.Name, propertyInfo);
		}
		
		//Get fields for current type
		FieldInfo[] fieldsInfo;
		if(inherit)
			fieldsInfo = type.GetFields();
		else
			fieldsInfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		
		foreach (FieldInfo f_info in fieldsInfo)
		{		
			bool skip = false;
			System.Object[] attrs = f_info.GetCustomAttributes(false);
			if(attrs.Length > 0)
			{
				for(int j = 0; (j < attrs.Length && !skip); j++)
				{
					skip = (attrs[j].GetType() == typeof(LitJson.SkipSerialization));
				}
			}

			if(skip)
				continue;

			ADAGEDataPropertyInfo propertyInfo = null;
			
			if(!f_info.Name.Contains("<"))
			{
				if(f_info.FieldType == typeof(int) || f_info.FieldType == typeof(float))
				{
					propertyInfo = new ADAGENumericInfo(f_info.FieldType);
				}
				else if(f_info.FieldType == typeof(bool))
				{
					propertyInfo = new ADAGEBooleanInfo();
				}
				else if(f_info.FieldType == typeof(string))
				{
					propertyInfo = new ADAGEStringInfo();
				}
				
				newEvent.properties.Add(f_info.Name, propertyInfo);
			}
		}
		
		return newEvent;
	}*/

	private void IndentGUI(int amount)
	{
		GUILayout.Space (33 * amount);
	}
}
#endif