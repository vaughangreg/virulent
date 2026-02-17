using UnityEngine;

/// <summary>
/// Changes production speed upon collision if possible.
/// </summary>
[RequireComponent(typeof(Collider))]
public class OnCollisionProductionSpeedChange : MonoBehaviour {
	public Constants.CombatFlags changeableObjects;
	public float changeAmount = -0.1f;
	public static float minimumSpeed = 0.5f;
	
	public Material materialToSwap;
	
	/// <summary>
	/// Changes production speed in other.gameObject upon collision if possible.
	/// </summary>
	/// <param name="other">
	/// A <see cref="Collision"/>
	/// </param>
	void OnCollisionEnter(Collision other) {
		Combat c = other.gameObject.GetComponent<Combat>();
		if (c == null) return;
		
		if (((int)c.thisObjectFlag & (int)changeableObjects) > 0) {
			Debug.Log(c.gameObject);
			Producer theProducer = c.GetComponent<Producer>();
			if (!theProducer) return;
			
			new MessageUpgradingObject(other.gameObject);
			
			theProducer.productionSpeedPercent += changeAmount;
			if (theProducer.productionSpeedPercent < minimumSpeed) theProducer.productionSpeedPercent = minimumSpeed;
			if (materialToSwap) other.gameObject.GetComponent<Renderer>().material = materialToSwap;
		}
	}
}
