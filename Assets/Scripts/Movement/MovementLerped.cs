using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Move along a path using LERPing.
/// </summary>
public class MovementLerped : MonoBehaviour {
	public float speed = 1.0f;
	public float percentCutOnSlow = 0.6f;
	public bool  forcePreviousPointOnCollision = true; 
	public bool  clearPathOnCollision = true;
	public bool  isLocal = false;
	
	public Vector3 position {
		get { return m_trans ? (isLocal ? m_trans.localPosition : m_trans.position) : ( Vector3.zero); }
		set { 
			if (isLocal) m_trans.localPosition = value;
			else m_trans.position = value;
		}
	}
	
	public enum MovementType {
		Lerp,
		Physics
	}
	public MovementType movementType = MovementLerped.MovementType.Lerp;
	public float epsilon = 10.0f;	// In pixels for large maps.
	
	public int pathLength { get { return m_path.Count; } }
	
	public List<Vector3> m_path = new List<Vector3>();
	protected List<Quaternion> m_rotations = new List<Quaternion>();
	public float m_time = 0.0f;
	public float time { get { return m_time; } set{ m_time = value; } }
	protected float m_timeScale;
	public float timeScale { 
		get { 
			if (movementType == MovementLerped.MovementType.Physics) return m_timeScale * TIME_MODIFIER; 
			else return m_timeScale;
		} 
	}
	
	public Vector3 m_prevPoint = Vector3.zero;
	protected Quaternion m_prevRot;
	protected Quaternion m_nextRot;
	
	protected delegate void UpdateDelegate();
	protected UpdateDelegate MovementUpdate;
	
	protected const int LINE_SOLID = 1;
	protected const int LINE_DASHED = 2;
	protected int m_lineAdvance = LINE_SOLID;
	public bool isSolid {
		get { return m_lineAdvance == LINE_SOLID; }	
	}
	
	private const float MULTIPLIER_SPEED = 150.0f;
	private const float TIME_MODIFIER = 0.75f;
	
	protected Vector3 m_directionDelta;
	protected float m_sqrDistanceToNextPoint = 0.0f;
	protected float drag = 2.0f;
	protected int   m_nextSmoothedPoint = 0;
	
	private Transform m_trans;
	
	/// <summary>
	/// Sets up delegates.
	/// </summary>
	void Awake() {
		m_trans = transform;
		
		UpdateDelegates(movementType);
		m_nextRot = m_trans.rotation;
	}
	
	/// <summary>
	/// Updates all delegates based on the new movement type.
	/// </summary>
	/// <param name="aType">
	/// A <see cref="MovementType"/>
	/// </param>
	public void UpdateDelegates(MovementType aType) {
		switch (aType) {
			case MovementType.Lerp:
				MovementUpdate = LerpUpdate;
				break;
			case MovementType.Physics:
				MovementUpdate = ForceUpdate;
				if (!GetComponent<Rigidbody>()) Debug.LogError("MovementLerped: Missing rigidbody for physics movement.", gameObject);
				break;
		}
		
		movementType = aType;
	}
	
	#region Points & Rotations
	/// <summary>
	/// Adds a point in world coordinates.
	/// </summary>
	/// <param name='aPoint'>
	/// A point in world coordinates.
	/// </param>
	public void AddWorldPoint(Vector3 aPoint) {
		if (this.enabled)  m_path.Add(aPoint);
	}
	
	/// <summary>
	/// Adds a position and rotation in world coordinates.
	/// </summary>
	/// <param name="aPoint">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="aQuat">
	/// A <see cref="Quaternion"/>
	/// </param>
	public void AddWorldPoint(Vector3 aPoint, Quaternion aQuat) {
		if (this.enabled) {
			//Debug.Log(gameObject.name + " adding point " + aPoint, gameObject);
			m_path.Add(aPoint);
			m_rotations.Add(aQuat);
		}
	}
	
