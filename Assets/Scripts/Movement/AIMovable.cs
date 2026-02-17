using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(MovementLerped))]
public class AIMovable : MonoBehaviour {
	public string[] targetTags;
	public bool findAiTarget = false;
	public bool useRadiusToSearch = false;
	public float searchRadius = 25f;
	public bool shouldShowPath = false;
	public PNodes[] patrolPath;
	protected int patrolPathIndex = -1;

	protected Seeker m_seeker;
	protected MovementLerped m_mover;
	protected bool m_hasStarted = false;
	
	protected const int LAYER_NOT_SET = -1;
	
	private static Rect m_searchableScope;
	
	public bool researchDuringPath = false;
	public float researchEveryXSeconds = 1f;
	
	#region Initialization
	void Awake() {
		m_seeker = GetComponent<Seeker>();
		m_mover = GetComponent<MovementLerped>();
		if(shouldShowPath) Dispatcher.ListenPostRender(m_mover);
	}
	
	void OnDestroy() {
		if(shouldShowPath) Dispatcher.StopListenPostRender(m_mover);
	}
	
	/// <summary>
	/// Sets the searchable scope to the A* grid dimensions.
	/// </summary>
	void SetSearchableScope() {
		m_searchableScope = AiHelper.instance.CalcGridDimensions();
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Vector3 topLeftCorner = new Vector3(m_searchableScope.x, 0, m_searchableScope.y);
		Vector3 topRightCorner = new Vector3(m_searchableScope.x + m_searchableScope.width, 0, m_searchableScope.y);
		Vector3 lowerLeftCorner = new Vector3(m_searchableScope.x, 0, m_searchableScope.y + m_searchableScope.height);
		Vector3 lowerRightCorner = new Vector3(m_searchableScope.x + m_searchableScope.width, 0, m_searchableScope.y + m_searchableScope.height);
		
		Gizmos.DrawLine(topLeftCorner, topRightCorner);
		Gizmos.DrawLine(topRightCorner, lowerRightCorner);
		Gizmos.DrawLine(lowerRightCorner, lowerLeftCorner);
		Gizmos.DrawLine(lowerLeftCorner, topLeftCorner);		                
	}
	
	void Start() {		
		SetSearchableScope();
		FindClosestTarget();
	}
	
	#endregion

	#region Handlers
	void OnEnable()
	{
		//Debug.Log("Invoke Find Closest",this);
		m_hasStarted = true;
	}
	#endregion

	private int last_target_id;
	/// <summary>
	/// Finds the closest target with one of the target tags.
	/// </summary>
	public void FindClosestTarget() { 
		if (this.enabled && m_hasStarted) {
			// NOTE: This is currently a hack; eventually,
			// we should search everything
			float closestDistance = float.MaxValue;
			GameObject finalTarget = null;
			
			foreach (string aTag in targetTags) {
				GameObject[] targets = GameObject.FindGameObjectsWithTag (aTag);
				foreach (GameObject aTarget in targets) {
					//Debug.Log(aTarget.tag + " is target");
					float sqrDistance = (aTarget.transform.position - transform.position).sqrMagnitude;
					//Debug.Log("sqrDistance " + sqrDistance + " closestDistance " + closestDistance);
					//Debug.Log("Onscreen is " + OnScreen(aTarget.transform.position));
					if (sqrDistance < closestDistance && OnScreen (aTarget.transform.position)) {
						if (useRadiusToSearch) {
							if (searchRadius * searchRadius > sqrDistance) {
								closestDistance = sqrDistance;
								finalTarget = aTarget;
							}
						} else {
							closestDistance = sqrDistance;
							finalTarget = aTarget;
							//print("Final Target = " + aTarget);
						}
					}
				}
			}
			
			// [2] - Jump to the Patrol path (if there is one)
			if (!finalTarget && patrolPath.Length > 0) {
				//Find nearest patrol point (if we aren't already on the path)
				if (patrolPathIndex < 0) {
					patrolPathIndex = Mathf.FloorToInt(Random.value * patrolPath.Length);
				}
				patrolPathIndex = (patrolPathIndex + 1) % patrolPath.Length;
				MoveToTarget(patrolPath[patrolPathIndex].position);
			}
			
			// [3] - Look for AI targets if nothing was found yet
			else if (!finalTarget && findAiTarget) {
				//Debug.Log(name + " looking for target (" + 
				//          (m_target == null ? "no target" : m_target.name) + ")...", gameObject);
				if (m_target != null && m_target.tag == Constants.TAG_AITarget) {
					// Keep going to the same location.
				//	Debug.Log("Ignoring path.");
					if (researchDuringPath) Invoke("FindClosestTarget", researchEveryXSeconds);
					return;	
				}
				
				GameObject[] targets = GameObject.FindGameObjectsWithTag (Constants.TAG_AITarget);
				foreach (GameObject aTarget in targets) { 
					float sqrDistance = (aTarget.transform.position - transform.position).sqrMagnitude; 
					if (sqrDistance < closestDistance && OnScreen (aTarget.transform.position)) {
						closestDistance = sqrDistance;
						finalTarget = aTarget; 
					}
				}
			}
			
			// We should now have something…assuming a tagged object exists.
			if (finalTarget) {
				if(last_target_id != finalTarget.GetInstanceID()){
					last_target_id = finalTarget.GetInstanceID();
					AITargetEvent retarget = new AITargetEvent(gameObject.tag.ToString(),gameObject.GetInstanceID(),finalTarget.GetInstanceID());
					ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
					ADAGE.LogData<AITargetEvent>(retarget);
				}

				//print("FinalTarget: " + finalTarget);
				m_targetLayerOriginal = finalTarget.layer;
				if (m_targetLayerOriginal != LayerMask.NameToLayer("SearchLayer")) {
					//Debug.Log("setting target");
					m_target = finalTarget;
					SendMessage("SetTargetPosition", finalTarget.transform.position, SendMessageOptions.DontRequireReceiver);
					//print("GotTarget: " + m_target);
					//finalTarget.layer = LayerMask.NameToLayer("SearchLayer");
				}
				else m_targetLayerOriginal = LAYER_NOT_SET;
				MoveToTarget(finalTarget.transform.position);
			}
			else if (!researchDuringPath && patrolPath.Length < 1) Invoke("FindClosestTarget", 0.25f + 0.25f * Random.value);
		}
		
		if (researchDuringPath) Invoke("FindClosestTarget", researchEveryXSeconds);
	}

