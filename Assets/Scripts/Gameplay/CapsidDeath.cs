using UnityEngine;
using System.Collections;

public class CapsidDeath : MonoBehaviour {

	public GameObject[] ActivateOnDamage;
	
	/// <summary>
	/// Animates the last bit of damage for a virion.  It is performed on a dummy object instead of the initial capsid.
	/// </summary>
	void AnimateDamage() {
		foreach (GameObject i in ActivateOnDamage) {
			GameObject tempObj = Instantiate(i,transform.position + new Vector3(0f, 1f, 0f), transform.rotation) as GameObject;
			tempObj.transform.parent = transform;
		} 
	}
	
}
