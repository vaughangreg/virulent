using UnityEngine;
using System.Collections;
 
public class AiTargets : MonoBehaviour {
	public bool      moveToAiTargets = false;
	public bool      destroyIncomingAi = false;
	public bool      destroyMyselfOnCollision = false;
	public Vector3[] myAiTargets;
	
	private int m_lastTarget = 0;
	/// <summary>
	/// Raises the trigger enter event.  Performs the appropriate tasks when something with AI collides with it.  
	/// If it is 'moveToAiTargets' then it will move to a new position.... it's a way to create the illusion that the cell parts
	/// are looking for something.
	/// If it is 'destroyIncomingAi', then it will kill any incoming objects that have AI
	/// If it is 'destroyMyselfOnCollision', then it will kill itself when hit by something with AI
	/// </summary>
	/// <param name='someCollider'>
	/// Some collider.
	/// </param>
	void OnTriggerEnter(Collider someCollider) {
		if (someCollider.GetComponent<AIMovable>() )
		   // || (someCollider.transform.parent && someCollider.transform.parent.GetComponent<AIMovable>()))
		{
			if (moveToAiTargets && myAiTargets.Length > 0) {
				m_lastTarget = (m_lastTarget + 1) % myAiTargets.Length;
				transform.position = myAiTargets[m_lastTarget];
			}
			if (destroyIncomingAi) {
				//AIMovable aim = someCollider.GetComponent<AIMovable>();
				//if (aim) aim.enabled = false;
				Destroy(someCollider.gameObject);
			}
			if (destroyMyselfOnCollision) {
				Destroy(gameObject);
			}
		}
	}
}