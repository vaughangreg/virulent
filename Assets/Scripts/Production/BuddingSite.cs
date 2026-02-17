using UnityEngine;

public class BuddingSite : MonoBehaviour {
	public GameObject	product;
	public AudioClip	clipSpawn;
	public GameObject	spawnPoint;
	public float		applyForceAtSpawn = 500f;
	public float		stepTime = 1.0f;
	public CircularMenu menu;
	public ParticleEmitter emitter;
	public Material     enabledMaterial;
	public Material     disabledMaterial;
	
	private int 		m_frame = 0;
	
	//Ted's added values
	public GameObject   	primaryBud;
	public Animation 		utilityAnimation;
	public AnimationClip 	gClip;
	
	public GameObject[] 	models;
	public EnergyContainer 	container;
	
	public GameObject[] 	mProteinSlots;
	private GameObject[] 	m_createdDummies  = new GameObject[4];
	private bool[]			m_positionIsTaken = new bool[4];
	public AnimationClip 	mClip;
	public GameObject       mProteinDummy;
	
	public GameObject		selectableComponent;
	
	bool gainedEnergy = false;
	
	
	/// <summary>
	/// Setup listeners
	/// </summary>
	void Awake() {
		Dispatcher.Listen(Dispatcher.COLLISION_HAPPENS, gameObject);
	}
	
	void Start() {
		for(int i = 1; i < models.Length; i++) {
		 	models[i].active = false;
		 }
		UpdateModels();
	}
	
	/// <summary>
	/// Instantiate a capsid and play a soundclip if available.
	/// </summary>
	public void SpawnCapsid() {
		Instantiate(product, spawnPoint.transform.position, spawnPoint.transform.rotation);
		
		UpdateModels();
	}
	
	/// <summary>
	/// Starts the animation.
	/// </summary>
	public void BeginBudding() {
		// Hide the existing objects.		
		CheckEnergy();
		
		StartCoroutine(StepAnimation());
	}
	
	private void CheckEnergy() {
		Command         budCommand = menu.commands[0];
		
		if (!container.IsEnergySufficient(budCommand.costs)) {
			bool shouldHideG = container.energyAvailable[(int)Constants.Energy.VesicleG] < budCommand.RequiredCostFor(Constants.Energy.VesicleG);
			if (shouldHideG) GProtienDisplay(false);
			else GProtienDisplay(true);
			
			int mAvailable = container.energyAvailable[(int)Constants.Energy.ProteinM];
			MProteinDisplayAtMost(mAvailable);
			
			bool shouldHideGenome = container.energyAvailable[(int)Constants.Energy.Genome] < budCommand.RequiredCostFor(Constants.Energy.Genome);
			if (shouldHideGenome) emitter.emit = false;
		}
	}
	
	/// <summary>
	/// Step through each model of the animation and call SpawnCapsid().
	/// </summary>
	System.Collections.IEnumerator StepAnimation() {
		m_frame = 0;
		
		if (clipSpawn) MusicManager.PlaySfx(clipSpawn);
		while (m_frame < models.Length) {
			models[m_frame].active = true;
			int previousFrame = (m_frame + models.Length - 1) % models.Length;
			models[previousFrame].active = false;
			
			m_frame++;
			yield return new WaitForSeconds(stepTime);
		}
		models[0].active = true;
		models[models.Length - 1].active = false;
		SpawnCapsid();
	}
	
	void _OnCollisionEvent(MessageCollisionEvent m) {
		gainedEnergy = false;
		if (m.collisionObj1 == selectableComponent) { //this acceptable
			if (m.collisionObj2.CompareTag("Vesicle")) {
				//Populate the G proteins if they are not
				ShrinkObject(m.collisionObj2);
				gainedEnergy = true;
			}
			else if (m.collisionObj2.CompareTag("M Protein")) {
				//Snatch up (and kill) the M, create a dummy and send it to an open protein slot
				
				//Locate next availble protein position
				int index;
				Vector3 position = GetMProteinSlot(out index);
				if (position != Vector3.zero) { 
					GameObject g = (GameObject)Instantiate(mProteinDummy, m.collisionObj2.transform.position,
					                                       m.collisionObj2.transform.rotation);
					Transform trans = g.transform;
					StartCoroutine(ScalePositionLerp(trans, trans.localScale, trans.localScale * 0.45f,
					                                 trans.position, mProteinSlots[index].transform.position,
					                                 trans.rotation, mProteinSlots[index].transform.rotation,
					                                 false));
					m_createdDummies[index] = g;
				}
				Destroy(m.collisionObj2);
				gainedEnergy = true;
			}
			else if (m.collisionObj2.CompareTag("Armored Genome")) {
				EnergyCost ec = new EnergyCost();
				ec.amount = 1;
				ec.type = Constants.Energy.Genome;
				container.AddEnergy(new EnergyCost[1]{ec});
				models[0].GetComponent<Renderer>().material = enabledMaterial;
				ShrinkObject(m.collisionObj2);
				gainedEnergy = true;
				emitter.emit = true;
			}
		}
	}
	
