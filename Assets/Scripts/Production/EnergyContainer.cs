using UnityEngine;
using System.Collections;

/// <summary>
/// Holds and uses energy.
/// </summary>
/// <see cref="Constants.Energy"/>
public class EnergyContainer : MonoBehaviour {
	public int[] energyAvailable = new int[(int)Constants.Energy.Count];
	public int[] energyMaximum = new int[(int)Constants.Energy.Count];
	
	/// <summary>
	/// Checks if sufficient energy exists to pay the costs.
	/// </summary>
	/// <param name="costs">
	/// A <see cref="EnergyCost[]"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool IsEnergySufficient(EnergyCost[] costs) {
		bool canConsume = true;
		
		foreach (EnergyCost aCost in costs) {
			if (energyAvailable[(int)aCost.type] < aCost.amount) {
				canConsume = false;
				break;
			}
		}
		
		return canConsume;
	}
	
	/// <summary>
	/// Spends the energy costs.
	/// </summary>
	/// <param name="costs">
	/// A <see cref="EnergyCost[]"/>
	/// </param>
	public void ConsumeEnergy(EnergyCost[] costs) {
		int total_cost = 0;
		int total = 0;
		foreach (EnergyCost aCost in costs) {
			energyAvailable[(int)aCost.type] -= aCost.amount;	
			total_cost -= aCost.amount;	
			total = energyAvailable[(int)aCost.type];
		}
		EnergyEvent loss = new EnergyEvent (gameObject.tag.ToString(),gameObject.GetInstanceID(),total_cost,total);
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<EnergyEvent> (loss);
		UpdateVirionEnergy();
	}
	
	/// <summary>
	/// Adds energy from a command.
	/// </summary>
	/// <param name="costs">
	/// A <see cref="EnergyCost[]"/>
	/// </param>
	public void AddEnergy(EnergyCost[] costs) {
		foreach (EnergyCost aCost in costs) {
			AddEnergy(aCost.type, aCost.amount);	
		}

		transform.SendMessageUpwards("OnAddEnergy", SendMessageOptions.DontRequireReceiver);
		new MessageEnergyReceived(gameObject, this);


	}
	
	/// <summary>
	/// Adds energy up to the maximum. 
	/// </summary>
	/// <param name="aType">
	/// A <see cref="Constants.Energy"/>
	/// </param>
	/// <param name="anAmount">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void AddEnergy(Constants.Energy aType, int anAmount) {
		energyAvailable[(int)aType] = Mathf.Min(energyAvailable[(int)aType] + anAmount,
		                                        energyMaximum[(int)aType]);

		EnergyEvent gain = new EnergyEvent (gameObject.tag.ToString(),gameObject.GetInstanceID(),anAmount,energyAvailable[(int)aType]);
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<EnergyEvent> (gain);

		UpdateVirionEnergy();
	}
	
	private void UpdateVirionEnergy() {
		if (energyAvailable.Length > (int)Constants.Energy.Capsid) {
			int sumVesicle = energyAvailable[(int)Constants.Energy.VesicleG];
			int sumProtein = energyAvailable[(int)Constants.Energy.ProteinM] / 4;
			int sumGenome  = energyAvailable[(int)Constants.Energy.Genome];
			int sum = Mathf.Min(sumVesicle, sumProtein, sumGenome);
			
			// Debug.Log("Virion sum: " + sum + "/" + energyMaximum[(int)Constants.Energy.Capsid]);
			energyAvailable[(int)Constants.Energy.Capsid] = Mathf.Min(sum, energyMaximum[(int)Constants.Energy.Capsid]);
		}	
	}
	
	/// <summary>
	/// Returns a value in [0, 1] representing the energy ratio for the given type.
	/// </summary>
	/// <param name="aType">
	/// A <see cref="Constants.Energy"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public float RatioFor(Constants.Energy aType) {
		float available = energyAvailable[(int)aType];
		float max = energyMaximum[(int)aType];
		
		return (max == 0.0f) ? (0.0f) : (available / max);
	}
}
