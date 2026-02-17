using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Watches for specified events and sends completion message when its conditions are met
/// </summary>
public class Objective : MonoBehaviour
{
	public enum CheckType
	{
		Collisions,
		TimeCounter,
		PopulationCount,
		PopulationEquation,
		Level10PopulationEq
	}
	[HideInInspector]public CheckType objectiveType;

	protected delegate void ObjectiveDelegate ();
	protected ObjectiveDelegate CheckObjective;
	// this is assigned a function based on what type of objective this is
	public string[] watchedTag;
	// the tag to look for when checking tags
	public GameObject watchedTrigger;
	// a collider object for counting collisions
	public int[] quantity;	
	// the goal quantity for objective completion
	
	public bool useEitherOr = false;
	//check this if you want to do one tag or another   !!!!!!! This only works for collisions
	public string[] eitherOrTags;
	//use this if you want to do one tag or another
	public int[] eitherOrQuantity;
	//use this in coordination with the eitherOrTags
	
	
	public bool isComplete = false;
	// true when objective parameters have been met
	public float invokeDelay = 0.1f;

	private float nextTimeToInvoke = 0.1f;

	public bool checkEnergyofContainer = false;
	public EnergyContainer[] energyContainerToCheck;
	public int energyQuantity = 0;

	public AudioClip achievementClip;
	
	public bool sendTextToGui = false;
	public string preText = "";
	public string postText = ""; 
	
	public GameObject[] activateOnCompletion;

	private int[] m_count;
	// when count == quantity objective is complete
	/// <summary>
	/// Calls SetType.
	/// </summary>
	void Awake ()
	{
		SetType ();
		//Debug.Log("Either Or is equal to:    " + useEitherOr);
		if(!useEitherOr) {
			m_count = new int[watchedTag.Length];
			for (int i = 0; i < watchedTag.Length; i++) {
				m_count[i] = (int)0;
			}
		}
		if(useEitherOr) {
			m_count = new int[eitherOrTags.Length];
			for (int i = 0; i < eitherOrTags.Length; i++) {
				m_count[i] = (int)0;
				//Debug.Log(m_count.Length + "   Is the length of the count");
			}
		}
	}
	 
	#region Listener Methods

	/// <summary>
	/// For each CollisionEvent increase count by one
	/// </summary>
	void _OnCollisionEvent (MessageCollisionEvent m)
	{
		if (enabled) {
			for (int i = 0; i < watchedTag.Length; i++) {
				if (m.collisionObj2.tag == watchedTag[i] && m.collisionObj1 == watchedTrigger) {
					m_count[i]++;
					CheckObjective ();
				}
			}
			if(useEitherOr) {
				
			for (int i = 0; i < eitherOrTags.Length; i++) {
				if (m.collisionObj2.tag == eitherOrTags[i] && m.collisionObj1 == watchedTrigger) {
					m_count[i]++;
					CheckObjective ();
				}
			}
			}
		}
	}

	/// <summary>
	/// On ObjectCreated update Count to a population and check for completion
	/// </summary>
	void _OnObjectCreated (MessageObjectCreated m)
	{
		if (enabled) {
			//if (objectiveType == Objective.CheckType.PopulationEquation ||
			    //objectiveType == Objective.CheckType.Level10PopulationEq) 
			AddDelay(0.1f);
			CheckObjective ();
		}
	}

	/// <summary>
	/// On ObjectDestroyed update Count to a population and check for completion
	/// </summary>
	void _OnObjectDestroyed (MessageObjectDestroyed m)
	{
		if (enabled) {
			//if (objectiveType == Objective.CheckType.PopulationEquation ||
			    //objectiveType == Objective.CheckType.Level10PopulationEq) 
			AddDelay(0.1f);
			CheckObjective ();
		}
	}

	void _OnBuildCommandStarted (MessageBuildCommandStarted m)
	{
		if (objectiveType == Objective.CheckType.PopulationCount) {
			foreach (EnergyContainer i in energyContainerToCheck) {
				if (i.gameObject == m.producerObj) {
					//Debug.Log("Adding a delay of : " + m.timeNeededToBuild);
					AddDelay (m.timeNeededToBuild + 1.1f);
				}
			}
		} 
		else if (objectiveType == Objective.CheckType.PopulationEquation ||
		         objectiveType == Objective.CheckType.Level10PopulationEq) {
			if (Constants.TAG_COLLECTIONS[Constants.COLLECTION_PRODUCERS].Contains(m.producerObj.tag) &&
				Constants.TAG_COLLECTIONS[Constants.COLLECTION_VIRUS].Contains(ObjectManager.instance.unitPrefabs[(int)m.unit].tag)) {
				
				//Delay the PopulationEquation check
				AddDelay(m.timeNeededToBuild + 1.1f);
			}
		}
		
	}
	#endregion