	void LateUpdate() {
		//See if we need to send messages
		if (gainedEnergy)  {
			GetBuddingSiteReady();
			gainedEnergy = false;
		}
	}
	
	void ShrinkObject(GameObject anObject) {
		MovementLerped ml    = anObject.GetComponent<MovementLerped>();
		Transform      trans = anObject.transform;
		ml.ClearPath();
		Destroy(anObject.GetComponent<Movable>());
		Destroy(anObject.GetComponent<Selectable>());
		Destroy(ml);
		anObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		StartCoroutine(ScalePositionLerp(trans, trans.localScale, trans.localScale * 0.5f,
		                                 trans.position, selectableComponent.transform.position,
		                                 trans.rotation, selectableComponent.transform.rotation, true));
	}
	
	System.Collections.IEnumerator ScalePositionLerp(Transform aTransform, Vector3 scaleOrigin, 
	                                                 Vector3 scaleTarget, Vector3 positionOrigin,
	                                                 Vector3 positionTarget, Quaternion rotOrigin,
	                                                 Quaternion rotTarget, bool shouldDestroy) 
	{
		float time = 0.0f;
		do {
			time += Time.deltaTime;
			aTransform.localScale = Vector3.Lerp(scaleOrigin, scaleTarget, time);
			aTransform.position   = Vector3.Lerp(positionOrigin, positionTarget, time);
			aTransform.rotation   = Quaternion.Lerp(rotOrigin, rotTarget, time);
			yield return null;
		} while (time <= 1.0f);
		
		if (shouldDestroy) {
		    if (aTransform.CompareTag(Constants.TAG_VESICLE) && !primaryBud.GetComponent<Animation>().isPlaying) {
				GProtienDisplay(true);
			}
			Destroy(aTransform.gameObject);
		}
		else {
			aTransform.localScale = scaleTarget;
			aTransform.position   = positionTarget;
			aTransform.rotation   = rotTarget;
		}
	}
	
	System.Collections.IEnumerator ScaleLerp(Transform aTransform, Vector3 scaleOrigin, 
	                                         Vector3 scaleTarget, MovementLerped aTimer) 
	{
		float time = 0.0f;
		do {
			time = Mathf.Max(aTimer.time, time + 0.001f);
			if (aTransform == null) break;
			aTransform.localScale = Vector3.Lerp(scaleOrigin, scaleTarget, time);
			yield return null;
		} while (time <= 1.0f);
		if (aTransform != null) aTransform.localScale = scaleTarget;
	}
	
	bool GetBuddingSiteReady() {
		bool isReady = true;
		//Get the container stats and modulo them for readiness
		EnergyCost[] costs = menu.commands[0].costs;
		for(int i = 0; i < costs.Length; i++) {
			int cost = costs[i].amount;
			int eng  = container.energyAvailable[(int)costs[i].type];
			if (eng == 0) isReady = false;
			if (eng % cost != 0) isReady = false; //We have not met the cost requirement
		}
		
		//Let the world know we're ready
		if (isReady) new MessageBuddingSiteReady(gameObject, this);
		
		return isReady;
	}
	
	Vector3 GetMProteinSlot(out int index) {
		for(int i=0; i< m_positionIsTaken.Length; i++) {
			if (!m_positionIsTaken[i]) {
				m_positionIsTaken[i] = true;
				index = i;
				return mProteinSlots[i].transform.position;
			}
		}
		index = -1;
		return Vector3.zero;
	}
	
	void ClearProteinSlots() {
		for(int i = 0; i < m_createdDummies.Length; i++) {
			Destroy(m_createdDummies[i]);
			m_positionIsTaken[i] = false;
		}
	}
			    
	void GProtienDisplay(bool show) {
		if (show) {
			utilityAnimation.Rewind(gClip.name);
			utilityAnimation.Play(gClip.name);
		}
		else { //Hide them
			utilityAnimation.Stop();
			gClip.SampleAnimation(utilityAnimation.gameObject, 0);
		}
	}
	
	void MProteinDisplayAtMost(int mRemaining) {
		// Don't need to hide anything if we have sufficient M
		if (mRemaining > m_positionIsTaken.Length) return;
		
		for (int i = m_createdDummies.Length - 1; i >= mRemaining; --i) {
			// Debug.Log("Destroying " + i);
			Destroy(m_createdDummies[i]);
			m_positionIsTaken[i] = false;
		}
	}
	
	void UpdateModels() {		
		emitter.emit = container.energyAvailable[(int)Constants.Energy.Genome] > 0;
		CheckEnergy();
	}
}
