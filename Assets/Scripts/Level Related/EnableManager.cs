using UnityEngine;
using System.Collections.Generic;

public class EnableManager : MonoBehaviour
{
	public bool autoStartLevel = false;
	public bool autoEnableBehaviorList = false;
	public float timeToWaitBeforeEnablingBehaviors = 0.5f;
	
	private static List<GameObject> DisabledObjects = new List<GameObject>();
	public MonoBehaviour[] behaviorsToActivate;
	
	/// <summary>
	/// Listen for the Level Start message to enable objects
	/// </summary>
	void Awake()
	{
		Dispatcher.Listen(Dispatcher.LEVEL_START,gameObject);
	}
	/// <summary>
	/// Send the level start message if autostart is set
	/// </summary>
	void Start()
	{
		if (autoStartLevel) new MessageLevelStart();
	}
	
	public static void AddObject(GameObject go, bool disable)
	{
		DisabledObjects.Add(go);
		if (disable)
		{
			SetActivity(go,false);
		}
	}
	private static void EnableObjects()
	{
		foreach(GameObject go in DisabledObjects)
		{
			SetActivity(go,true);
		}
		DisabledObjects.Clear();
	}
	
	private static void SetActivity(GameObject go, bool on)
	{
		if (go) {
			go.SetActiveRecursively(on);
		}
	}
	
	void _OnLevelStart(MessageLevelStart m)
	{
		//Debug.Log("Level started.");
		EnableObjects();
		Invoke("EnableBehaviors", timeToWaitBeforeEnablingBehaviors); 
	}
	
	void EnableBehaviors() {
		foreach(MonoBehaviour i in behaviorsToActivate) {
			i.enabled = true;
		}
	}
	
}

