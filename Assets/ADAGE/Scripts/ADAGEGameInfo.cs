using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ADAGEPlayerInformation
{
	public string identifier;
	public Color color;

	public ADAGEPlayerInformation(){}

	public ADAGEPlayerInformation(string sID, Color color)
	{
		this.identifier = sID;
		this.color = color;
	}
}

[System.Serializable, ADAGE.BaseClass]
public class ADAGEGameInformation : ADAGEData
{
	public Dictionary<string, ADAGEPlayerInformation> players;
}

//[System.Serializable]
public class ADAGEEventInfoDictionary : Dictionary<string, ADAGEEventInfo>{} 

//[System.Serializable]
public class ADAGEGameVersionInfo
{
	//public Dictionary<string, ADAGEContextInfo> virtualContext = new Dictionary<string, ADAGEContextInfo>();
	[LitJson.SkipSerialization]
	public bool dirty;

	//[SerializeField]
	public ADAGEEventInfoDictionary context = new ADAGEEventInfoDictionary();

	//[SerializeField]
	public ADAGEEventInfoDictionary eventTypes = new ADAGEEventInfoDictionary();

	public void AddContext(string newContext, ADAGEEventInfo info)
	{
		dirty = true;
		if(context == null)
			context = new ADAGEEventInfoDictionary();
		context[newContext] = info;
	}

	public void RemoveContext(string badContext)
	{
		if(context == null)
			context = new ADAGEEventInfoDictionary();

		if(context.ContainsKey(badContext))
		{
			context.Remove(badContext);
			dirty = true;
		}
	}

	public void AddEvent(string newEvent, ADAGEEventInfo info)
	{
		dirty = true;
		if(eventTypes == null)
			eventTypes = new ADAGEEventInfoDictionary();

		if(!eventTypes.ContainsKey(newEvent))
			eventTypes.Add(newEvent, info);
		else
			eventTypes[newEvent] = info;
	}

	public void RemoveEvent(string badEvent)
	{
		if(eventTypes == null)
			eventTypes = new ADAGEEventInfoDictionary();
		
		if(eventTypes.ContainsKey(badEvent))
		{
			eventTypes.Remove(badEvent);
			dirty = true;
		}
	}	
}

[System.Serializable]
public class ADAGEContextInfo
{
	public string name;
	public bool selected;

	public ADAGEContextInfo(string name)
	{
		this.name = name;
		selected = false;
	}
}

[System.Serializable]
public class ADAGEDataPropertyInfo
{             
	public static ADAGEDataPropertyInfo Build(Type propertyType)
	{
		if(propertyType.IsArray)
		{
			return new ADAGEArrayInfo(propertyType.GetElementType());
		}
		else if(propertyType == typeof(int))
		{
			return new ADAGEIntegerInfo();
		}
		else if(propertyType == typeof(float))
		{
			return new ADAGEFloatInfo();
		}
		else if(propertyType == typeof(bool))
		{
			return new ADAGEBooleanInfo();
		}
		else
		{
			return new ADAGEStringInfo();
		}
	}

	[SerializeField]
	public string type;

	[LitJson.SkipSerialization]
	public System.Type Type
	{
		get
		{
			return m_type;
		}

		set
		{
			m_type = value;
			type = m_type.ToString();
		}
	}
	
	#if UNITY_EDITOR
		[LitJson.SkipSerialization]
		protected bool showing = false;
	#endif

	[LitJson.SkipSerialization]
	private System.Type m_type;

	public virtual void DrawLabel(string name)
	{
		
		#if UNITY_EDITOR
		showing = EditorGUILayout.Foldout(showing, name + " : " + Type.ToString());
		
		if(showing)
		{
			DrawContents();
		}
		
		#endif
	}

	public virtual void DrawContents()
	{
		#if UNITY_EDITOR
		EditorGUILayout.LabelField("There are no properties to define.");
		#endif // UNITY_EDITOR
	}
}

[System.Serializable]
public class ADAGEBooleanInfo : ADAGEDataPropertyInfo 
{
	public ADAGEBooleanInfo()
	{
		this.Type = typeof(bool);
	}
}

[System.Serializable]
public class ADAGEStringInfo : ADAGEDataPropertyInfo
{
	[SerializeField]
	public List<string> acceptableValues = new List<string>();

	private int acceptableValuesCount
	{
		get
		{
			if(acceptableValues != null)
				return acceptableValues.Count;
			return 0;
		}

		set
		{
			int size = value;

			if(size < 0)
				size = 0;
			else if(size > 9999)
				size = 9999;

			if(acceptableValues == null)
				acceptableValues = new List<string>(size);
			else
			{
				if(acceptableValues.Count < size)
				{
					while(acceptableValues.Count < size)
					{
						acceptableValues.Add ("");
					}
				}
				else if(acceptableValues.Count > size)
				{
					while(acceptableValues.Count > size)
					{
						acceptableValues.RemoveAt(acceptableValues.Count - 1);
					}
				}
			}
		}
	}