	/// <summary>
	/// Tests if the object is on screen
	/// </summary>
	/// <param name="position">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	bool OnScreen(Vector3 position) {
		SetSearchableScope();
		return m_searchableScope.Contains(new Vector2(position.x, position.z));
	}

	/// <summary>
	/// Starts searching for a path.
	/// </summary>
	/// <param name='targetPos'>
	/// Target position.
	/// </param>
	void MoveToTarget(Vector3 targetPos) {
		if (m_mover.movementType == MovementLerped.MovementType.Lerp)
		{
			if (m_targetLayerOriginal != LAYER_NOT_SET) {
				AiHelper.Rescan();
			}
		}
		
		m_mover.ClearAIPath();
		m_seeker.StartPath(transform.position, targetPos);
	}
	GameObject m_target;
	LayerMask m_targetLayerOriginal;
	
	/// <summary>
	/// Moves the target back to its original layer.
	/// </summary>
	void RestoreTargetLayer() {
		if (m_targetLayerOriginal != LAYER_NOT_SET && m_target) {
			m_target.layer = m_targetLayerOriginal;
		}
	}
	
	/// <summary>
	/// Callback for the Seeker script. Contains the points we should move to.
	/// </summary>
	/// <param name='points'>
	/// Points.
	/// </param>/
	public void PathComplete(Vector3[] points) {
		m_mover.AddWorldPoint(transform.position);
		m_mover.AddWorldRange(points);
		if (m_target) m_mover.AddWorldPoint(m_target.transform.position);
		
		m_mover.CalculateSpeed();
		RestoreTargetLayer();
	}
	
	/// <summary>
	/// Clears current tags from the array and replaces with a new one.
	/// </summary>
	/// <param name="New Tag">
	/// A <see cref="System.String"/>
	/// </param>
	public void SetNewTags(string newTag) {
		targetTags = new string[]{newTag};
	}
	
	/// <summary>
	/// Clears current tags from the array and replaces with an array of new ones.
	/// </summary>
	/// <param name="newTag">
	/// A <see cref="System.String[]"/>
	/// </param>
	public void SetNewTags(string[] newTag) {
		targetTags = new string[newTag.Length];
		for (int i = 0; i < newTag.Length; i++) {
			targetTags[i] = newTag[i];
		}
	}	
		
	/// <summary>
	/// Called when an error occurs in the Seeker. TODO: Figure out the error.
	/// </summary>
	public void PathError() {
		Debug.LogError("Pathing error; check that " + name + " is on the grid.", gameObject);
		RestoreTargetLayer();
	}
	
	/// <summary>
	/// Shows a line to the target if applicable.
	/// </summary>
	void OnDrawGizmosSelected() {
		//Draw the custom AI Path if we have one...
		if (patrolPath.Length > 0) {
			for (int i = 0; i < patrolPath.Length; i++) {
				Gizmos.DrawSphere(patrolPath[i].position, 5);
				if (i+1 < patrolPath.Length) Gizmos.DrawLine(patrolPath[i].position, patrolPath[i+1].position);
				else Gizmos.DrawLine(patrolPath[i].position, patrolPath[0].position);
			}
		}
		//Draw a custom radius
		if (useRadiusToSearch) {
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(transform.position, searchRadius); 
		}
		if (!m_target) return;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, m_target.transform.position);
		Gizmos.DrawWireCube(m_target.transform.position, new Vector3(3.0f, 3.0f, 3.0f));
		
	}
	
	void OnMovementComplete() {
		m_target = null;
	}
}
