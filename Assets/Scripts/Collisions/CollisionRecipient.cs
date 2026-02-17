using UnityEngine;

// Disable this if we don't need it.
[RequireComponent(typeof(Combat))]
[RequireComponent(typeof(EnergyContainer))]
public class CollisionRecipient : MonoBehaviour {	
	void Start() {
		GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//primitive.transform.rotation = transform.rotation;
		primitive.transform.position = transform.position;
		primitive.transform.localScale = Vector3.one * 100.0f;
		primitive.layer = LayerMask.NameToLayer("Ignore Raycast");
		CollisionForwarder forwarder = primitive.AddComponent<CollisionForwarder>();
		Combat thisCombat = GetComponent<Combat>();
		forwarder.targetCombat = thisCombat;
		
		Combat primitiveCombat = primitive.AddComponent<Combat>();
		primitiveCombat.thisObjectFlag = thisCombat.thisObjectFlag;
		
		EnergyTransmitter transmitter = primitive.AddComponent<EnergyTransmitter>();
		transmitter.recipientBattery = GetComponent<EnergyContainer>();
		
		Destroy(primitive.GetComponent<Renderer>());
		
		primitive.name = "Budding Collision Site (generated)";
	}
}
