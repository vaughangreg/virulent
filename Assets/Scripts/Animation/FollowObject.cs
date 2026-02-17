using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

	public Transform transformToFollow;
	private float myYPosition;
	
	void Start() {
		myYPosition = Camera.main.transform.position.y - Mathf.Abs(Camera.main.nearClipPlane) - 0.5f;	
	}
	
	// Update is called once per frame
	void Update () {
		if (!transform || !transformToFollow) {
			Destroy(gameObject);
			return;
		}
		transform.position = new Vector3(transformToFollow.position.x, myYPosition, transformToFollow.position.z);	
	}
}
