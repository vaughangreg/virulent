using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour
{

	public GameObject nextNodeInPath;
	public bool ignoreDepth = true;
	public bool ignoreOrientation = true;
	public Vector3 nodePosition;
	public Quaternion nodeOrientation;
	
	private GameObject nextNode;
	private Quaternion orientationToUse;

	void Awake()
	{
		//nodePosition = transform.position;
		nodeOrientation = transform.rotation;
	}

	/// <summary>
	/// Adds the given PathNode do the given MoveObjectTo at the given rotation.
	/// </summary>
	/// <param name="node">
	/// PathNode to add to path
	/// </param>
	/// <param name="mot">
	/// MoveObjectTo that PathNode is added to
	/// </param>
	/// <param name="orientation">
	/// The direction that the object will be facing at this node
	/// </param>
	public static void AddPathNode(PathNode node, MovementLerped moveLerp, Quaternion orientation)
	{
		//Add a point to given MovementLerped path
		print ("Adding Path...");
		
		Vector3 position = node.nodePosition;
		if (node.ignoreDepth) {
			position.y = moveLerp.gameObject.transform.position.y;
		}
		moveLerp.AddWorldPoint (position, orientation);
	}
	
	/// <summary>
	/// Gives specified game object a path (from attached nodes) on it's movementLerped component
	/// </summary>
	/// <param name="go">
	/// GameObject to give path to
	/// </param>
	/// <param name="usingNodeOrientation">
	/// Set 'true' to use node orientations, 'false' for gameObject's personal orientation
	/// </param>
	public void SetPath(GameObject go, bool usingNodeOrientation)
	{
		//Make sure the passed game object has the component
		MovementLerped moveLerp = go.GetComponent<MovementLerped> ();
		if (moveLerp != null) {
			//Find the next path, and continue until null
			AddPathNode (this, moveLerp, this.nodeOrientation);
			
			nextNode = nextNodeInPath;
			if (ignoreOrientation)
				orientationToUse = go.transform.rotation;
			else
				orientationToUse = this.nodeOrientation;

			while (nextNode != null) {
				PathNode pathNode = nextNode.GetComponent<PathNode> ();
				if (pathNode != null) 
				{
					//Update the orientation only if we need to set a new one
					if (!pathNode.ignoreOrientation) 
					{
						if (usingNodeOrientation)
							orientationToUse = pathNode.nodeOrientation;
						else 
							orientationToUse = go.transform.rotation;
					}
					AddPathNode(pathNode, moveLerp, orientationToUse);
					nextNode = pathNode.nextNodeInPath;
				} 
				else {
					Debug.LogError ("Next Path Node (GameObject: " + nextNode.name + ") does not have Path Node Component.", gameObject);
					break;
				}
			}
			//Calculate the speed of the movementlerped
			moveLerp.CalculateSpeed();
		} 
		else Debug.LogError("GameObject " + go.name + " MUST have a 'MovementLerped' component to set path", gameObject);
	}
	
	/// <summary>
	/// Draw the path node positions and a line to the next known path node (if it exists)
	/// </summary>
	void OnDrawGizmos()
	{
		if (!Application.isPlaying) nodePosition = transform.position;

		if (nextNodeInPath) {
			Gizmos.color = new Color(0.4f,1,0.4f,0.8f);
		}
		else Gizmos.color = new Color(1.0f,0.4f,0.4f,0.8f);
		Gizmos.DrawSphere(nodePosition,8);
		if (nextNodeInPath)
		{
			if (nextNodeInPath.GetComponent<PathNode>() != null)
			{
			Vector3 point = nextNodeInPath.GetComponent<PathNode>().nodePosition;
			Gizmos.color = new Color(1,1,1,1);
			Gizmos.DrawLine(nodePosition,point);
			}
		}
	}
}