	/// <summary>
	/// Adds the collection of points to the path.
	/// </summary>
	/// <param name="points">
	/// A <see cref="IEnumerable<Vector3>"/>
	/// </param>
	public void AddWorldRange(IEnumerable<Vector3> points) {
		m_path.AddRange(points);	
	}
	
	/// <summary>
	/// Adds a range of points and quaternions to the path.
	/// </summary>
	/// <param name="points">
	/// A <see cref="IEnumerable<Vector3>"/>
	/// </param>
	/// <param name="quats">
	/// A <see cref="IEnumerable<Quaternion>"/>
	/// </param>
	public void AddWorldRange(IEnumerable<Vector3> points, IEnumerable<Quaternion> quats) {
		m_path.AddRange(points);
		m_rotations.AddRange(quats);
	}
	
	/// <summary>
	/// Returns the last point in the path.
	/// </summary>
	/// <returns>
	/// The point.
	/// </returns>
	public Vector3 lastPoint {
		get { return m_path.Count > 0 ? m_path[m_path.Count - 1] : Vector3.zero; }
	}
	
	/// <summary>
	/// Returns the path count. Note that this follows the C# conventions for properties.
	/// </summary>
	public int Count {
		get { return m_path.Count; }	
	}
	#endregion
	
	public void ClearAIPath() {
		m_path.Clear();
		m_time = 0;
		//if (movementType == MovementLerped.MovementType.Physics) rigidbody.velocity *= 0.5f;
	}
	
