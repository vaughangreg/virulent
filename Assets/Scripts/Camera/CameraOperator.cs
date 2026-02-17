using UnityEngine;
using System.Collections.Generic;

public class CameraOperator : MonoBehaviour {
	public float movementDistancePerSecond = 240.0f;
	
	protected List<MessageCameraFocus> m_direction = new List<MessageCameraFocus>();
	protected MessageCameraFocus m_currentDirection;
	protected float              m_t = 0.0f;	// Parametric time value
	protected Vector3            m_originalPosition;
	protected float              m_originalZoom;
	protected float              m_timeScalingFactor;
	protected float              m_timeAtStart;
	
	protected delegate void UpdateDelegate();
	protected UpdateDelegate DoDirection;
	
	void Start() {
		Dispatcher.Listen(Dispatcher.CAMERA_FOCUS,         gameObject);
		Dispatcher.Listen(Dispatcher.CAMERA_TRACKING_STOP, gameObject);
		Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED,     gameObject);
		
		DoDirection = UpdateMoveTo;
		m_originalZoom = GetComponent<Camera>().orthographicSize;
		enabled = m_direction.Count > 0;
	}
	
	void OnEnable() {
		if (m_currentDirection == null && DoDirection != null) SelectNextDirection();
	}
	
	void LateUpdate() {
		float delta = (Time.realtimeSinceStartup - m_timeAtStart);
		m_t = delta * m_timeScalingFactor;
		
		DoDirection();
		if (m_currentDirection != null) {
			GetComponent<Camera>().orthographicSize = Virulent.Math.SmoothLerp(m_originalZoom, m_currentDirection.zoomOrthographicSize, m_t);
		}
		
		if (delta > m_currentDirection.durationInSeconds + m_currentDirection.secondsRequired) SelectNextDirection();
	}
	
	void UpdateMoveTo() {
		transform.position = Virulent.Math.SmoothLerp(m_originalPosition, m_currentDirection.targetPosition, m_t);
	}
	
	void UpdateTrack() {
		Vector3 targetPosition = m_currentDirection.targetTransform.position;
		targetPosition.y = transform.position.y;
		transform.position     = Virulent.Math.SmoothLerp(m_originalPosition, targetPosition, m_t);
	}
	
	void SelectNextDirection() {
		if (m_direction.Count < 1) {
			m_currentDirection = null;
			enabled            = false;
			new MessageCameraMovementComplete(gameObject);
			return;
		}
		m_currentDirection = m_direction[0];
		m_originalPosition = transform.position;
		m_t                = 0;
		m_originalZoom     = Camera.main.orthographicSize;
		m_timeAtStart      = Time.realtimeSinceStartup;
			
		float timeRequired  = m_currentDirection.secondsRequired;
		m_timeScalingFactor = 1.0f / timeRequired;
		if (m_currentDirection.shouldTrack) DoDirection = UpdateTrack;
		else DoDirection    = UpdateMoveTo;
		
		if (!Mathf.Approximately(m_currentDirection.timeDistortion, Time.timeScale)) {
			new MessageTimeChange(gameObject, m_currentDirection.timeDistortion);	
		}
		
		m_direction.RemoveAt(0);
	}
	
	/// <summary>
	/// Queues the camera direction.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageCameraFocus"/>
	/// </param>
	void _OnCameraFocus(MessageCameraFocus m) {
		m.targetPosition.y = transform.position.y;
		m_direction.Add(m);
		if (m.shouldUndo) SetupUndo(m);
		enabled = true;
	}
	
	void SetupUndo(MessageCameraFocus m) {
		MessageCameraFocus undoMessage = new MessageCameraFocus(m);
		
		undoMessage.targetPosition       = transform.position;
		undoMessage.zoomOrthographicSize = GetComponent<Camera>().orthographicSize;
		undoMessage.shouldUndo           = false;
		undoMessage.shouldTrack			 = false;
		undoMessage.timeDistortion       = Time.timeScale;
		if (m.shouldTrack) Invoke("StopTracking", m.durationInSeconds);
		
		m_direction.Add(undoMessage);
	}
	
	void StopTracking() {
		new MessageCameraTrackingStop(gameObject);
	}
	
	/// <summary>
	/// Stops tracking an object if needed.
	/// </summary>
	void _OnCameraTrackingStop() {
		if (m_currentDirection != null && m_currentDirection.shouldTrack) {
			SelectNextDirection();
		}
	}
	
	/// <summary>
	/// Ensures we don't try to follow dead objects.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageObjectDestroyed"/>
	/// </param>
	void _OnObjectDestroyed(MessageObjectDestroyed m) {
		for (int i = m_direction.Count - 1; i >= 0; --i) {
			if (ShouldCullObjectAt(m.destroyedObject, i)) m_direction.RemoveAt(i);
		}
		if (m_currentDirection != null && m.destroyedObject.transform == m_currentDirection.targetTransform) {
			SelectNextDirection();	
		}
	}
	
	/// <summary>
	/// Determines if the given object exists at the specified direction index.
	/// </summary>
	/// <param name="anObject">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="anIndex">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	bool ShouldCullObjectAt(GameObject anObject, int anIndex) {
		return (m_direction[anIndex].targetTransform == anObject.transform);	
	}
}
