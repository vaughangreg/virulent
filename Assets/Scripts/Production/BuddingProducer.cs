using UnityEngine;

public class BuddingProducer : Producer {
	public BuddingSite buddingSite;
	
	/// <summary>
	/// Override Produce and run the budding site animation instead.
	/// </summary>
	/// <param name="aUnitType">
	/// A <see cref="ObjectManager.Unit"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public override bool Produce(ObjectManager.Unit aUnitType) {
		buddingSite.BeginBudding();
		return true;
	}
	
	/// <summary>
	/// Override these gizmos because they are of no use on budding site.
	/// </summary>
	public override void OnDrawGizmosSelected() {
		
	}
}
