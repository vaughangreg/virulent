using UnityEngine;
using System.Collections;

public class Caspace : MonoBehaviour
{
	public GameObject childModel;
	public string[] attackModeTags;
	public AIMovable aIMoveable;
	public Spin spinScriptA;
	public Spin spinScriptB;
	public bool isArmed;
	public GameObject mySpinChild;
	
	void Start() {
		if (isArmed) {
			//Extend claws
			childModel.GetComponent<Animation>().Play();
			isArmed = true;
			//Activate spin scriptA
			spinScriptA.enabled = true;
			//Change out the detection tags
			aIMoveable.targetTags = attackModeTags;
			mySpinChild.active = true;
		}
	}
	
	/// <summary>
	/// Message method called from EnergyContainer; extends blades and changes AI
	/// </summary>
	void OnAddEnergy()
	{
		if (!isArmed) {
			//Extend claws
			childModel.GetComponent<Animation>().Play();
			isArmed = true;
			//Activate spin scriptA
			spinScriptA.enabled = true;
			//Change out the detection tags
			aIMoveable.targetTags = attackModeTags;
			mySpinChild.active = true;
			
		}
	}
}

