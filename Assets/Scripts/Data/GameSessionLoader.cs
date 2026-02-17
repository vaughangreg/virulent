using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

class GameSessionLoader : MonoBehaviour {
	public List<string[]> myLoadedData = new List<string[]>();
	private List<Vector3> myPositions = new List<Vector3>();
	// private List<float> m_times = new List<float>();
	private List<string> m_messageTypes = new List<string>();
	private bool hasChecked = false;
	
    public void Start()
    {
        try
        {
			
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(Application.dataPath + "/test_Level_Info.data"))
            {
                String line;
                // Read and display lines from the file until the end of
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
					string[] lineStrings = line.Split('\t');
					myLoadedData.Add(lineStrings);
                }
            }			
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }
	
	public void Update() {
		if (!hasChecked) {
			hasChecked = true;
			//Debug.Log(myLoadedData.Count);
			for (int i = 1; i < myLoadedData.Count; i++) {
				//Debug.Log(i);
				if (myLoadedData[i][3] == "UnitPath") { 
					Vector3 returnedVector3 = StringToVector3(myLoadedData[i][4]);
					//Vector3 tempVec3 = (Vector3)myLoadedData[i][4]; 
					myPositions.Add(returnedVector3);
				}
			}
			//Debug.Log(myLoadedData);
			
			// foreach(Vector3 i in myPositions) {
			//	Instantiate(someObj, i, Quaternion.identity);
			// }
			
			foreach (string[] i in myLoadedData) {
				if (i != myLoadedData[0] && i != myLoadedData[1]) {
					//m_times.Add(float.Parse(i[0])); 
					m_messageTypes.Add(i[3]);
				}
			}
			//Debug.Log(m_times.Count);
		}
	}
	
	Vector3 StringToVector3(string incString) {
		string temp = "(";
		int j = incString.IndexOf(temp);
        if (j >= 0)
        {
            incString = incString.Remove(j, temp.Length);
        }
		temp = ")";
		j = incString.IndexOf(temp);
        if (j >= 0)
        {
            incString = incString.Remove(j, temp.Length);
        }
		
		/*foreach (char i in incString) {
			if (i.ToString() == "(" || i.ToString() == ")") {
				incString.Replace(i,' ');
				Debug.Log("caught a character");
			}
		}*/
		//incString = incString.Trim();
		//Debug.Log(incString);
		string[] vecStrings = incString.Split(',');
		Vector3 myVector3 = new Vector3(float.Parse(vecStrings[0]) , 
		                                float.Parse(vecStrings[1]), float.Parse(vecStrings[2]) ) ;
		myVector3 = Camera.main.GetComponent<InputManager>().ScreenToWorld(myVector3);
		return myVector3;
	}
	
	float StringToTime(string incString) {
		string[] hoursMinSec = incString.Split(':');
		return (60 * (float.Parse(hoursMinSec[1] + (60f * float.Parse(hoursMinSec[0])))));  // needs to be finished 
	}
}