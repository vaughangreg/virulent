using UnityEngine;

public class Transition : MonoBehaviour {
	public bool startInView  = false;
	public bool destroyAtEnd = false;
	
	public GameObject topPanelChild;
	public GameObject bottomPanelChild;
	
	private Panel   m_topPanel;
	private Panel   m_bottomPanel;
	private bool    m_moving = false;
	
	private bool doneAnimating;	
	
	/// <summary>
	/// Holds position information of panels, and enables movement and the like
	/// </summary>
	private class Panel {
		public Vector3              outsidePosition;
		public Vector3              insidePosition;
		public MoveBetweenTwoPoints mbtp;
		public MovementLerped mlp;
		public GameObject           controlledObject;
		
		private float   m_baseOrthographicSize = 287.0f;
		private Vector3 m_cameraPosition;
		private Vector3 m_endLocation;
		
		public Panel(GameObject go, float depth) {
			mbtp                = go.GetComponent<MoveBetweenTwoPoints>();
			mlp                = go.GetComponent<MovementLerped>();
			// mbtp.pointsAreLocal = false;
			mbtp.enabled        = false;
			controlledObject    = go;
		}
		
		public void Setup(bool isTop, bool startInside) {
			// Inside position should always be 0, depth, 0
			insidePosition = Vector3.zero;
			// Debug.Log("Set inside position to " + insidePosition, controlledObject);
			
			//Setup OutsidePosition
			outsidePosition = insidePosition;
			if (isTop) outsidePosition.z += 1.25f * m_baseOrthographicSize;
			else outsidePosition.z       -= 1.25f * m_baseOrthographicSize;
			
			// Debug.Log("Set outside position to " + outsidePosition, controlledObject);
			Vector3 top = Camera.main.ScreenToWorldPoint (new Vector2 (3.1f, 0.0f));
			top.y = 0;
			//Start In correct Position
			// Debug.Log("Start inside: " + startInside, controlledObject);
			if (startInside) {
				controlledObject.transform.localPosition = insidePosition;
				//controlledObject.transform.localPosition = top;
				
				mbtp.SetupPoints(insidePosition, outsidePosition);
				//mbtp.SetupPoints(top, top);
			}
			else {
				controlledObject.transform.localPosition = outsidePosition;
				mbtp.SetupPoints(outsidePosition, insidePosition);
			}
			
			//Debug.Log("Current local position: " + controlledObject.transform.localPosition, controlledObject);
			//Debug.Log("Current world position: " + controlledObject.transform.position, controlledObject);
			
			m_endLocation = mbtp.pointEnd;
			
			//Set the GameObject to the correct scale
			UpdateScale();
		}
		
		/// <summary>
		/// Wraps move between two points's enabled property.
		/// </summary>
		public void EnableMovement() { 
			mbtp.enabled = true; 
			mlp.m_time = 1.0f;
			mlp.time = 1.0f;
		}
		
		/// <summary>
		/// Wraps move between two points's disable property.
		/// </summary>
		public void DisableMovement() { 
			mbtp.enabled = false; 
		}
		
		
		/// <summary>
		/// Moves the object to the end position.
		/// </summary>
		public void ForceEndPosition() { 
			controlledObject.transform.localPosition = Vector3.zero; 

			mlp.m_time = 1.0f;
			mlp.time = 1.0f;
			DisableMovement ();
		}
		
		/// <summary>
		/// Changes the local scale to reflect the current camera ortho size.
		/// </summary>
		public void UpdateScale() {
			Vector3 newScale = Vector3.one * (Camera.main.orthographicSize / m_baseOrthographicSize);
			controlledObject.transform.localScale = newScale;
		}
	}
	
	private bool m_gameIsPaused {
		get { return !Mathf.Approximately(Time.timeScale, 1.0f); }
	}
	
	void Awake() {
		// Ensure that we're on the main camera
		transform.parent   = Camera.main.transform;
		transform.localPosition = Vector3.zero;
		
		
		float relativeDepth = Camera.main.nearClipPlane - 0.1f;
		// Debug.Log("Relative depth: " + relativeDepth);
		
		//m_topPanel    = new Panel(topPanelChild,    relativePosition.y);
		//m_bottomPanel = new Panel(bottomPanelChild, relativePosition.y);
		
		m_topPanel    = new Panel(topPanelChild,    relativeDepth);
		m_bottomPanel = new Panel(bottomPanelChild, relativeDepth);
		
		// Setup listeners
		Dispatcher.Listen(Dispatcher.PAUSE_GAME,gameObject);
		if (destroyAtEnd) Dispatcher.Listen(Dispatcher.MOVE_COMPLETE,gameObject);

		
		m_topPanel.ForceEndPosition();
		m_bottomPanel.ForceEndPosition();
	}
	
	void Start() {
		// Setup the needed positions
		m_topPanel.Setup(true, startInView);
		m_bottomPanel.Setup(false, startInView);
		
		// Begin Movement if the game isn't paused
		enabled = false;	// By default, don't update panel scales.
		if (m_gameIsPaused && !m_moving) { 
			
			m_topPanel.ForceEndPosition();
			m_bottomPanel.ForceEndPosition();
		}
	}
	
	/// <summary>
	/// Make the panels enable their movement
	/// </summary>
	void BeginMovement() {
		m_topPanel.EnableMovement();
		m_bottomPanel.EnableMovement();
		enabled = true;	// Start updating panel scales.
	}
	
	/// <summary>
	/// Changes the scales of the panels.
	/// </summary>
	void Update() {
		transform.localPosition = Vector3.zero;
		m_topPanel.UpdateScale();
		m_bottomPanel.UpdateScale();

	}
	
	/// <summary>
	/// Destroy ourselves if the option was set
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageMovementComplete"/>
	/// </param>
	void _OnMovementComplete(MessageMovementComplete m) {
		// Update the pause button
		if (m.source == topPanelChild || m.source == bottomPanelChild) {
			//Force End Position of panels
			m_topPanel.ForceEndPosition();
			m_bottomPanel.ForceEndPosition();

			//Let the level manager and others know that we are done moving
			if (!doneAnimating) {

				doneAnimating = true;
				new MessageMovementComplete(gameObject);
			}
			
			if (destroyAtEnd) Destroy(gameObject);
		}
	}
	
	/// <summary>
	/// Unpauses the games.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessagePauseGame"/>
	/// </param>
	void _OnPauseGame(MessagePauseGame m) {
		if (!m.paused && !m_moving) { 
			BeginMovement ();
		} else if (m.paused) {
			m_topPanel.ForceEndPosition ();
			m_bottomPanel.ForceEndPosition ();
		} else {

			BeginMovement ();
		}
	}
}
