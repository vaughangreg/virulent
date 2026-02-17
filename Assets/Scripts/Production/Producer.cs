using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Instantiates an object and sets up its destination.
/// </summary>
//[RequireComponent(typeof(Collider))]
public class Producer : MonoBehaviour {
	public AudioClip 	audioNoSpace;
	public int			upgradeLevel = 0;
	public int			upgradeLevelMax = 2;
	public float		productionSpeedPercent = 1.0f;
	public float 		productionDistance = 75.0f;
	public string       unitStartLayerName;
	private float       unitLayerChangeWait = 2.0f;
	public Collider     colliderToIgnoreAtProduction;
	
	protected static Vector3[] m_spawnPoints = {
		Vector3.back, Vector3.left, Vector3.right, Vector3.forward,
		new Vector3(-0.5f, 0.0f, -0.5f),
		new Vector3(-0.5f, 0.0f,  0.5f),
		new Vector3( 0.5f, 0.0f,  0.5f),
		new Vector3( 0.5f, 0.0f, -0.5f)
	};
	
	protected const float COLLISION_RADIUS = 30.0f;
	
	/// <summary>
	/// Produces the type at a spawn point if possible.
	/// </summary>
	/// <param name="aUnitType">
	/// A <see cref="ObjectManager.Unit"/>
	/// </param>
	public virtual bool Produce(ObjectManager.Unit aUnitType) {
		List<Vector3> availablePositions;
		//Check walls and default layers
		availablePositions = GetAvailablePositions(m_spawnPoints,new LayerMask[3] { LayerMask.NameToLayer("Walls"), 
			LayerMask.NameToLayer("TransparentFX"), LayerMask.NameToLayer("Default")});
		if (availablePositions.Count > 0) {
			DoBuild(availablePositions, aUnitType);
			return true;
		}
		
		// now try just walls
		availablePositions = GetAvailablePositions(m_spawnPoints,new LayerMask[2] { LayerMask.NameToLayer("Walls"),
			LayerMask.NameToLayer("TransparentFX")});
		if (availablePositions.Count > 0) {
			DoBuild(availablePositions, aUnitType);
			return true;
		}
		
		//WELL HELLS BELLS, THERE'S NO ROOM AT ALL
		if (audioNoSpace) MusicManager.PlaySfx(audioNoSpace);
		return false;
	}
	
	protected List<Vector3> GetAvailablePositions(Vector3[] positions, LayerMask[] layers) {
		List<Vector3> temp = new List<Vector3>();
		foreach (Vector3 v in positions) {
			Vector3 newOrigin = ClosestPoint(v);
			if (PositionOpen(newOrigin, layers)) {
				temp.Add(newOrigin);
			}
		}
		return temp;
	}
	
	protected void DoBuild(List<Vector3> positions, ObjectManager.Unit unitType) {
		GameObject go = ObjectManager.Build(unitType, transform.position, transform.rotation);
		SendUnitToPosition(go, transform.position, positions[Random.Range(0,positions.Count)]);
	}
	
	/// <summary>
	/// Returns wether the given position is free to produce under the given layer conditions
	/// </summary>
	/// <param name="point">
	/// point to spawn
	/// </param>
	/// <param name="layers">
	/// layers to check
	/// </param>
	/// <returns>
	/// can we spawn here
	/// </returns>
	public bool PositionOpen(Vector3 point, LayerMask[] layers) {
		//Setup the checked layer mask
		int checkedLayerMask = (1<<layers[0]);
		foreach (LayerMask l in layers) { //Bitwise Or those layers, baby
			checkedLayerMask |= (1<<l);
		}
		// This isn't documented well in the Unity docs;
		// the mask here should include layers that ARE
		// checked.
		return (!Physics.CheckSphere(point, COLLISION_RADIUS, checkedLayerMask));
	}
	
	Vector3 ClosestPoint(Vector3 aSpawnPoint) {
		Vector3 direction = aSpawnPoint.normalized;
		
		return transform.position + (direction * productionDistance);
	}
	
	public virtual void OnDrawGizmosSelected() {
		foreach (Vector3 aSpawnPoint in m_spawnPoints) {
			Vector3 newOrigin = ClosestPoint(aSpawnPoint);
			
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(newOrigin, COLLISION_RADIUS);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, newOrigin);
		}
	}
	
	public void SendUnitToPosition(GameObject go, Vector3 startPos, Vector3 endPos) {
		//Make sure the object has a MovementLerped component
		go.transform.position = startPos;
		MovementLerped lerp   = go.GetComponent<MovementLerped>();
		
		//Create a node based on the given positions
		MoveObjectTo.Node nodeGoal = new MoveObjectTo.Node();
		nodeGoal.position          = endPos;
		nodeGoal.direction         = Vector3.Angle(startPos, endPos);
		
		//Disable AI if it exists
		AIMovable m_ai = go.GetComponent<AIMovable>();
		if (m_ai) m_ai.enabled = false;
		
		//Attach a MoveObjectTo to the object and pass it the AI to enable upon completion
		MoveObjectTo mot        = go.AddComponent<MoveObjectTo>();
		mot.usedMovementLerp    = lerp;
		mot.destroyOnCompletion = true;
		mot.enabled   = true;
		mot.pathNodes = new MoveObjectTo.Node[1] { nodeGoal };
		if(m_ai) mot.enableOnCompletion = new MonoBehaviour[1]{m_ai};
		else mot.enableOnCompletion     = new MonoBehaviour[0];
		mot.StartMovement();
		
		//Make the producer ignore the thing it produced for a while
		Collider colliderToIgnore = colliderToIgnoreAtProduction;
		if (!colliderToIgnore) colliderToIgnore = GetComponent<Collider>();
		
		/*Physics.IgnoreCollision(go.collider, colliderToIgnore);
		RestoreCollisionAfterTime rcat = go.AddComponent<RestoreCollisionAfterTime>();// as RestoreCollisionAfterTime; 
		rcat.colliderToRestore = colliderToIgnore;
		rcat.timeToWait        = unitLayerChangeWait;
		rcat.layerToRestore    = go.layer;*/  //Replaced this stuff with the following line:
		if (!go.GetComponent<RestoreCollisionAfterTime>()) 
		{
			int layerToTarget = 0;
			if (Constants.TAG_COLLECTIONS[Constants.COLLECTION_VIRUS].Contains(go.tag)) layerToTarget = LayerMask.NameToLayer("Default");
			else layerToTarget = LayerMask.NameToLayer("Enemies");
			go.AddComponent<RestoreCollisionAfterTime>().Setup(colliderToIgnore, unitLayerChangeWait, layerToTarget);
			go.layer = LayerMask.NameToLayer("Production");
		}
	}
}
