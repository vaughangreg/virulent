using UnityEngine;

/// <summary>
/// Moves the object between two designated points upon 'SetupPoints'. Can ping-pong.
/// </summary>
[RequireComponent(typeof(MovementLerped))]
public class MoveBetweenTwoPoints : MonoBehaviour {
	public Vector3 pointStart;
	public Vector3 pointEnd;
	// public Vector3 pointOrigin;
	public bool isLocal = false;
	public Vector3 position {
		get { return isLocal ? transform.localPosition : transform.position; }
	}
	
	// Possibly deprecated; use MovementLerped.isLocal instead.
	// public bool    pointsAreLocal            = false;
	
	public bool    shouldPingPong            = true;
	public bool    cycleThenDie              = false;
	public bool    setPointsInEditor         = false;
	public bool    triggerGridRescanAtFinish = false;
	public int     noOfCycles                = 1;
	
	private Quaternion m_rotation;
	
	private int    m_endOfRoutineCounter   = 0;
	private int    m_startOfRoutineCounter = 0;

	public float delayInSeconds       = 3.0f;
	public float delayWiggleInSeconds = 0.5f;

	public float timeToWaitInSec = 0;
	public float timePassedInSec = 0;
	
	[HideInInspector]public Vector3 originalPosition;
	
	protected enum State {
		IdleStart,
		IdleEnd,
		MovingStart,
		MovingEnd
	}
	protected State m_state = State.IdleStart;
	protected MovementLerped moveLerp;
	
	void Awake() {
		originalPosition = transform.localPosition;
	}
	
	/// <summary>
	/// Set starting values; identify needed attached components, add starting location and rotation
	/// </summary>
	void Start() { 
		m_rotation = transform.rotation;
		moveLerp = GetComponent<MovementLerped>();
		timePassedInSec = RandomizeTimeToWait();
		//moveLerp.AddWorldPoint(position, m_rotation);
	}
	
	/// <summary>
	/// Enables the component and sets the points to which it will travel, sets origin.
	/// </summary>
	/// <param name="A">
	/// Starting point
	/// </param>
	/// <param name="B">
	/// Ending point
	/// </param>
	public void SetupPoints(Vector3 A, Vector3 B) {
		pointStart = A;
		pointEnd   = B;
		//Debug.Log("SetupPoints: " + A + "\t" + B, gameObject);
		enabled = true;
	}
	
	/// <summary>
	/// Disables the component and resets timers. Must call 'Setup Points' to continue moving.
	/// </summary>
	public void ClearPoints() {
		enabled = false;
		moveLerp.ClearPath();
		m_state = State.IdleStart;
		timePassedInSec = 0;
	}
	
	/// <summary>
	/// Do appropirate update sequencing according to the current state of the component
	/// </summary>
	void Update () {
		switch (m_state) {
			case State.IdleStart:
				m_state = UpdateStates(State.MovingEnd, pointEnd);
				break;
			case State.IdleEnd:
				m_state = UpdateStates(State.MovingStart, pointStart);
				break;
			default: break;
		} 
	}
	
	/// <summary>
	/// Makes component wait, if its wait is over, it will set a new state and begin movement
	/// </summary>
	/// <param name="aStateMovement">
	/// State to become if component is done waiting
	/// </param>
	/// <param name="pointTarget">
	/// Point to move to when waiting is over
	/// </param>
	/// <returns>
	/// Current state of the component
	/// </returns>
	State UpdateStates(State aStateMovement, Vector3 pointTarget) {
		if (timePassedInSec > timeToWaitInSec) {
			SendMessage("MoveStateChange", SendMessageOptions.DontRequireReceiver);
			MoveTowardsPoint(pointTarget);
			timeToWaitInSec = RandomizeTimeToWait();
			timePassedInSec = 0;
			return aStateMovement;
		}
		timePassedInSec += Time.deltaTime;
		return m_state;
	}
	
	/// <summary>
	/// Move the object to the specified point using MovementLerped
	/// </summary>
	/// <param name="point">
	/// Desired point to travel to
	/// </param>
	void MoveTowardsPoint(Vector3 point) {
		if (shouldPingPong) {
			moveLerp.AddWorldPoint(position, m_rotation);
			//moveLerp.AddWorldPoint(point, m_rotation);
			
			if (isLocal) moveLerp.AddWorldPoint(Vector3.Scale(transform.localScale, point + originalPosition), m_rotation);
			else moveLerp.AddWorldPoint(point, m_rotation);
			
			//if (isLocal) moveLerp.AddWorldPoint(point + originalPosition, m_rotation);
			//else moveLerp.AddWorldPoint(point, m_rotation);
		}
		else {
			if (isLocal) {
				moveLerp.AddWorldPoint(pointStart + originalPosition, m_rotation);
				moveLerp.AddWorldPoint(pointEnd + originalPosition, m_rotation);
			}
			else {
				moveLerp.AddWorldPoint(pointStart, m_rotation);
				moveLerp.AddWorldPoint(pointEnd, m_rotation);
			}
		}

		moveLerp.CalculateSpeed();
	}
	
	/// <summary>
	/// Generates a random value based off of delay and delayWiggle
	/// </summary>
	/// <returns>
	/// The ammount of time in seconds resulting
	/// </returns>
	float RandomizeTimeToWait() {
		return delayInSeconds + Random.Range(-delayWiggleInSeconds, delayWiggleInSeconds);
	}
	
	/// <summary>
	/// Begins waiting.
	/// </summary>
	void OnMovementComplete() {
		switch (m_state) {
			case State.MovingEnd: 
				SendMessage("MoveStateChange", SendMessageOptions.DontRequireReceiver);
				m_state = MoveBetweenTwoPoints.State.IdleEnd;
				m_endOfRoutineCounter++;
				break;
			case State.MovingStart:
				SendMessage("MoveStateChange", SendMessageOptions.DontRequireReceiver);
				m_state = MoveBetweenTwoPoints.State.IdleStart;
				m_startOfRoutineCounter++;
				break;
			default:
				Debug.LogError("Invalid state: " + m_state, gameObject);
				break;
		}
		if (cycleThenDie) { 
			if (!shouldPingPong && m_endOfRoutineCounter > noOfCycles) Destroy(this);
			if (shouldPingPong && m_endOfRoutineCounter >= noOfCycles && m_startOfRoutineCounter >= noOfCycles) Destroy(this);
		} 
	}
	
	void OnDestroy() {
		if (triggerGridRescanAtFinish) {
			AiHelper.RescanWithDelay();
		}
	}
}
