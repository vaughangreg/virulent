using UnityEngine;

[RequireComponent(typeof(PathNode))]
[RequireComponent(typeof(MoveBetweenTwoPoints))]
[RequireComponent(typeof(RandomizeMesh))]
[RequireComponent(typeof(Selectable))]
public class ProteinReceptor : MonoBehaviour {
	public float movementDistance = 0.7f;
	
	public GameObject movementPrefab;
	
	public bool allowRandomization = true;
	public bool destroyOnConnection = false;
	
	private PathNode m_pathNode;
	private MoveBetweenTwoPoints m_mbtp;
	private bool  m_insideCell = false;
	private bool  m_correctType = true;
	
	private Vector3 m_insidePosition;
	private Vector3 m_outsidePosition;
	
	public MeshFilter currentMeshFilter;
	public MeshRenderer currentMeshRenderer;
	public RandomizeMesh currentRandomizeMesh;

	public Mesh acceptedProtienType;
	
	public AudioClip matchSound;
	public AudioClip mismatchSound;
	
	
	/// <summary>
	/// This will randomize several values of the protein receptor
	/// </summary>
	void Start()
	{	
		if (!currentMeshFilter || !currentRandomizeMesh) {
			Debug.LogError("Object " + gameObject.name + " doesn't have currentMeshFilter or currentRandomizeMesh set in inspector.",gameObject);
		}
		
		m_pathNode = GetComponent<PathNode>();
		m_pathNode.nodeOrientation = transform.rotation;// + new Vector3(0,90*rotationModifier,0);
		m_pathNode.nodePosition = transform.position;//TransformPoint(0,0,0);//-1);
		
		//Setup our MoveBetweenTwoPoints script
		m_mbtp = GetComponent<MoveBetweenTwoPoints>();
		
		//randomize starting position
		Vector3 pointA = transform.position;
		Vector3 pointB = transform.position + transform.forward * movementDistance;
		m_outsidePosition = pointA;
		m_insidePosition = pointB;
		
		if (allowRandomization)
		{
			float rand = Random.value;
			if (rand > 0.5f)
			{
				transform.position = pointB;
				Vector3 temp = pointA;
				pointA = pointB;
				pointB = temp;
				m_insideCell = true;
			}
		}
		m_mbtp.SetupPoints(pointA, pointB);
		
		//Randomly set our protein color/type
		ChangeColor();
		
		//Setup listener
		Dispatcher.Listen(Dispatcher.MOVE_COMPLETE, gameObject);
		Dispatcher.Listen(Dispatcher.BEACON_SIGNAL, gameObject);
	}
	
	/// <summary>
	/// Retracts Protein Receptor into cell, creates dummy, and sends virion to invagination site
	/// </summary>
	/// <param name="virion">
	/// The virion that we're going to be moving
	/// </param>
	void SendToInvaginate(GameObject virion)
	{	
		//Create the mover object and Attach the MoveObjectTo script
		GameObject mover = CreatePRMover(virion);
		m_pathNode.SetPath(mover,true);
		
		print("Mover has " + mover.GetComponent<MovementLerped>().Count +" points on its path");
		
		//Kill the movement of the capsid
		Destroy(virion.GetComponent<Movable>());
		virion.GetComponent<MovementLerped>().ClearPath();
		//Lock rotation from physics of virion, and record the orientation and relative position
		virion.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		virion.SendMessage("LockTransform");
		
		//Snap us back into the cell
		m_mbtp.ClearPoints();
		print("Instantly snap into position");
		
		gameObject.transform.position = m_insidePosition;
		m_insideCell = true;
		
		//Reset our moveBetweenToPoints script
		m_mbtp.SetupPoints(m_insidePosition,m_outsidePosition);
		
		//Destroy us if option was set
		if (destroyOnConnection) Destroy(gameObject);
	}
	
	/// <summary>
	/// Create a dummy protien receptor and set virion to be attached to it
	/// </summary>
	/// <param name="virion">
	/// Virion that collided with original PR
	/// </param>
	/// <returns>
	/// The dummy protein receptor
	/// </returns>
	GameObject CreatePRMover(GameObject virion)
	{
		//Create a dummy object and instantly suck us back into the cell
		GameObject dummy = GameObject.Instantiate(movementPrefab) as GameObject;
		dummy.transform.position = transform.position;
		dummy.transform.rotation = transform.rotation;
		//Set the dummy's mesh to ours
		dummy.GetComponent<MeshFilter>().sharedMesh = currentMeshFilter.sharedMesh;
		
		virion.transform.parent = dummy.transform; //Attach the virion to the mover
		
		return dummy;
	}
	
	/// <summary>
	/// Changes the material (later, change the model...) to a random kind
	/// </summary>
	void ChangeColor()
	{
		if (allowRandomization)
		{
			Mesh mesh = currentRandomizeMesh.Randomize(gameObject,currentMeshFilter,currentMeshRenderer);
			
			//Activate the correct type variable if we are
			if (mesh == acceptedProtienType) m_correctType = true;
			else m_correctType = false;
		}
	}
	
	/// <summary>
	/// Handles only collisions with 'Capsids'
	/// </summary>
	/// <param name="other">
	/// the collider of the game object that we are checking
	/// </param>
	void OnCollisionEnter(Collision other)
	{
		print("Collided With Protein Receptor");
		if (other.gameObject.CompareTag("Capsid"))
		{
			bool childCheck = false;
			if (other.gameObject.transform.parent != null)
				childCheck = true;
			
			if (m_correctType && !childCheck)
			{
				MusicManager.PlaySfx(matchSound);
				//Begin journey of the great invagination
				print("Capsid Moving to Invagination");
				
				//Ignore Collisions
				other.gameObject.layer = LayerMask.NameToLayer("Invagination");
				
				SendToInvaginate(other.gameObject);
			}
			else if (!m_correctType)
			{
				MusicManager.PlaySfx(mismatchSound);
				// TODO: Clean up this comment & add a pivotal entry for feedback
				//Some sort of error action
			}
		}
	}
	void _OnMovementComplete(MessageMovementComplete m)
	{
		if(gameObject == m.source)
		{
			//Change our in or out state, and change color if we are hiding
			m_insideCell = !m_insideCell;
			if (m_insideCell) {
				ChangeColor();
				gameObject.GetComponent<Selectable>().isSelected = false;
			}
			gameObject.GetComponent<Selectable>().isSelectable = !m_insideCell;
			
		}
	}
	
	void _OnBeaconSignal(MessageBeaconSignal m) {
		foreach(string i in m.targetTag) {
			if (i == gameObject.tag && m_insideCell) Invoke("KillBeacons", 0.2f);
		}
		
		/*
		if (m.targetTag == gameObject.tag) {
			if (m_insideCell) {
				Invoke("KillBeacons", 0.2f);
			}
		}*/
	}
	
	void KillBeacons() {
		foreach(ScaleV2 i in gameObject.GetComponentsInChildren<ScaleV2>()) {
			if (i.gameObject.tag == Constants.TAG_BEACON) {
				Destroy(i.gameObject);
			}
		}
	}
	
}