	[LitJson.SkipSerialization]
	string editingValue;

	[LitJson.SkipSerialization]
	string lastFocusedControl;
	
#if UNITY_EDITOR
	[LitJson.SkipSerialization]
	private bool showingValues = false;
#endif

	public ADAGEStringInfo()
	{
		this.Type = typeof(string);
	}

	public override void DrawContents()
	{
		#if UNITY_EDITOR
		{
			int currentIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel++;

			showingValues = EditorGUILayout.Foldout(showingValues, "Possible Values");

			if(showingValues)
			{
				EditorGUI.indentLevel++;
				acceptableValuesCount = EditorGUILayout.IntField("Size", acceptableValuesCount, GUILayout.ExpandWidth(false));

				for(int i = 0; i < acceptableValues.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					{
						acceptableValues[i] = EditorGUILayout.TextField(string.Format("Element {0}", i), acceptableValues[i], GUILayout.ExpandWidth(false)); 
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUI.indentLevel = currentIndent;
		}
		#endif // UNITY_EDITOR
	}
}

[System.Serializable]
public class ADAGEArrayInfo : ADAGEDataPropertyInfo
{
	[SerializeField]
	public int maxSize = -1;

	public ADAGEDataPropertyInfo elementInfo;

	public ADAGEArrayInfo(Type elementType)
	{
		Type = elementType;
		elementInfo = ADAGEDataPropertyInfo.Build(elementType);
	}

	public override void DrawLabel(string name)
	{
		#if UNITY_EDITOR
		showing = EditorGUILayout.Foldout(showing, name + " : " + Type.ToString() + "[]");
		
		if(showing)
		{
			DrawContents();
		}
		#endif // UNITY_EDITOR
	}

	public override void DrawContents()
	{
		#if UNITY_EDITOR
		
			EditorGUI.indentLevel++;
				maxSize = EditorGUILayout.IntField("Max Size", maxSize, GUILayout.ExpandWidth(false));
				maxSize = Mathf.Clamp(maxSize, -1, int.MaxValue);
			EditorGUI.indentLevel--;
			
			elementInfo.DrawContents();

		#endif // UNITY_EDITOR
	}
}

[System.Serializable]
public class ADAGEFloatInfo : ADAGEDataPropertyInfo
{
	[SerializeField]
	public float minValue = 0.0f;
	
	[SerializeField]
	public float maxValue = 0.0f;
	
	public ADAGEFloatInfo()
	{
		Type = typeof(float);
	}
	
	public override void DrawContents()
	{
		#if UNITY_EDITOR
		{
			int currentIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel++;
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(150f));
				{
					minValue = Mathf.Clamp(EditorGUILayout.FloatField("Minimum", minValue, GUILayout.ExpandWidth(false)), float.MinValue, float.MaxValue);
					maxValue = Mathf.Clamp(EditorGUILayout.FloatField("Maximum", maxValue, GUILayout.ExpandWidth(false)), float.MinValue, float.MaxValue);
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUI.indentLevel = currentIndent;
		}
		#endif // UNITY_EDITOR
	}
}

[System.Serializable]
public class ADAGEIntegerInfo : ADAGEDataPropertyInfo
{
	[SerializeField]
	public int minValue = 0;

	[SerializeField]
	public int maxValue = 0;
	
	public ADAGEIntegerInfo()
	{
		Type = typeof(int);
	}
	
	public override void DrawContents()
	{
		#if UNITY_EDITOR
		{
			int currentIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel++;
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(150f));
				{
					minValue = EditorGUILayout.IntField("Minimum", minValue, GUILayout.ExpandWidth(false));
					maxValue = EditorGUILayout.IntField("Maximum", maxValue, GUILayout.ExpandWidth(false));

					Mathf.Clamp(minValue, int.MinValue, int.MaxValue);
					Mathf.Clamp(maxValue, int.MinValue, int.MaxValue);
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUI.indentLevel = currentIndent;
		}
		#endif // UNITY_EDITOR
	}
}

[System.Serializable]
public class ADAGEDataPropertyInfoDictionary : Dictionary<string, ADAGEDataPropertyInfo>{}

[System.Serializable]
public class ADAGEEventInfo
{
	[LitJson.SkipSerialization]
	public bool showing = false;

	[LitJson.SkipSerialization]
	public bool selected = false;

	[SerializeField]
	public ADAGEDataPropertyInfoDictionary properties = new ADAGEDataPropertyInfoDictionary();
}