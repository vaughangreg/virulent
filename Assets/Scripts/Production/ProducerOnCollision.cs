using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Produces units after hitting an appropriate object.
/// </summary>
[RequireComponent(typeof(Collider))]
//[RequireComponent(typeof(EnergyContainer))]
[RequireComponent(typeof(Producer))]
public class ProducerOnCollision : MonoBehaviour {
	public    TerrainDeformer   deformer;
	public    float             baseFrequency = 5.0f;
	//protected EnergyContainer	m_battery;
	protected Producer			m_producer;
	protected List<Command> 	m_inProduction = new List<Command>();
	public    bool 				usesEnergy = false;
	public    EnergyContainer   container;
	
	/// <summary>
	/// Caches relevant components.
	/// </summary>
	void Start() {
		//m_battery = GetComponent<EnergyContainer>();
		m_producer = GetComponent<Producer>();
	}
	
	/// <summary>
	/// Produces objects after sufficient time.
	/// </summary>
	void Update() {
		List<Command> commandsToRemove = new List<Command>();
		Selectable selector = GetComponent<Selectable>();
		
		deformer.enabled = (m_inProduction.Count > 0) || (selector != null && selector.isSelected);
		deformer.frequency = m_producer.productionSpeedPercent * baseFrequency;
		foreach (Command aCommand in m_inProduction) {
			if (aCommand.IsProductionReady(Time.deltaTime * m_producer.productionSpeedPercent)) {
				if (!m_producer.Produce(aCommand.unitProduced)) {
					//m_battery.AddEnergy(aCommand.costs);
					// TODO: Change to messages in the future.
					Debug.Log("Couldn't produce " + aCommand.unitProduced, gameObject);
				}
				// TODO: Change to messages in the future.
				else Debug.Log("Produced " + aCommand.unitProduced, gameObject);
				commandsToRemove.Add(aCommand);
			}
		}
		
		foreach (Command aCommand in commandsToRemove) {
			m_inProduction.Remove(aCommand);	
		}
	}
	
	/// <summary>
	/// Produces units when the other has CommandOnCollision.
	/// </summary>
	/// <param name="other">
	/// A <see cref="Collision"/>
	/// </param>
	void OnCollisionEnter(Collision other) {
		if (!this.enabled) return;
		CommandOnCollision aCommandWrapper = other.gameObject.GetComponent<CommandOnCollision>();
		if (!aCommandWrapper) return;
		
		Command aCommand = aCommandWrapper.commandOnCollision;
		if (usesEnergy && container) {//Make sure we have enough energy
			if (!container.IsEnergySufficient(aCommand.costs)) return;
			else container.ConsumeEnergy(aCommand.costs);
		}
		
		float timeRequired = 0.0f;
		foreach (Command aTempCommand in m_inProduction) {
			timeRequired = Mathf.Max(aTempCommand.timeRequired, timeRequired);
		}
		
		aCommand.timeRequired = Mathf.Max(timeRequired + Random.Range(0.4f, 1.0f), aCommand.timeRequired);
		m_inProduction.Add(aCommand);
		aCommand.StartProduction();
		new MessageBuildCommandStarted(gameObject, aCommand.displayName, aCommand.timeRequired, aCommand.unitProduced);
	}
}
