using UnityEngine;
using System.Collections.Generic;

public class MoveObjectTo : MonoBehaviour {
	
	/// <summary>
	/// Describes position and direction information for Nodes on MoveObjectTo
	/// </summary>
	[System.Serializable]
	public class Node {
		public Vector3 position = new Vector3();
		public float direction = 0.0f;
		public bool useDirection = true;
		public bool waitHere = false;
		public bool isOpen = false;
		
		public static PNodes ToPNode(Node node)
		{
			PNodes pnode = new PNodes();
			pnode.position = node.position;
			pnode.rotation = Quaternion.Euler(0,node.direction,0);
			return pnode;
		}
		public static Node FromPNode(PNodes pnode)
		{
			Node node = new Node();
			node.direction = pnode.rotation.eulerAngles.y;
			node.position = pnode.position;
			return node;
		}
	}
	
	public MovementLerped usedMovementLerp;
	public bool activeAtStart;
	public bool smoothPath;
	public bool useNodeRotations;
	public bool destroyOnCompletion;
	public MonoBehaviour[] enableOnCompletion;
	[HideInInspector]public bool isDisposableParent;
	[HideInInspector]public MoveObjectTo.Node[] pathNodes = new MoveObjectTo.Node[0];
	
	private int m_nextNodeIndex = 0;
	private bool m_isLastPath = false; 
	
	public void Start()
	{
		if (!usedMovementLerp) {
			usedMovementLerp = GetComponent<MovementLerped>();
			Debug.LogWarning("No referenced MovementLerped", gameObject);
			if (!usedMovementLerp) Debug.LogError("Can't autofind a usable MovementLerped", gameObject);
		}
		Dispatcher.Listen(Dispatcher.MOVE_COMPLETE, gameObject); 
		if (!activeAtStart) enabled = false;
	}
	void OnEnable()
	{
		if (usedMovementLerp) StartMovement();
	}
	/// <summary>
	/// Sets up movement from given points to MovementLerped
	/// </summary>
	public void StartMovement()
	{
		//Setup Movement Lerped
		usedMovementLerp.AddWorldPoint(transform.position, transform.rotation);
		usedMovementLerp.AddWorldPoint(transform.position, transform.rotation);

		for (int i = m_nextNodeIndex; i < pathNodes.Length; i++) {
			m_nextNodeIndex = i+1;
			Node p = pathNodes[i];
			//Add the point to MovementLerped
			if (p.useDirection && useNodeRotations) usedMovementLerp.AddWorldPoint(p.position,Quaternion.Euler(0, p.direction, 0));
			else usedMovementLerp.AddWorldPoint(p.position, gameObject.transform.rotation);
			//Stop Path if p wants to pause
			if (p.waitHere) break;
		}
		if (m_nextNodeIndex >= pathNodes.Length) m_isLastPath = true;
		if (smoothPath) usedMovementLerp.SmoothPath();//Do path smoothing
	}
	
	public void RestartMovement()
	{
		m_nextNodeIndex = 0;
		StartMovement();
	}
	
	void _OnMovementComplete(MessageMovementComplete m)
	{ 
		MovementLerped ml = m.source.GetComponent<MovementLerped>();
		if (ml == null) return;
		if (ml == usedMovementLerp) {
			if (enableOnCompletion != null) {
				if (m_isLastPath) foreach(MonoBehaviour j in enableOnCompletion) j.enabled = true; //enables behaviors on completion
			}
			if (destroyOnCompletion && m_isLastPath) { //Remove the component
				if (isDisposableParent) { //Remove us and detach children if we're the last one
					MoveObjectTo[] array = GetComponents<MoveObjectTo>() as MoveObjectTo[];
					if (array.Length <= 1 && gameObject.transform.childCount > 0) {
						for(int i = 0; i < gameObject.transform.childCount; i++) {
							gameObject.transform.GetChild(i).transform.parent = null;
						}
						Destroy(gameObject);
					}
				}
				Destroy(this);
			}
			else enabled = false;
		}
	}
	
	void OnDrawGizmos()
	{
		Vector3 point = gameObject.transform.position;
		//Draw lines and dots between and on Nodes
		Gizmos.color = Color.white;
		foreach(Node n in pathNodes) {
			Gizmos.DrawLine(point, n.position);
			point = n.position;
		}
		//Draw Position Node things
		foreach(Node n in pathNodes) {
			if (n.waitHere) Gizmos.color = Color.red;
			else Gizmos.color = Color.green;
			Gizmos.DrawSphere(n.position, 8);
		}
	}
}