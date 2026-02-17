using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveBetweenTwoPoints))]
public class MoveBetweenTwoPointsEditor : Editor {
	private MoveBetweenTwoPoints mbtp;
	
	void OnSceneGUI() {
		mbtp = (MoveBetweenTwoPoints)target;
		
		// Only draw the handles if the object has the boolean checked
		if (mbtp.setPointsInEditor) {
			if (!mbtp.isLocal) { // display and record coordinates as they are
				mbtp.pointStart = Handles.PositionHandle(mbtp.pointStart, Quaternion.identity);
				mbtp.pointEnd   = Handles.PositionHandle(mbtp.pointEnd,   Quaternion.identity);
				
				Handles.Label(mbtp.pointStart, "START");
				Handles.Label(mbtp.pointEnd,   "END");
			}
			else { // Points are local, so use local coordinates
				Vector3 hPointA = mbtp.transform.TransformPoint(mbtp.pointStart); // local to world
				Vector3 hPointB = mbtp.transform.TransformPoint(mbtp.pointEnd);   // local to world

				hPointA = Handles.PositionHandle(hPointA, Quaternion.identity);
				hPointB = Handles.PositionHandle(hPointB, Quaternion.identity);

				mbtp.pointStart = mbtp.transform.InverseTransformPoint(hPointA); // world to local
				mbtp.pointEnd   = mbtp.transform.InverseTransformPoint(hPointB); // world to local
				
				Handles.Label(hPointA, "START");
				Handles.Label(hPointB, "END");
				
				Handles.color = Color.blue;
				Handles.DrawLine(hPointA, hPointB);
				
				Vector3 origin = (EditorApplication.isPlaying) ? mbtp.originalPosition : mbtp.transform.localPosition;
				origin = mbtp.transform.position;
				Handles.SphereCap(0, origin, Quaternion.identity, 3.0f);
				Handles.Label(origin, "ORIGIN");
			}
		}
	}
}
