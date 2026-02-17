using UnityEngine;

/// <summary>
/// Gives energy upon collision.
/// </summary> 
public class EnergyProvider : MonoBehaviour {
	
	public float energyRadius = 30f;
	public float initialDelay = 1f;
	public EnergyCost[] providedEnergy;
	
	
	private float timeBetweenPulses;
	private float m_counter = 0f;
	
	void Start() {
		ParticleEmitter epe = transform.GetComponentInChildren<ParticleEmitter>();
		timeBetweenPulses = 1f / epe.minEmission ;
		m_counter = -initialDelay;
	}
	
	void Update() {
		m_counter += Time.deltaTime;
		if (m_counter > timeBetweenPulses) {
			m_counter -= timeBetweenPulses;
			m_counter = 0; 
			
			Ray m_ray = new Ray(transform.position - new Vector3(0, 1.5f * energyRadius, 0), Vector3.up);
			RaycastHit[] m_objs = Physics.SphereCastAll(m_ray, energyRadius);
			
			foreach(RaycastHit i in m_objs) HandleCollision(i.collider);
		}		
	}
	
	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position, energyRadius);
	}
	
	/// <summary>
	/// Adds energy to objects with an EnergyContainer.
	/// </summary>
	/// <param name="other">
	/// A <see cref="GameObject"/>
	/// </param>
	void HandleCollision(Collider other) {
		EnergyContainer battery = other.GetComponent<EnergyContainer>();
		if (!battery) return;
		
		battery.AddEnergy(providedEnergy);
	}
}
