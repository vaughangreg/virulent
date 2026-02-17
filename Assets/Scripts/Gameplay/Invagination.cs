using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Invagination : MonoBehaviour 
{
	
	public Mesh defaultMesh;
	public PathNodeV2 childPathNode;
	public  List<GameObject> invaginationModels = new List<GameObject>();
	public float animationSpeedInSeconds = 0.02f; //Time in seconds
	public AudioClip invaginationSFX;
	
	private List<GameObject> virionList = new List<GameObject>();
	
	private int    modelNumber =  0;
	private int    modelCurrentNum = 0;
	private float  modelCounter = 0;
	private string modelString =  "Cell_Wall_";
	
	private bool isAnimating = false;
	
	private GameObject currentCapsid;
	
	// THESE ARE HACKS.
	private Quaternion 	originRotation;
	private Quaternion 	finalRotation;
	private Vector3 	originPosition;
	private Vector3 	finalPosition;
	
	
	void Start() {
		//centerNode.transform.LookAt(childPathNode.nodePosition);
		finalRotation = Quaternion.Euler(new Vector3(0.0f, centerNode.transform.eulerAngles.y, 0.0f));
	}
	
	void Update() {
		if (isAnimating) AnimateInvagination();
	}
	
	void OnTriggerEnter(Collider other) {
		//print("Something Hit the Invagination Trigger");
		GameObject go = other.gameObject;
		if (go.CompareTag("Capsid") && go.transform.parent != null)
		{
			//print("Invagination Collided with Capsid");
			//Check and see if it's already in the list, add it if it's not
			if (!virionList.Contains(go))
				AddVirionToList(go);
		}
	}
	
	void AddVirionToList(GameObject virion) {
		virionList.Add(virion);
		if (virionList.Count == 1)
			Invaginate(virion);
	}
	
	public GameObject centerNode;
	void Invaginate(GameObject virion) {
		//Begin animating
		isAnimating = true;
		
		//Destroy the parent monomer
		GameObject go = virion.transform.parent.gameObject;
		virion.transform.parent = null;		
		Destroy(go);
		
		currentCapsid = virion;
		
		DummyifyCapsid(virion);
		
		//set positions and rotations for the lerp
		originRotation = 	virion.transform.rotation;
		originPosition = 	virion.transform.position;
		finalPosition = 	childPathNode.myPath[0].position;
	}
	
	/// <summary>
	/// Turns a capsid into a capsid dummy by stripping off all non-essential components.
	/// </summary>
	/// <param name="virion">
	/// A <see cref="GameObject"/>
	/// </param>
	void DummyifyCapsid(GameObject virion) {
		Destroy(virion.GetComponent<Rigidbody>());
		Destroy(virion.GetComponent("Combat"));
		Destroy(virion.GetComponent<Collider>());
		Destroy(virion.GetComponent("MoveObjectTo"));
		Destroy(virion.GetComponent("MovementLerped"));
	}
	
	void AnimateInvagination() {
		modelCounter += Time.deltaTime/animationSpeedInSeconds;
		modelNumber = (int)modelCounter;
		if (modelNumber != modelCurrentNum) {
			
			currentCapsid.transform.rotation = Quaternion.Lerp(originRotation, 
			                                                   finalRotation, 
			                                                   (float) modelNumber / (float)invaginationModels.Count);
			currentCapsid.transform.position = Vector3.Lerp(originPosition,
			                                                finalPosition,
			                                                (float) modelNumber / (float)invaginationModels.Count);
			
			//Swap out the actual model
			foreach (GameObject go in invaginationModels) {
				if (go.name == modelString + modelNumber) {
					gameObject.GetComponent<MeshFilter>().mesh = go.GetComponent<MeshFilter>().sharedMesh;
					break;
				}
			}
			modelCurrentNum = modelNumber;
			if (modelCurrentNum > invaginationModels.Count) { //Reset and prepare for another animation if needed
				new MessageInvaginationComplete(gameObject);
			}
		}   
	}

	void _OnInvaginationComplete(MessageInvaginationComplete e) {
		ResetMesh();
		
		//Play the invagination sfx
		MusicManager.PlaySfx(invaginationSFX);
		
		//kill the capsid and remove it from the list
		currentCapsid.SendMessage("DeathPrep");
		virionList.Remove(currentCapsid);
		
		//Allow the next capsid to invaginate...
		if (virionList.Count > 0)
		{
			Invaginate(virionList[0]);
		}
	}
	
	/// <summary>
	///Reset the mesh and stop animation.
	/// </summary>
	void ResetMesh() {
		gameObject.GetComponent<MeshFilter>().mesh = defaultMesh;
		modelNumber =  0;
		modelCurrentNum = 0;
		modelCounter = 0;
		isAnimating = false;
	}
}
