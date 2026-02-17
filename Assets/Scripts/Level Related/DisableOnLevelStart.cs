using UnityEngine;
using System.Collections;

public class DisableOnLevelStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.active = false;
	}
}