	#region Functions assigned to delegate CheckObjective

	/// <summary>
	/// Checks to see if there have been enough collisions to complete the objective
	/// </summary>
	void CheckCollisionCount () {
		if (enabled) {
			if (achievementClip) MusicManager.PlaySfx (achievementClip);
			bool myCheck = true;
			//Debug.Log(name + " checking collision count...", gameObject);
			for (int i = 0; i < watchedTag.Length; i++) {
				//Debug.Log(sendTextToGui + "       " + preText + m_count[i] + postText);
				if (sendTextToGui) new MessageCheckPoint(gameObject, preText + m_count[i] + postText, new AudioClip());
				if (m_count[i] < quantity[i]) {
					myCheck = false;
				}
			}
			if(useEitherOr) {
				myCheck = false;
				for (int i = 0; i < eitherOrTags.Length; i++) {
					if (m_count[i] >= eitherOrQuantity[i]) {
						myCheck = true;
					}
				}
			}
			
			//Debug.Log(name + " result: " + myCheck, gameObject);
			isComplete = myCheck;
			if (isComplete) CompletedObjective();
		}
	}

	/// <summary>
	/// A delayed check of the specified population.  Delayed to avoid checking before population has been updated.
	/// </summary>
	void CheckPopulation()
	{
		//Debug.Log(name + " checking population.", gameObject);
		if (checkEnergyofContainer) {
			if (CheckEnergy ()) return;
		}
		if (enabled) {
			//Debug.Log(name + ": " + Time.time + "   " + nextTimeToInvoke);
			if (Time.time < nextTimeToInvoke) {
				// Debug.Log("waiting");
				// return;
				
				// We don't check until something else happens, so...invoke.
				CancelInvoke("DelayCheck");
				Invoke("DelayCheck", nextTimeToInvoke - Time.time + 0.01f);
				return;
			}
			
			// if the time is fine, shouldn't we just check?
			DelayCheck();
			
			//Invoke ("DelayCheck", 0.1f);
			//Debug.Log("Checking bc the time is fine");
		}
	}
	
	void DelayCheck ()
	{
		if (Time.time < nextTimeToInvoke) return;
		bool myCheck = true;
		for (int i = 0; i < watchedTag.Length; i++) {
			if (ObjectManager.instance.GetCount (watchedTag[i]) != quantity[i]) {
				myCheck = false;
			}
		}
		//Debug.Log(myCheck);
		if (myCheck) {
			isComplete = true;
			CompletedObjective();
		}
	}

	bool CheckEnergy ()
	{
		if (isComplete) return false; //added in to remove an error when the level is ended
		bool myCheck = false;
		foreach (EnergyContainer eC in energyContainerToCheck) {
			if (eC && eC.gameObject.active) { //check for eC's existence added in to remove error when ending level in editor
				foreach (int i in eC.energyAvailable) {
					if (i > energyQuantity) {
						myCheck = true;
					}
				}
			}
		}
		return myCheck;
	}
	
	void CheckLossByPopulationEquation()
	{
		//Escape if a delay was requested
		if (nextTimeToInvoke > Time.time) { 
			Invoke("CheckLossByPopulationEquation", nextTimeToInvoke - Time.time);  
			return;
		}
		//Find all player production units
		List<GameObject> genome = GameObject.FindGameObjectsWithTag(Constants.TAG_GENOME).ToList();
		genome.AddRange(GameObject.FindGameObjectsWithTag(Constants.TAG_ARMORGENOME).ToList());
		
		List<GameObject> antiGenome = GameObject.FindGameObjectsWithTag(Constants.TAG_ANTIGENOME).ToList();
		antiGenome.AddRange(GameObject.FindGameObjectsWithTag(Constants.TAG_ARMORANTIGENOME).ToList());
		
		//Get the number of upgraded genome units
		int count = GetUpgradedCount(genome);
		
		//Get the count of energy in the 
		List<GameObject> buddingSite = GameObject.FindGameObjectsWithTag(Constants.TAG_BUDDINGSITE).ToList();
		
		//Use Nathan's population equation check
		ObjectManager om = ObjectManager.instance;
		int lpCount = om.GetCount(Constants.TAG_MRNALP) + om.GetCount(Constants.TAG_PROTEINLP);
		
		int energy = 0;
		int mCount       = om.GetCount(Constants.TAG_PROTEINM);
		int vesicleCount = om.GetCount(Constants.TAG_VESICLE);
		
		foreach(GameObject g in buddingSite) {
			//energy += g.GetComponent<EnergyContainer>().energyAvailable[(int)Constants.Energy.Capsid];
			EnergyContainer battery = g.GetComponent<EnergyContainer>();
			
			int energyVesicle = (battery.energyAvailable[(int)Constants.Energy.VesicleG] + vesicleCount);
			int energyM       = (battery.energyAvailable[(int)Constants.Energy.ProteinM] + mCount) / 4;
			int energyGenome  = (battery.energyAvailable[(int)Constants.Energy.Genome] + count);
			energy += Mathf.Min(energyVesicle, energyM, energyGenome);
		}
		
		// A genome exists or a genome + LP exists
		bool genomeOk     = (GetUpgradedCount(genome) > 0) || (genome.Count > 0 && lpCount > 0);
		// An upgraded antigenome + LP exists or an antigenome + 2 LP exist
		bool antigenomeOk = (GetUpgradedCount(antiGenome) > 0 && lpCount > 0)
			|| (antiGenome.Count > 0 && lpCount > 1);
		// Virions exist or there's enough energy to create one.
		bool virionsOk    = om.GetCount(Constants.TAG_CAPSID) > 0 || energy > 0;
		
		// If none of the above are OK, then we're done.
		if (!(genomeOk || antigenomeOk || virionsOk)) {
			isComplete = true;
			new MessageObjectiveComplete(gameObject); //<--  Objective was 'completed'
		}
	}
	
