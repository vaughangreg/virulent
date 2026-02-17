using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// It will create the specified units when it deems fit
/// </summary>
public class AIProducer : Producer {
	[System.Serializable]
	public class UnitShell {
		public ObjectManager.Unit unitType;
		public PopulationGoal populationRestrictor;
		public PopulationGoal[] populationGoals = new PopulationGoal[1];
		[System.NonSerialized] public bool show = false;
	}
	
	public float delay = 1.0f;
	public float productionRate = 4.0f;
	public float productionVarience = 1.0f;
	public ObjectManager.Unit onUpgradeUnitToProduce;
	[HideInInspector]public UnitShell[] unitsToProduce = new UnitShell[1];
	
	private float m_timeWaited;
	private float m_timeToWait;
	private bool m_makeUpgradedUnit = false;
	
	void Start() {
		Dispatcher.Listen(Dispatcher.OBJECT_UPGRADING, gameObject);
		
		// While the type is being serialized, the operator delegate
		// isn't being set correctly. This should remedy the problem.
		foreach (UnitShell aShell in unitsToProduce) {
			foreach (PopulationGoal aGoal in aShell.populationGoals) {
				aGoal.Refresh();	
			}
			aShell.populationRestrictor.Refresh();
		}
		
		m_timeToWait = delay;	
	}
	
	void Update() {
		m_timeWaited += Time.deltaTime;
		if (m_timeWaited >= m_timeToWait) { //Add a unit to produce
			PickUnitAndProduce();
			m_timeWaited = 0;
			m_timeToWait = productionRate + (Random.value * productionVarience);
		}
	}
	
	bool PickUnitAndProduce() {
		//Debug.Log("------------ Picking a unit to produce --------------");
		if (m_makeUpgradedUnit) {
			m_makeUpgradedUnit = !Produce(onUpgradeUnitToProduce);
			return !m_makeUpgradedUnit;
		}
		//Make a temp list of usable units
		List<ObjectManager.Unit> tempList = new List<ObjectManager.Unit>();
		
		//Search the populations and see if the units are available to produce
		foreach(UnitShell u in unitsToProduce) {
			bool usable = false;
			//Trigger this if any of the values aere true
			foreach (PopulationGoal p in u.populationGoals) {
				if (p.MetGoal()) {
					usable = true;
					break;
				}
			}
			if (u.populationRestrictor.MetGoal()) usable = false;
			if (usable) tempList.Add(u.unitType);
		}
		
		//Pick a random unit to create, if there are any
		if (tempList.Count > 0) {
			bool result = Produce(tempList[Random.Range(0, tempList.Count-1)]);
			return result;
		}
		else { 
			return false;
		}
	}
	
	void _OnUpgradingObject (MessageUpgradingObject m) {
		if (m.upgradedObject == gameObject) {
			//Debug.Log("Hi");
			Debug.Log("AI upgrade");
			m_makeUpgradedUnit = true;
		}
	}
	
}

