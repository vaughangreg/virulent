using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OldInvagination : MonoBehaviour {
	/*
	public float lengthOfAnimation = 2f;
	public int spritesWidth = 4;
	public int spritesHeight = 4; 
	public GameObject virionMoveTo;
	public float virionMovementSpeed; 
	public GameObject endosomePrefab;
	private GameObject temporaryVirion;
	private List<GameObject> futureVirions;
	private bool doneThis = false;
	
	/// <summary>
	/// Initialize the futureVirions list
	/// </summary>
	void Start() {
		futureVirions = new List<GameObject>(); 
		
		//Dispatcher.Listen(Dispatcher.UNIT_SELECT, this);
		
	}
	
	void Update() {
		if (!doneThis && Time.time > 5f) {
			PassVirionToInvagination(virionMoveTo);
			doneThis = true;
		}
	}
	
	/// <summary>
	/// This is where you pass a Gameobject.... supposed to be a virion.... that is then pulled 
	/// into an endosome (adds a MoveObjectTo script and a SpriteAnimate script to do appropriate actions.  
	/// If a virion is already being pulled in then it will put the virion into a waiting list AND it will ignore all collisions between waiting virions
	/// 
	/// </summary>
	/// <param name="incomingVirion">
	/// A <see cref="GameObject"/>
	/// </param>
	public void PassVirionToInvagination(GameObject incomingVirion) { 
		if (temporaryVirion) {
			futureVirions.Add(incomingVirion);
			foreach(GameObject j in futureVirions) {
				if (j.collider && incomingVirion.collider) Physics.IgnoreCollision(j.collider, incomingVirion.collider, true);
			}
			if (temporaryVirion.collider && incomingVirion.collider) Physics.IgnoreCollision(incomingVirion.collider, temporaryVirion.collider, true);
			return;
		} else {
			temporaryVirion = incomingVirion;
		}
		foreach (Transform i in transform) {
			if (i.collider && incomingVirion.collider) Physics.IgnoreCollision(i.gameObject.collider, incomingVirion.collider, true);
		}
		MoveObjectTo movementScript = incomingVirion.AddComponent<MoveObjectTo>();
		movementScript.SetTarget(virionMoveTo.transform.position, virionMoveTo.transform.rotation, virionMovementSpeed);
		SpriteAnimate mySpriteAnimation = gameObject.AddComponent<SpriteAnimate>();
		mySpriteAnimation.PassSpriteInfo(renderer.material, spritesWidth, spritesHeight, lengthOfAnimation, false, this);
		this.enabled = false;
	} 
	
	/// <summary>
	/// When the script is re-enabled it 
	/// 1) creates an endosome around the virion that just entered
	/// 2) re-enables collisions so virion cant escape
	/// 3) passes next virion in the list to be pulled into the cell
	/// </summary>
	void OnEnable() {
		if (!temporaryVirion) return;
		Instantiate(endosomePrefab, temporaryVirion.transform.position, temporaryVirion.transform.rotation);
		// This is where you can pass the virion to the Endosome script
		foreach (Transform i in transform) {
			if (i.collider && temporaryVirion.collider) Physics.IgnoreCollision(i.gameObject.collider, temporaryVirion.collider, false);
		}
		temporaryVirion = null;
		if (futureVirions.Count > 0) {
			PassVirionToInvagination(futureVirions[0]);
			futureVirions.RemoveAt(0);
		}
	}
	
	void _OnUnitSelect(MessageUnitSelect e) {
		Debug.Log("hello");
		/*
		if (e.selectedUnit == gameObject) {
			PassVirionToInvagination(virionMoveTo);
		}
		*/
	/*}
	
	void OnTap() {
		PassVirionToInvagination(virionMoveTo);	
	}
   */
}