	void Check10Status() {
		//Wait if nextTimeToInvoke is set
		if (nextTimeToInvoke > Time.time) { 
			Invoke("Check10Status", nextTimeToInvoke - Time.time);  
			return;
		}
		bool noGenome = false;
		bool noLP = false;
		bool noUpgrade = false;
		ObjectManager om = ObjectManager.instance;
		//No LP stuff
		//print("HEYEYEYEYEYE: "+ om.GetCount(Constants.TAG_MRNALP) + om.GetCount(Constants.TAG_PROTEINLP));
		if (om.GetCount(Constants.TAG_MRNALP) + om.GetCount(Constants.TAG_PROTEINLP) <= 0) {
			noLP = true;
		}
		//Is there an upgraded antigenome?
		List<GameObject> antiGenomes = GameObject.FindGameObjectsWithTag(Constants.TAG_ANTIGENOME).ToList();
		antiGenomes.AddRange(GameObject.FindGameObjectsWithTag(Constants.TAG_ARMORANTIGENOME).ToList());
		
		if (antiGenomes.Count < 1) { //you've lost -- no genomes
			noGenome = true;
		} 
		else {
			foreach(GameObject g in antiGenomes) {
				if (g.GetComponent<Producer>().upgradeLevel > 0) return;
			}
			noUpgrade = true;
		}
		
		//DID WE LOSE
		if (noGenome || (noLP && noUpgrade)) {
			isComplete = true;
			new MessageObjectiveComplete(gameObject);
		}
	}
	
	int GetUpgradedCount(List<GameObject> list)
	{
		int count = 0;
		foreach(GameObject g in list) {
			Producer p = g.GetComponent(typeof(Producer)) as Producer;
			if (p.upgradeLevel > 0) count++;
		}
		return count;
	}

	public void AddDelay (float deltaTime)
	{
		if (Time.time > nextTimeToInvoke) nextTimeToInvoke = Time.time + deltaTime;
		else nextTimeToInvoke += deltaTime;
	}
	#endregion

	/// <summary>
	/// Turns on necessary Listeners and assigns the proper check function to CheckObjective
	/// </summary>
	void SetType ()
	{
		if (checkEnergyofContainer) Dispatcher.Listen(Dispatcher.BUILD_COMMAND_START, gameObject);
		switch (objectiveType) {
		case CheckType.PopulationCount:
			CheckObjective = CheckPopulation;
			Dispatcher.Listen (Dispatcher.OBJECT_CREATED, gameObject);
			Dispatcher.Listen (Dispatcher.OBJECT_DESTROYED, gameObject);
			break;
		case CheckType.Collisions:
			CheckObjective = CheckCollisionCount;
			Dispatcher.Listen (Dispatcher.COLLISION_HAPPENS, gameObject);
			break;
		case CheckType.TimeCounter:
			Invoke ("TimeComplete", (float)quantity[0]);
			break;
		case CheckType.PopulationEquation:
			CheckObjective = CheckLossByPopulationEquation;
			Dispatcher.Listen(Dispatcher.OBJECT_CREATED, gameObject);
			Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
			Dispatcher.Listen(Dispatcher.BUILD_COMMAND_START, gameObject);
			break;
		case CheckType.Level10PopulationEq:
			CheckObjective = Check10Status;
			Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
			Dispatcher.Listen(Dispatcher.OBJECT_CREATED, gameObject);
			Dispatcher.Listen(Dispatcher.BUILD_COMMAND_START, gameObject);
			break;
		}
	}

	void TimeComplete ()
	{
		isComplete = true;
		CompletedObjective();
	}
	
	public void CompletedObjective() {
		foreach(GameObject i in activateOnCompletion) {
			i.active = true;
		}
		new MessageObjectiveComplete (gameObject);
	}
}
