using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeaconManager : MonoBehaviour {
	public GameObject beaconPrefab;
	public float myYPosition;
	
	/// <summary>
	/// Start listening.
	/// </summary>
	void Start () {
		Dispatcher.Listen(Dispatcher.BEACON_SIGNAL, gameObject);
		myYPosition = Camera.main.transform.position.y - Mathf.Abs(Camera.main.nearClipPlane) - 0.5f;
	}
	
	/// <summary>
	/// Received a beacon.
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageBeaconSignal"/>
	/// </param>
	void _OnBeaconSignal(MessageBeaconSignal e) {
		bool beaconedSomething = false;
				
		for (int objectIndex = 0; objectIndex < e.targetObj.Length; ++objectIndex) {
			GameObject anObject = e.targetObj[objectIndex];
			if (anObject.active) {
				Beacon(anObject, e.targetTime);
				beaconedSomething = true;
			}
		}
		
		for (int tagIndex = 0; tagIndex < e.targetTag.Length; ++tagIndex) {
			GameObject[] objects = GameObject.FindGameObjectsWithTag(e.targetTag[tagIndex]);
			beaconedSomething = (beaconedSomething || objects.Length > 0);
			for (int objectIndex = 0; objectIndex < objects.Length; ++objectIndex) {
				Beacon(objects[objectIndex], e.targetTime);
			}
		}

		if (beaconedSomething) MusicManager.PlaySfx(MusicManager.singleton.sFXBeacon);
	}
	
	/// <summary>
	/// Actually perform the beaconing.
	/// </summary>
	/// <param name="anObject">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="aDuration">
	/// A <see cref="System.Single"/>
	/// </param>
	void Beacon(GameObject anObject, float aDuration) {
		Transform trans = anObject.transform;
		
		Vector3 newPosVec = new Vector3(trans.position.x, myYPosition, trans.position.z);
		GameObject newBeacon = (GameObject)Instantiate(beaconPrefab, newPosVec, trans.rotation);
		newBeacon.GetComponent<FollowObject>().transformToFollow = trans;
		newBeacon.GetComponent<DestroyAfterTime>().timeToWaitBeforeDestroyInSeconds = aDuration;
	}
}
