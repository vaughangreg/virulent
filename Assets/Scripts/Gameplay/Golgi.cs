using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Golgi : MonoBehaviour
{
	public GameObject 	vesicle;
	public GameObject[] vesicleProteins;
	public Collider 	vesicleCollider;
	
	public Collider 	golgiCollider;
	
	public PathNodeV2 	gProteinPath;
	public GameObject 	gProteinDummyPrefab;
	
	public Animation    mouthAnimation;
	public AnimationState mouthAnimationState { get { return mouthAnimation["Take 001"];}} //<--- May want to change this
	
	public EnergyContainer energyContainer;
	
	private Queue<GameObject> m_dummy = new Queue<GameObject>();
	private MessageCollisionEvent m_collisionEvent;
	
	/// <summary>
	/// Setup listeners and make the Vesicle appear correctly
	/// </summary>
	public void Awake()
	{
		Dispatcher.Listen(Dispatcher.COLLISION_HAPPENS, gameObject);
		Dispatcher.Listen(Dispatcher.OBJECT_CREATED, gameObject);
		//Show the vesicle properly
		PopulateVesicle();
	}

	/// <summary>
	/// Execute when we recieve energy
	/// </summary>
	public void OnAddEnergy()
	{
		//Add a protein to the vesicle
		int energy = PopulateVesicle();
		if (m_dummy.Count > 0) Destroy(m_dummy.Dequeue());
		//Play the opening animation if we have enough energy
		if (energy >= 4 && !mouthAnimation.isPlaying) Invoke("OpenMouth",1.5f);
	}
	
	/// <summary>
	/// Creates a G Protein dummy, and sends it on a path to the Vesicle creator
	/// </summary>
	/// <param name="protein">
	/// The protein that we are basing the dummy off of
	/// </param>
	public void CreateGDummy(GameObject protein)
	{
		GameObject g = Instantiate(gProteinDummyPrefab, protein.transform.position, protein.transform.rotation) as GameObject;
		MovementLerped ml = g.GetComponent<MovementLerped>();
		ml.AddWorldPoint(g.transform.position, g.transform.rotation);
		foreach(Vector3 v in gProteinPath.pathPositions){
			ml.AddWorldPoint(v);
		}
		m_dummy.Enqueue(g);
	}
	
	/// <summary>
	/// When a G runs into the event trigger, send a dummy to the Vesicle and destroy the G
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageCollisionEvent"/>
	/// </param>
	public void _OnCollisionEvent(MessageCollisionEvent m)
	{
		if (m.collisionObj1 == vesicleCollider.gameObject && m.collisionObj2.CompareTag("G Protein")) {
			CreateGDummy(m.collisionObj2);
			//m_collisionEvent = m;
			Destroy(m.collisionObj2);
		}
	}
	/// <summary>
	/// Make sure that the vesicle is showing the right number of proteins and do animations for production
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageObjectCreated"/>
	/// </param>
	public void _OnObjectCreated(MessageObjectCreated m)
	{
		if (m.createdObject.CompareTag(Constants.TAG_VESICLE)) {
			PopulateVesicle();
			//Make the created vesicle ignore our collider
			
			m.createdObject.AddComponent<RestoreCollisionAfterTime>().Setup(golgiCollider, 1.5f, LayerMask.NameToLayer("Default"));
			
			//Ask the mouth to close in time
			Invoke("CloseMouth", 1.0f);
		}
	}
	/// <summary>
	/// Closes the "Golgi Mouth"
	/// </summary>
	public void CloseMouth()
	{
		mouthAnimationState.speed = -1;
		mouthAnimationState.time = mouthAnimationState.length;
		mouthAnimation.Play();
		
	}
	/// <summary>
	/// Opens the "Golgi Mouth"
	/// </summary>
	public void OpenMouth()
	{
		mouthAnimationState.speed = 1;
		mouthAnimationState.time = 0;
		mouthAnimation.Play();
	}
	
	/// <summary>
	/// Makes the vesicle inside the Golgi appear correctly
	/// </summary>
	/// <returns>
	/// Returns the energy in the energy container
	/// </returns>
	public int PopulateVesicle()
	{
		int energy = energyContainer.energyAvailable[(int)Constants.Energy.ProteinG];
		foreach(GameObject g in vesicleProteins) g.active = false;
		if (energy <= 0) { //Break if there is no energy
			vesicle.active = false;
			return energy;
		} else vesicle.active = true;
		
		for (int i = 0; i < energy  && i < vesicleProteins.Length; i++) {
			vesicleProteins[i].active = true;
		}
		return energy;
	}
}

