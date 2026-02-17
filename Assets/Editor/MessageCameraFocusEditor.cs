using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MessageCameraFocus))]
public class MessageCameraFocusEditor : Editor {
	public override void OnInspectorGUI () {
		/*
		MessageCameraFocus message = (MessageCameraFocus)target;
		message.source             = null;
		message.listenerType       = Dispatcher.CAMERA_FOCUS;
		message.functionName       = message.GetFunctionName();
		
		message.targetTransform = (Transform)EditorGUILayout.ObjectField(message.targetTransform, typeof(Transform));
		message.targetPosition  = EditorGUILayout.Vector3Field("Target Position", message.targetPosition);
		message.zoomPercentage  = EditorGUILayout.FloatField("Zoom Percentage", message.zoomPercentage);
		message.secondsRequired = EditorGUILayout.FloatField("Seconds Required", message.secondsRequired);
		message.shouldTrack     = EditorGUILayout.Toggle("Should Track", message.shouldTrack);	
		*/
		base.DrawDefaultInspector();
	}
}
