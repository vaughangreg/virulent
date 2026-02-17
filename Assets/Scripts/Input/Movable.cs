using UnityEngine;

/// <summary>
/// Allows players to interact with this object.
/// </summary>
[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(MovementLerped))]
public class Movable : MonoBehaviour {
#if UNITY_WEBPLAYER
	public const int   POINTS_MAX = 150;
#else
	public const int   POINTS_MAX = 75;
#endif
	public AudioClip[] pathClips;
	public GameObject  movementBeaconPrefab;
	public int         initialSmoothingPoint = 0;
	
	protected int            m_addedPoints = 0;
	public    int            addedPoints { get { return m_addedPoints; } }
	protected MovementLerped m_mover;
	protected bool			 m_hasSpoken = false;
	protected GameObject     m_movementBeacon;
	
	/// <summary>
	/// Start listening for post render messages.
	/// </summary>
	void Awake() {
		m_mover = GetComponent<MovementLerped>();
		Dispatcher.ListenPostRender(m_mover);
	}
	
	/// <summary>
	/// Stop listening for messages.
	/// </summary>
	void OnDestroy() {
		m_mover = GetComponent<MovementLerped>();
		Dispatcher.StopListenPostRender(m_mover);
		
		if (m_movementBeacon) Destroy(m_movementBeacon);//m_movementBeacon.active = false;
	}


	private bool path_changed = false;
	/// <summary>
	/// Starts a new path.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStart(InputManager source) {
		
		if (m_mover.Count > 0) {
			path_changed = true;
		} else {
			path_changed = false;
		}
		new MessageUnitPath(gameObject, source.position);

		ClearPath();
		initialSmoothingPoint = 0;
		m_addedPoints = 0;
		AddScreenPoint(source.position);
		m_hasSpoken = false;
		
		GetComponent<Selectable>().SilentlySelect();
		if (m_movementBeacon != null) m_movementBeacon.active = false;
	}
	
	/// <summary>
	/// Adds to the path if possible.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStay(InputManager source) {
		if (m_addedPoints < POINTS_MAX) {
			new MessageUnitPath(gameObject, source.position);
			AddScreenPoint(source.position);
			source.PlayDragSound();
			++m_addedPoints;
			if (!m_hasSpoken && pathClips.Length > 0) {
				int index = Mathf.FloorToInt(Random.value * pathClips.Length);
				MusicManager.PlaySfx(pathClips[index]);
				m_hasSpoken = true;
			}
		}
	}
	
	/// <summary>
	/// Smooths the entered path.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStop(InputManager source) {

		if (initialSmoothingPoint > 0) {
			m_mover.AppendSmoothPath ();
		} else {
			m_mover.SmoothPath ();
		}
		initialSmoothingPoint = m_mover.pathLength;
		CreateBeacon(lastPoint);

		if (path_changed) {
			PathChangeEvent changed = new PathChangeEvent (gameObject.tag.ToString (), gameObject.GetInstanceID ());
			changed.path = m_mover.m_path;
			ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
			ADAGE.LogData<PathChangeEvent> (changed);
		} else {
			PathCreateEvent path_created = new PathCreateEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
			path_created.path = m_mover.m_path;
			ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
			ADAGE.LogData<PathCreateEvent>(path_created);
		}
	}
	
	void CreateBeacon(Vector3 atPoint) {
		if (movementBeaconPrefab == null || m_addedPoints >= POINTS_MAX || m_mover.pathLength < 2) return;
		if (m_movementBeacon == null) {
			m_movementBeacon = (GameObject)Instantiate(movementBeaconPrefab);
			m_movementBeacon.GetComponent<MovableBeacon>().mover = this;
		}
		m_movementBeacon.transform.position = atPoint;
		m_movementBeacon.active = true;
	}
	
	/// <summary>
	/// Adds a point to the movement path. Called by the input manager.
	/// </summary>
	/// <param name='aPoint'>
	/// A point in screen coordinates.
	/// </param>
	public void AddScreenPoint(Vector3 aPoint) {
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(aPoint);
		if (m_mover.Count == 0) m_mover.AddWorldPoint(transform.position);
		m_mover.AddWorldPoint(new Vector3(worldPoint.x, 0, worldPoint.z));
		
		if (m_mover.Count == 2) m_mover.CalculateSpeed();
	}
	
	/// <summary>
	/// Adds a point in world coordinates.
	/// </summary>
	/// <param name='aPoint'>
	/// A point in world coordinates.
	/// </param>
	protected void AddPoint(Vector3 aPoint) { m_mover.AddWorldPoint(aPoint); }
	
	/// <summary>
	/// Returns the last point in the path.
	/// </summary>
	/// <returns>
	/// The point.
	/// </returns>
	public Vector3 lastPoint { get { return m_mover.lastPoint; } }
	
	/// <summary>
	/// Clears the movement path. Called by the input manager.
	/// </summary>
	public void ClearPath() { m_mover.ClearPath(); }
	
	/// <summary>
	/// Hides the beacon.
	/// </summary>
	public void OnMovementComplete() {
		if (m_movementBeacon) m_movementBeacon.active = false;
	}
	
	/// <summary>
	/// Hides beacon if needed.
	/// </summary>
	public void OnPathCleared() {
		if (m_movementBeacon) m_movementBeacon.active = false;
	}
}
