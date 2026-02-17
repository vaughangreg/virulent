using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Produces units after hitting an appropriate object.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Producer))]
public class ProducerOnTime : MonoBehaviour {
	public    Command[]         productions;
	public    TerrainDeformer   deformer;
	public    float             baseFrequency = 5.0f;
	public    float             productionPercentChange = 0.95f;
	protected Producer			m_producer;
	
	/// <summary>
	/// Caches relevant components.
	/// </summary>
	void Start() {
		m_producer = GetComponent<Producer>();
		deformer   = GetComponent<TerrainDeformer>();
		deformer.enabled = productions.Length > 0;
		
		foreach (Command aCommand in productions) {
			aCommand.StartProduction();	
		}
	}
	
	/// <summary>
	/// Produces objects after sufficient time.
	/// </summary>
	void Update() {
		deformer.frequency = m_producer.productionSpeedPercent * baseFrequency;
		
		foreach (Command aCommand in productions) {
			if (aCommand.IsProductionReady(Time.deltaTime * m_producer.productionSpeedPercent)) {
				if (!m_producer.Produce(aCommand.unitProduced)) {
					Debug.Log("Couldn't produce " + aCommand.unitProduced, gameObject);
				}
				else {
					Debug.Log("Produced " + aCommand.unitProduced, gameObject);
				}
				aCommand.StartProduction();
				m_producer.productionSpeedPercent *= productionPercentChange;
			}
		}
	}
}
