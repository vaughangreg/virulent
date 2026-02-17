using UnityEngine;

public class PRR : MonoBehaviour {
	public GameObject MProteinPrefab;
	public Transform killZone;
	public Transform childPRR;
	public Transform childPRRFinalPos;
	public AudioClip proteinGrabClip;
	public float animationSpeed = 6f;
	public float speedCutOnGrab = 2f;
	
	GameObject m_dummy;
	Vector3 m_targetPosition;
	
	void Start() {
		GetComponent<Animation>().wrapMode = WrapMode.ClampForever;	
	}
	
	void GrabProtein(GameObject protein) {
		
		LocalMovementLerp lml = childPRR.gameObject.AddComponent<LocalMovementLerp>();
		lml.SetTarget(childPRRFinalPos.localPosition, 1f);
		
		//Cut movement speed by 1/2
		MovementLerped ml = GetComponent<MovementLerped>();
		ml.speed /= speedCutOnGrab;
		
		//Instantiate a dummy protein
		m_dummy = Instantiate(MProteinPrefab) as GameObject;
		m_dummy.transform.position = killZone.position;
		m_dummy.transform.localRotation = protein.transform.rotation;
		m_dummy.GetComponent<Spin>().enabled = true;
		
		m_dummy.transform.parent = childPRR.gameObject.transform;
		
		//Start the grab animation...
		if (!GetComponent<Animation>().isPlaying) {
			if (proteinGrabClip) MusicManager.PlaySfx(proteinGrabClip);
			GetComponent<Animation>().Play();
			foreach(AnimationState state in GetComponent<Animation>()) {
				state.speed = animationSpeed;
			}
		}
		
		//Change the search radius to make sure it will go to the DNA
		AIMovable aim = GetComponent<AIMovable>();
		aim.useRadiusToSearch = false;
		aim.researchDuringPath = true; 
		aim.patrolPath = new PNodes[0];
		
		//Change the navigation tags
		string[] newTags = new string[1] {Constants.TAG_DNA};
		aim.targetTags = newTags;
		ml.ClearPath(); //Clear movement lerped
		GetComponent<DeathGeneric>().preservedChild = m_dummy;
		
	}
	
	/// <summary>
	/// Sets the target and calls FindClosestChomper.  Called from AIMOvable in a SendMessage.
	/// </summary>
	/// <param name="target">
	/// A <see cref="Vector3"/>
	/// </param>
	void SetTargetPosition(Vector3 target) {
		m_targetPosition = target;
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(m_targetPosition, transform.position);
	}
	
}
