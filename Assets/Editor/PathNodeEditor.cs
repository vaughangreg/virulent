using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof (PathNodeV2))]
public class PathNodeEditor : Editor {

	private PathNodeV2 pN;
	
	public void OnSceneGUI()
	{
			
		pN = (PathNodeV2)target;

		List<PathNodeV2> pathList = new List<PathNodeV2>();
		pathList.Add(pN);
		GameObject m_next = pathList[0].nextNode;
		while (m_next != null) {
			pathList.Add(m_next.GetComponent<PathNodeV2>());
			m_next = pathList[pathList.Count - 1].nextNode;			
		}
		
		foreach(PathNodeV2 i in pathList) {
			if(i && i.myPath.Length > 0) {
				foreach (PNodes j in i.myPath) {
					Handles.color = Color.white;
					
					float m_rot = 0;
					if (!i.transformsAreLocal) {
						j.position = Handles.PositionHandle(j.position, Quaternion.identity);
						j.localPosition = Vector3.zero;
					} else { //Handle local transforms
						
						Vector3 pos = Handles.PositionHandle(i.transform.TransformPoint(j.localPosition), Quaternion.identity);
						j.localPosition = i.transform.InverseTransformPoint(pos);
						j.position = pos;
					}
					
					j.position.y = 0;  
					m_rot = j.rotation.eulerAngles.y;
					m_rot = Handles.Disc(Quaternion.Euler(0, m_rot,0), j.position, Vector3.up, 15, false, 1).eulerAngles.y;
					j.rotation = Quaternion.Euler(0, m_rot, 0);
					
					Handles.color = new Color(1,1,0);
					Handles.ArrowCap(8, j.position,Quaternion.Euler(0, j.rotation.eulerAngles.y,0), 15);
				}
			}
		}
	}
}
