using UnityEngine;

[System.Serializable]
public class MessageCameraFocus : Message {
	public Transform targetTransform;
	public Vector3   targetPosition;
	public bool      shouldUndo;
	
	public float   durationInSeconds     = 5.0f;
	public float   zoomOrthographicSize  = 288.0f;
	public float   secondsRequired       = 1.0f;
	public bool    shouldTrack           = false;
	public float   timeDistortion        = 1.0f;
	
	public MessageCameraFocus(MessageCameraFocus m) : base(m.source, Dispatcher.CAMERA_FOCUS) {
		targetTransform      = m.targetTransform;
		targetPosition       = m.targetPosition;
		durationInSeconds    = m.durationInSeconds;
		zoomOrthographicSize = m.zoomOrthographicSize;
		secondsRequired      = m.secondsRequired;
		shouldTrack          = m.shouldTrack;
		shouldUndo           = m.shouldUndo;
		timeDistortion       = m.timeDistortion;
	}
	
	public MessageCameraFocus(GameObject aSender, Vector3 aPosition) 
		: this(aSender, aPosition, 1.0f, 1.0f, 5.0f, true, 1.0f) { }
	
	public MessageCameraFocus(GameObject aSender, Vector3 aPosition, float aZoomSize,
	                          float aSecondsRequired, float aDuration, bool aShouldUndo, float aTimeDistortion)
		: base(aSender, Dispatcher.CAMERA_FOCUS) 
	{
		targetTransform      = null;
		targetPosition       = aPosition;
		zoomOrthographicSize = aZoomSize;
		secondsRequired      = aSecondsRequired;
		shouldTrack          = false;
		durationInSeconds    = aDuration;
		shouldUndo           = aShouldUndo;
		timeDistortion       = aTimeDistortion;
		base.Send();
	}
	
	public MessageCameraFocus(GameObject aSender, Transform aTransform)
		: this(aSender, aTransform, 1.0f, 1.0f, true, 5.0f, true, 1.0f) { }
	
	public MessageCameraFocus(GameObject aSender, Transform aTransform, float aZoomSize, float aSecondsRequired, 
	                          bool aShouldTrack, float aDuration, bool aShouldUndo, float aTimeDistortion)
		: base(aSender, Dispatcher.CAMERA_FOCUS) 
	{
		targetPosition       = aTransform.position;
		targetTransform      = aTransform;
		zoomOrthographicSize = aZoomSize;
		secondsRequired      = aSecondsRequired;
		shouldTrack          = aShouldTrack;
		durationInSeconds    = aDuration;
		shouldUndo           = aShouldUndo;
		timeDistortion       = aTimeDistortion;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return System.String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", targetPosition, 
		                            zoomOrthographicSize, secondsRequired, targetTransform, shouldTrack,
		                            durationInSeconds, shouldUndo, timeDistortion);
	}
	
	public void SendManually(GameObject aSource) {
		base.source = aSource;
		base.listenerType = Dispatcher.CAMERA_FOCUS;
		base.functionName = GetFunctionName();
		// Debug.Log("Func name: " + base.functionName);
		base.Send();
	}
}
