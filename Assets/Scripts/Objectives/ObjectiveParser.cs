using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Watches for specified events and sends completion message when its conditions are met
/// </summary>
public class ObjectiveParser : Objective {
	public string test;
	
	private IObjective m_testRoot;
	private const string LIST_OPEN  = "(";
	private const string LIST_CLOSE = ")";
	
	private const string NODE_AND          = "and";
	private const string NODE_OR           = "or";
	private const string NODE_NOT          = "not";
	private const string NODE_EQUALS       = "=";
	private const string NODE_GREATER_THAN = ">";
	private const string NODE_LESS_THAN    = "<";
	private const string NODE_COUNT_POP    = "population";
	
	// when count == quantity objective is complete
	/// <summary>
	/// Calls SetType.
	/// </summary>
	void Awake() {
		Dispatcher.Listen(Dispatcher.OBJECT_CREATED,   gameObject);
		Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
		Dispatcher.Listen(Dispatcher.PAUSE_GAME, gameObject);
		
		ParseTest(test);
		enabled = false;
	}
	
	#region Parser
	void ParseTest(string aTest) {
		string[] tokens = aTest.Trim().Split();
		int      currentIndex = SkipEmptyNodes(tokens, 0);
		
		if (tokens[currentIndex].CompareTo(LIST_OPEN) == 0) {
			m_testRoot = ParseList(tokens, currentIndex + 1, out currentIndex);
		}
		else Debug.LogError("Malformed test; ensure that spaces delimit each token.", gameObject);
	}
	
	int SkipEmptyNodes(string[] tokens, int fromIndex) {
		int currentIndex = fromIndex;
		while (tokens[currentIndex] == string.Empty) ++currentIndex;
		return currentIndex;
	}
	
	IObjective ParseList(string[] tokens, int fromIndex, out int newIndex) {
		int currentIndex = SkipEmptyNodes(tokens, fromIndex);
		IObjective node  = NodeFactory(tokens, currentIndex, out currentIndex);
		
		int parsedInt = 0;
		while (currentIndex < tokens.Length) {
			switch (tokens[currentIndex]) {
				case LIST_OPEN:
					node.AddChild(ParseList(tokens, currentIndex + 1, out currentIndex));
					break;
				case LIST_CLOSE:
					newIndex = currentIndex + 1;
					return node;
				default:
					// We should ONLY have numbers here.
					try {
						currentIndex = SkipEmptyNodes(tokens, currentIndex);
						parsedInt    = System.Int32.Parse(tokens[currentIndex]);
						currentIndex++;
						node.AddChild(new ObjectiveLiteral(parsedInt));
					}
					catch {
						Debug.LogError("Test expected number instead of '" + tokens[currentIndex] + "' for token " + currentIndex, gameObject);
					}
					break;
			}
		}
		
		// If we reached this, then someone forgot a LIST_CLOSE
		Debug.LogError("Test missing " + LIST_CLOSE + " at " + currentIndex + " for list starting at " + (fromIndex - 1), gameObject);
		newIndex = tokens.Length;
		return null;
	}
	
	IObjective NodeFactory(string[] tokens, int fromIndex, out int newIndex) {
		string anIdentifier = tokens[fromIndex].ToLower();	// Normalize
		newIndex = fromIndex + 1;
		string tag;
		
		switch (anIdentifier) {
			case NODE_AND:			return new ObjectiveTest(ObjectiveTest.Type.And);
			case NODE_OR:			return new ObjectiveTest(ObjectiveTest.Type.Or);
			case NODE_NOT:			return new ObjectiveTest(ObjectiveTest.Type.Not);
			case NODE_EQUALS:		return new ObjectiveTest(ObjectiveTest.Type.Equals);
			case NODE_GREATER_THAN:	return new ObjectiveTest(ObjectiveTest.Type.GreaterThan);
			case NODE_LESS_THAN:	return new ObjectiveTest(ObjectiveTest.Type.LessThan);
			case NODE_COUNT_POP:
				newIndex++;
				tag = tokens[fromIndex + 1].Replace('_', ' ');
				return new ObjectivePopulationCount(tag);
			default:
				Debug.LogError("Invalid node type: " + anIdentifier, gameObject);
				return null;
		}
	}
	
	#endregion
	 
	#region Listener Methods
	/// <summary>
	/// On ObjectCreated update Count to a population and check for completion
	/// </summary>
	void _OnObjectCreated (MessageObjectCreated m) {
		m_testRoot.OnObjectCreated(m.createdObject);
		SignalIfComplete();
	}

	/// <summary>
	/// On ObjectDestroyed update Count to a population and check for completion
	/// </summary>
	void _OnObjectDestroyed (MessageObjectDestroyed m) {
		m_testRoot.OnObjectDestroyed(m.destroyedObject);
		SignalIfComplete();
	}
	
	void _OnPauseGame(MessagePauseGame m) {
		enabled = !m.paused;
	}	
	#endregion
	
	void SignalIfComplete() {
		if (enabled && m_testRoot.isComplete) {
			isComplete = true;
			CompletedObjective(); //new MessageObjectiveComplete(gameObject);
		}
	}

	#region Functions assigned to delegate CheckObjective
/*
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
		int count;
		count = GetUpgradedCount(antiGenome) + GetUpgradedCount(genome);
		//Get the count of energy in the 
		List<GameObject> buddingSite = GameObject.FindGameObjectsWithTag(Constants.TAG_BUDDINGSITE).ToList();
		int energy = 0;
		foreach(GameObject g in buddingSite) {
			energy += g.GetComponent<EnergyContainer>().energyAvailable[0];
		}
		//Use Nathan's population equation check
		ObjectManager om = ObjectManager.instance;
		//---- No Upgrade capable of happening
		bool check1 = (genome.Count + antiGenome.Count < 1 || om.GetCount(Constants.TAG_MRNALP) + om.GetCount(Constants.TAG_PROTEINLP) < 1);
		//---- No upgraded genomes
		bool check2 = (count < 1);
		//---- No Capsids/Virions
		bool check3 = (om.GetCount(Constants.TAG_CAPSID) < 1);
		//---- No production capable budding sites
		bool check4 = (energy < 1);
		//print(check1 +","+check2 +","+check3 +","+check4);
		if (check1 && check2 && check3 && check4) {
			isComplete = true;
			new MessageObjectiveComplete(gameObject); //<--  Objective was 'completed'
		}
	}
	*/
	#endregion
}
