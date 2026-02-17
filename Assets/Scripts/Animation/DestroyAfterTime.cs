using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
	public float timeToWaitBeforeDestroyInSeconds = 2.0f;
	
	private float timeWaited;
	
	void Update()
	{
		timeWaited += Time.deltaTime;
		if (timeWaited >= timeToWaitBeforeDestroyInSeconds) Destroy(gameObject);
	}
	void OnDisable()
	{
		Destroy(gameObject);
	}
}

