using UnityEngine;

/// <summary>
/// Holds and uses energy.
/// </summary>
/// <see cref="Constants.Energy"/>
public class EnergyTransmitter : MonoBehaviour {
	public EnergyContainer recipientBattery;
	
	void OnCollisionEnter(Collision other) {
		//Return if there is no EnergyOnCollision on other
		EnergyOnCollision source = other.gameObject.GetComponent<EnergyOnCollision>();
		if (!source) return;
		
		//Send a message out in case the game object needs to do some actions
		//new MessageCollisionEvent(gameObject, other.gameObject);
		
		//Add energy to the recipient battery
		recipientBattery.AddEnergy(source.energyProvided);
	}
}
