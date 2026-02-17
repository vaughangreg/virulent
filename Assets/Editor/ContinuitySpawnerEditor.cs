using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ContinuitySpawner))]
public class ContinuitySpawnerEditor : Editor
{
	public ContinuitySpawner cs;
	
	public void OnSceneGUI()
	{
		cs = (ContinuitySpawner)target;
		
		if (cs != null)
		{
			for (int i = 0; i < cs.spawnZones.Count; i ++)
			{
				//Make the position handles only allow movement on the X and Z axis
				cs.spawnZones[i].position = Handles.PositionHandle(cs.spawnZones[i].position,Quaternion.identity);
				cs.spawnZones[i].position.y = 0;
				
				
				Handles.color = Color.white;

				cs.spawnZones[i].yRotation = 
					Handles.Disc(Quaternion.Euler(0,cs.spawnZones[i].yRotation,0),cs.spawnZones[i].position,Vector3.up,20,false,1).eulerAngles.y;
				cs.spawnZones[i].orientation = Quaternion.Euler(0,cs.spawnZones[i].yRotation,0);
				
				//Draw the orientation arrow so we know what direction it's facing
				Handles.color = new Color(1,0.5f,0,1);
				Handles.ArrowCap(1,cs.spawnZones[i].position,cs.spawnZones[i].orientation,15);
				
				GUIStyle style = new GUIStyle();
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				Handles.Label(cs.spawnZones[i].position - new Vector3(0,0,10),"SPAWN POINT ["+i+"]", style);
			}
			
			if (cs.createSecondaryObjects) {
				for (int i = 0; i < cs.secondarySpawnZones.Count; i ++)
				{
					cs.secondarySpawnZones[i].position = Handles.PositionHandle(cs.secondarySpawnZones[i].position,Quaternion.identity);
					cs.secondarySpawnZones[i].position.y = 0;
				
					Handles.color = Color.white;

					cs.secondarySpawnZones[i].yRotation = 
						Handles.Disc(Quaternion.Euler(0,cs.secondarySpawnZones[i].yRotation,0),cs.secondarySpawnZones[i].position,Vector3.up,20,false,1).eulerAngles.y;
					cs.secondarySpawnZones[i].orientation = Quaternion.Euler(0,cs.secondarySpawnZones[i].yRotation,0);
				
					//Draw the orientation arrow so we know what direction it's facing
					Handles.color = new Color(1,0.5f,0,1);
					Handles.ArrowCap(1,cs.secondarySpawnZones[i].position,cs.secondarySpawnZones[i].orientation,15);
					
					GUIStyle style = new GUIStyle();
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					Handles.Label(cs.secondarySpawnZones[i].position - new Vector3(0,0,10),"SECONDARY SPAWN POINT ["+i+"]", style);
				}
			}
		}
	}
	
}

