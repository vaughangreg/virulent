using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class PlayerData : MonoBehaviour {
	public int myPlayerId;
	public List<LevelData> myLevels = new List<LevelData>();
}

public class LevelData : MonoBehaviour {
	public List<EventInfo> myEvents = new List<EventInfo>();
	public List<string> listOfEvents = new List<string>();
}

public class EventInfo : MonoBehaviour {
	public GameDataExtractor.eventType myEventType;
	public GameObject mainGameObject;
	public GameObject secondGameObject;
	public DateTime myTime;
	public Vector3 myLocation;
	public string myTextualInfo;
	public bool myBoolState;
}

public class GameDataExtractor : MonoBehaviour {
	
	public enum eventType {
		CollisionEvent,
		ObjectCreated,
		ObjectDestroyed,
		GameOver,
	}
	public List<PlayerData> myPlayers;
	public string levelToExtractDataFrom;
	
	private List<string> filesToParse;
	
	void Start () {
		filesToParse = new List<string>();
		
		string[] fileNames = Directory.GetFiles("Assets/aaDatafiles/");
		foreach (string i in fileNames) {
			int j = i.IndexOf(levelToExtractDataFrom);
			if (j != -1) filesToParse.Add(i);
		}
		
		foreach(string i in filesToParse) {
			PlayerData m_pd = FindOrCreatePlayer(i);
			AddLevelData(m_pd, i);
		}
		
		using (System.IO.StreamWriter targetFile 
		       = new System.IO.StreamWriter(@"Assets/CombinedData.txt", true))
		foreach(PlayerData i in myPlayers) {
			Debug.Log("Exporting data for user :" + i.myPlayerId);
			int lcounter = 0;
			foreach (LevelData j in i.myLevels) {
				foreach(string k in j.listOfEvents) {
					string preString = i.myPlayerId + "\t" + lcounter + "\t";
					targetFile.WriteLine(preString + k);
				}
				lcounter++;
			}
		}
	}
	
	public bool SearchForTargetEvents(string incString) {
		bool test = false;
		foreach (eventType et in Enum.GetValues(typeof(eventType))) {
			if (incString.Contains(et.ToString())) test = true;
		}
		return test;
	}
	
	public void AddLevelData(PlayerData incPD, string incFile) {
		incPD.myLevels.Add(new LevelData());
		LevelData m_ld = incPD.myLevels[incPD.myLevels.Count - 1];
		try
        {
            using (StreamReader sr = new StreamReader(incFile))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
					if (SearchForTargetEvents(line)) {
						m_ld.listOfEvents.Add(line);
					}
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
	}
	
	public PlayerData FindOrCreatePlayer(string fileLocation) {
		List<string> i = new List<string>(fileLocation.Split('/'));
		i.RemoveRange(0,2);
		i = new List<string>(i[0].Split('_'));
		//foreach(string j in i) Debug.Log(j);
		PlayerData pd;
		PlayerData targetPlayerData = null;
		int newPlayerId = Convert.ToInt32(i[0]);
		bool isNewPlayer = true;
		foreach(PlayerData j in myPlayers) {
			if (j.myPlayerId == newPlayerId) {
				isNewPlayer = false;
				targetPlayerData = j;
			}
		}
		if (isNewPlayer) {
			pd = new PlayerData();
			pd.myPlayerId = newPlayerId;
			myPlayers.Add(pd);
			targetPlayerData = myPlayers[myPlayers.Count - 1];
		}
		
		return targetPlayerData;
		
	}	
}
