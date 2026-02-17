using UnityEngine;
using System.Collections;

/// <summary>
/// Encapsulates a command for the circular menu.
/// </summary>
[System.Serializable]
public class Command {
	public string					displayName;
	public ObjectManager.Unit		unitProduced;
	public EnergyCost[]				costs;
	public int						minUpgradeRequired = 0;
	
	public float 					timeRequired = 1.0f;
	
	protected bool					m_isProducing = false;
	public bool                     isProducing { get { return m_isProducing; } }
	protected float					m_time = 0.0f;
	
	/// <summary>
	/// A value in [0, 1] indicating how much longer remains.
	/// </summary>
	public float productionRatio {
		get {
			return m_time / timeRequired;
		}
	}
	
	/// <summary>
	/// Update production times and, if we're done, update state & return true.
	/// </summary>
	/// <param name="deltaTime">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool IsProductionReady(float deltaTime) {
		if (m_isProducing) {
			m_time += deltaTime;
			if (m_time >= timeRequired) {
				m_time = 0.0f;
				m_isProducing = false;
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Start making our unit.
	/// </summary>
	public void StartProduction() {
		if (m_isProducing) return;
		
		m_isProducing = true;
		m_time = 0.0f;
	}
	
	public int RequiredCostFor(Constants.Energy aType) {
		for (int i = 0; i < costs.Length; ++i) {
			if (costs[i].type == aType) return costs[i].amount;
		}
		return 0;
	}
}