	#region Path management
	/// <summary>
	/// Clears the movement path. Called by the input manager.
	/// </summary>
	public void ClearPath() {
		m_path.Clear();
		//m_rotations.Clear();
		m_time = 0;
		m_lineAdvance = LINE_SOLID;
		ZeroVelocity();
		m_nextSmoothedPoint = 0;
		SendMessage("OnPathCleared", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Sets the Lerp values to their end point and time to zero
	/// </summary>
	public void ForceEnd(){
		m_prevPoint = Vector3.zero;

		m_time = 0.0f;
		time = 0.0f;
	}


	/// <summary>
	/// Smooths the input path.
	/// </summary>
	public void SmoothPath() {
		if (m_path.Count < 2) return;
		// Debug.Log("Original path length: " + m_path.Count, gameObject);
		
		Vector3 p0 = m_path[0];
		m_path.RemoveAt(0);
		Vector3[] workingPath = m_path.ToArray();
		workingPath = AstarProcess.PostProcessSplines.CatmullRom(workingPath, 2);
		
		m_path.Clear();
		m_path.Add(p0);
		m_path.AddRange(workingPath);
		m_nextSmoothedPoint = m_path.Count;
		CalculateSpeed();
		// Debug.Log("Smoothed to " + m_path.Count, gameObject);
		m_lineAdvance = LINE_DASHED;
	}
	
	/// <summary>
	/// Smooths the input path.
	/// </summary>
	public void AppendSmoothPath() {
		if (m_path.Count - m_nextSmoothedPoint < 2) return;
		
		if (m_nextSmoothedPoint < 0) SmoothPath();
		Vector3[] workingPath = new Vector3[m_path.Count - m_nextSmoothedPoint];
		System.Array.Copy(m_path.ToArray(), m_nextSmoothedPoint, workingPath, 0, workingPath.Length);
		m_path.RemoveRange(m_nextSmoothedPoint, m_path.Count - m_nextSmoothedPoint);

		workingPath = AstarProcess.PostProcessSplines.CatmullRom(workingPath, 2);
		
		m_path.AddRange(workingPath);
		m_nextSmoothedPoint = m_path.Count;
		CalculateSpeed();
		// Debug.Log("Smoothed to " + m_path.Count, gameObject);
		m_lineAdvance = LINE_DASHED;
	}
	#endregion
	
	#region Speed & Velocity Management
	/// <summary>
	/// Calculates the constant movement speed.
	/// </summary>
	public void CalculateSpeed() {
//		Debug.Log("Path length: " + m_path.Count, gameObject);
		float distance = isLocal 
			? ((m_trans.TransformPoint(m_path[1]) - m_trans.TransformPoint(m_path[0])).magnitude)
			: (m_path[1] - m_path[0]).magnitude;
		if (distance == 0) distance = 0.01f;
		m_timeScale = distance / speed;
//		Debug.Log(gameObject.name + "\td: " + distance + "\tspeed: " + speed + "\tscale: " + m_trans.localScale.magnitude + "\tm_timeScale: " + m_timeScale, gameObject);
		
		if (movementType == MovementLerped.MovementType.Physics) {
			m_directionDelta = m_path[1] - position;
			m_directionDelta.y = 0;
			m_sqrDistanceToNextPoint = m_directionDelta.sqrMagnitude;
			GetComponent<Rigidbody>().drag = drag;
		}
		
		// To smoothly look at the next point
		if (m_trans) m_prevRot = m_trans.rotation;
		
		if (m_rotations.Count > 1) {
			m_nextRot = m_rotations[1];
		}
		else {
			Quaternion oldRotation = m_trans.rotation;
			Vector3 target = m_path[1];
			target.y = position.y;
			m_trans.LookAt(target);
			m_nextRot = m_trans.rotation;
			m_trans.rotation = oldRotation;
		}
		
		if (m_trans) m_trans.rotation = m_prevRot;
	}
	
	/// <summary>
	/// Zeros the velocity.
	/// </summary>
	protected void ZeroVelocity() {
		if (movementType == MovementLerped.MovementType.Physics) GetComponent<Rigidbody>().velocity = Vector3.zero;
		if (enabled) SendMessage("FindClosestTarget", SendMessageOptions.DontRequireReceiver);
	}
	
	/// <summary>
	/// Causes something to slow down.
	/// </summary>
	void OnSlow() {
		speed = (1.0f - percentCutOnSlow) * speed;
	}
	#endregion
	
	#region Updates
	/// <summary>
	/// Move along the path, calculating speed as necessary.
	/// </summary>
	public void Update() {
		m_prevPoint = position;
		m_prevRot   = m_trans.rotation;
		
		if (m_path.Count <= 1) {
			m_time = 0;
			time = 0;
			// if there are no paths freeze velocity and send movement complete message
			if (!GetComponent<Rigidbody>()) ZeroVelocity();
			
			m_trans.rotation = m_nextRot;
			return;
		}
		
		MovementUpdate();
	}
	
	protected void LerpUpdate() {
		if (m_time >= 1.0f) {
			// Debug.Log("Removing " + m_path[0], gameObject);
			m_path.RemoveAt(0);
			if (m_rotations.Count > 0) m_rotations.RemoveAt(0);
			m_time = 0;
			m_nextSmoothedPoint--;
			if (m_path.Count <= 1) {
				new MessageMovementComplete(gameObject);
				SendMessage("OnMovementComplete", SendMessageOptions.DontRequireReceiver);
				

			//	Debug.Log("MOVEMENTCEOMPLELE");
				//Clear path to get rid of jittery path snap at last node
				ClearPath();
				return;
			}
			
			CalculateSpeed();
		}
		
		position = Vector3.Lerp(m_path[0], m_path[1], m_time);
		m_trans.rotation = Quaternion.Lerp(m_prevRot, m_nextRot, m_time);
		
		m_time += Time.deltaTime / m_timeScale;
	}
	
	
	protected void ForceUpdate() {
		Vector3 currentDirectionDelta = m_path[1] - position;
		currentDirectionDelta.y = 0;
		float sqrDistance = currentDirectionDelta.sqrMagnitude;
		
		/*
		bool passedPoint = (Mathf.Sign(currentDirectionDelta.x) != Mathf.Sign(m_directionDelta.x))
			|| (Mathf.Sign(currentDirectionDelta.z) != Mathf.Sign(m_directionDelta.z));*/
		bool passedPoint = 0 > Vector3.Dot(currentDirectionDelta, m_directionDelta);
		
		if (sqrDistance < epsilon * epsilon || passedPoint) {
			m_path.RemoveAt(0);
			if (m_rotations.Count > 0) m_rotations.RemoveAt(0);
			m_time = 0.0f;
			m_nextSmoothedPoint--;
			
			if (m_path.Count <= 1) {
				new MessageMovementComplete(gameObject);
				SendMessage("OnMovementComplete", SendMessageOptions.DontRequireReceiver);
				
				// Clear path to get rid of jittery path snap at last node
				ClearPath();
				return;
			}
			CalculateSpeed();
		}
		m_time = 1.0f - (sqrDistance / m_sqrDistanceToNextPoint) * .95f;
		
		GetComponent<Rigidbody>().AddForce(currentDirectionDelta.normalized * speed * MULTIPLIER_SPEED * Time.deltaTime);
		//HACK: we would like to eventually find the cause of the NaN appearing in the quat
		Quaternion quat = Quaternion.Lerp(m_trans.rotation, m_nextRot, m_time);
		if (float.IsNaN(quat.w)) quat = Quaternion.identity;
		//Assign the rotation
		m_trans.rotation = quat;
	}
	#endregion
	
	#region Event Handlers
	/// <summary>
	/// Ensure a previous point and rotation exists.
	/// </summary>
	void OnEnabled() {
		m_prevPoint = position;
		m_prevRot = m_trans.rotation;
	}
	
	/// <summary>
	/// We don't want to move if we're dying.
	/// </summary>
	void DeathPrep() { 
		enabled = false;
	}
	#endregion
	
	#region Collisions


	private bool collision_logged = false;
	/// <summary>
	/// A simple collision handler. Will need to make this better in the future.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnCollisionEnter(Collision collisionInfo) {
		if (!collision_logged) {
			//Debug.Log (collisionInfo.gameObject.GetComponent<Combat> ());
		}
		collision_logged = true;
		if (movementType == MovementLerped.MovementType.Physics) {
			GetComponent<Rigidbody>().AddForce((position
			                    - collisionInfo.transform.position) * 5f,
			                    //+ m_trans.TransformDirection(Vector3.right) * 5000f),
			                   ForceMode.Force);
			//!collisionInfo.rigidbody || 
			if (collisionInfo.gameObject.layer == LayerMask.NameToLayer("Walls") && clearPathOnCollision) ClearPath();
			return;
		}
		else {
			if (forcePreviousPointOnCollision) {
				position = m_prevPoint;
				m_trans.rotation = m_prevRot;
			}
			if (clearPathOnCollision) ClearPath();
		}
		//Debug.Log("Returning to " + m_prevPoint, gameObject);
	}
	
	/// <summary>
	/// Indicates a problem with collisions.
	/// </summary>
	void OnCollisionStay(Collision collisionInfo) {
		// If this fires, it means we still need to work on the collision detection.
		//if (movementType != MovementLerped.MovementType.Physics) Debug.LogError("" + name + " still hitting " + collisionInfo.gameObject.name, gameObject);
	}

	void OnCollisionExit(Collision collisionInfo){
		collision_logged = false;
	}
	#endregion
	
	#region Path rendering
	/// <summary>
	/// Draw the path.
	/// </summary>
	public void _OnPostRender() {
		if (m_path.Count < 2) return;
		float lineY = GuiResources.POST_RENDER_LINE_HEIGHT;

		GL.Begin(GL.LINES);
		GL.Vertex3(m_trans.position.x, lineY, m_trans.position.z);
		GL.Vertex3(m_path[1].x, lineY, m_path[1].z);
		lineY += 20f;
		for (int i = 2; i < m_path.Count; i++) {
			GL.Vertex3(m_path[i-1].x, lineY, m_path[i-1].z);
			GL.Vertex3(m_path[i].x, lineY, m_path[i].z);
		}
		GL.End();
	}
	#endregion
}
