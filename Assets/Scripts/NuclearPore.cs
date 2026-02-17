using UnityEngine;
using System.Collections;

public class NuclearPore : MonoBehaviour {
	public GameObject dummyMPrefab;
	public float timePerClog = 15f;
	public float otherFloat = 0f;
	private GameObject myProtein;
	private Vector3 collisionPos;
	private Quaternion collisionRot;
	private Vector3 collisionSize;
	//private Vector3 targetSize;
	private float counter = 0f;
	private bool rescanMe = false;
	
	/// <summary>
	/// Raises the trigger stay event.
	/// </summary>
	/// <param name='incomingCollision'>
	/// Incoming collision.
	/// </param>
	void OnCollisionStay(Collision incomingCollision) {
		OnCollisionEnter(incomingCollision);
	}
	
	/// <summary>
	/// Raises the trigger enter event.  If it doesnt already have a protein it instantiates a dummy and collects the info on the original
	/// and then destroys the original.
	/// </summary>
	/// <param name='incomingCollision'>
	/// Incoming collision.
	/// </param>
	void OnCollisionEnter(Collision incomingCollision) {
		if (myProtein) return; 
		if (incomingCollision.gameObject.tag == Constants.TAG_PROTEINM) { 
			myProtein = (GameObject)Instantiate(dummyMPrefab, incomingCollision.transform.position, incomingCollision.transform.rotation);
			//Physics.IgnoreCollision(myProtein.collider, gameObject.collider);
			counter = 0f; 
			myProtein.transform.parent = transform;
			collisionPos = myProtein.transform.position;
			collisionRot = myProtein.transform.rotation;
			collisionSize = myProtein.transform.localScale;
			//targetSize = new Vector3(collisionSize.x / transform.localScale.x, collisionSize.y / transform.localScale.y, collisionSize.z / transform.localScale.z);
			new MessageCollisionEvent(gameObject, incomingCollision.gameObject); 
			Destroy(incomingCollision.gameObject);
			ToggleTag();
			rescanMe = true;
		}		
	}
	
	/// <summary>
	/// If the pore has a protein it updates the position, rotation, and scale.  It pulls in proteins, orients them, and scales them appropriately
	/// After the timePerClog has passed, it destroys the dummy protein.
	/// </summary>
	void Update() {
		if (myProtein) {
			counter += Time.deltaTime;
			myProtein.transform.position = Vector3.Lerp(collisionPos, transform.position, counter);
			myProtein.transform.rotation = Quaternion.Slerp(collisionRot, transform.rotation, counter);
			myProtein.transform.localScale = Vector3.Lerp(collisionSize, Vector3.one, counter);
			if (counter > 1.0f && rescanMe) {
				AiHelper.RescanColliderArea(GetComponent<Collider>());
				rescanMe = false;
			}
			if (counter > timePerClog) {
				Destroy(myProtein); 
				ToggleTag();
				int oldLayer = gameObject.layer;
				gameObject.layer = 1 << LayerMask.NameToLayer("Walls");
				AiHelper.RescanColliderArea(GetComponent<Collider>());
				gameObject.layer = oldLayer;
			}
		}
	}
	
	void ToggleTag() {
		new MessageObjectDestroyed(gameObject, gameObject.tag, gameObject);
		if (gameObject.tag == Constants.TAG_NUCLEARPORE) gameObject.tag = Constants.TAG_CLOGGEDPORE;
		else gameObject.tag = Constants.TAG_NUCLEARPORE;
		new MessageObjectCreated(gameObject, gameObject, gameObject); 
	}
}
