using UnityEngine;
using System.Collections;

public class ContinuityManager : MonoBehaviour
{
	public GameObject watchedTrigger;
	public string unitTag;
	// Only use unitTag    OR   tagsToListenFor.   NOT BOTH!
	public string[] tagsToListenFor;
	
	public GameObject unitPrefabToSpawn;
	public GameObject secondaryUnitPrefabToSpawn;
	
	public ContinuitySpawner continuitySpawner;
	
	[HideInInspector] public const string PREFIX = "Continuity_";
	[HideInInspector] public const string LEVEL_SELECT = "Level Select";
	[HideInInspector] public const string LAST_LOADED = "LastSceneLoaded";
	[HideInInspector] public const string LEVEL_CHALLENGE_SELECT = "ChallengeSelect";
	
	public int spawnCount;
	public int unitsToTransfer = 0;
	private string location;
	private bool m_gameIsWon = false;
	
	void Awake()
	{
		//Setup Listeners
		Dispatcher.Listen(Dispatcher.COLLISION_HAPPENS, gameObject);
		Dispatcher.Listen(Dispatcher.GAME_OVER, gameObject);
		Dispatcher.Listen(Dispatcher.LEVEL_START, gameObject);
		Dispatcher.Listen(Dispatcher.LOAD_LEVEL, gameObject);
		Dispatcher.Listen(Dispatcher.CONTINUITY_SET, gameObject);
		
		//Get the count from the last level we played
		spawnCount = GetSpawnCount(Application.loadedLevel);
		PlayerPrefs.SetInt(PREFIX + "previousUnitCount", spawnCount);
	}
	
	int GetSpawnCount(int loadedLevel)
	{
		//Debug.Log("hi there");
		if (Application.platform == RuntimePlatform.OSXEditor) return 1;  // Can use just to make sure you get at least 1 virion while playing in the editor
		string previousLevel = PlayerPrefs.GetString(LAST_LOADED,LEVEL_SELECT);
		//print("Last Scene Loaded: " + previousLevel);
		if (previousLevel == LEVEL_SELECT || previousLevel == LEVEL_CHALLENGE_SELECT) {
			//Load the best count we have gotten on the previous scene
			if (Application.loadedLevelName != "Level 0" || Application.loadedLevelName != "Level 1") {	
				//print("Loaded Previous Level Unit Count");
				return PlayerPrefs.GetInt(PREFIX + "Level " + (loadedLevel-2) + "_bestUnitCount", 1);
			}
			else {
#if UNITY_EDITOR
				//If it's level 0, there's only ever going to be 1
				print("Loaded Default 1 value for Unit Count");
				return 1;
#else
				print("Loaded Default 0 value for Unit Count");
				return 0;
#endif
			}
		}
		else {
			//Load the previous count we just made last scene
			print("Loaded Stored previous level count of: " + PlayerPrefs.GetInt(PREFIX + "previousUnitCount",1));
			return PlayerPrefs.GetInt(PREFIX + "previousUnitCount",1);
		}
	}
	
	
	void SpawnUnits(int amount)
	{
		continuitySpawner.SpawnObjects(unitPrefabToSpawn, amount, secondaryUnitPrefabToSpawn, amount);
	}
	
	void _OnCollisionEvent(MessageCollisionEvent m)
	{
		//Check to see that the collision was between the stuff we're watching
		if (m.collisionObj1 == watchedTrigger && m.collisionObj2.CompareTag(unitTag)) {
			//Add to the count
			unitsToTransfer += 1;
		}
		
		// Only use unitTag or tagsToListenFor
		foreach(string i in tagsToListenFor) {
			if (m.collisionObj1 == watchedTrigger && m.collisionObj2.CompareTag(i)) {
				//Add to the count
				unitsToTransfer += 1;
			}
		}
	}
	
	void _OnContinuity(MessageContinuity m) {
		unitsToTransfer = m.amountToTransfer;	
	}
	
	void _OnGameOver(MessageGameOver m)
	{
		if (m.isGameWon)
		{
			m_gameIsWon = true;
			//Record best value
//			print("Game is won");
			string location = PREFIX + Application.loadedLevelName + "_bestUnitCount";
			if (PlayerPrefs.GetInt(location, 0) < unitsToTransfer) {
				PlayerPrefs.SetInt(location, unitsToTransfer);
			}
		}
	}
	void _OnLevelStart()
	{
		SpawnUnits(spawnCount);
	}
	
	void _OnLoadLevel(MessageLoadLevel m)
	{
		//Debug.Log(m.levelToLoad + "  " + Application.loadedLevelName);
		if (m_gameIsWon && m.levelToLoad != Application.loadedLevelName)
		{
			PlayerPrefs.SetInt(PREFIX + "previousUnitCount", unitsToTransfer);
		}
		PlayerPrefs.SetString(LAST_LOADED, Application.loadedLevelName);
		//print(Application.loadedLevelName.ToUpper() + "is last loaded Level");
	}
}

