using UnityEngine;
using System.Collections;

public class LockYPosition : MonoBehaviour
{
	public bool lockToZero = true;
	
	private float m_y;
	
	void Awake()
	{
		if (!lockToZero) m_y = transform.position.y;
	}
	
	void Update()
	{
		Lock();
	}
	void LateUpdate()
	{
		Lock();
	}
	
	void Lock()
	{
		if (lockToZero) {
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 0, gameObject.transform.localPosition.z);
		}
		else {
			gameObject.transform.position = new Vector3(gameObject.transform.localPosition.x, m_y, gameObject.transform.localPosition.z);
		}
	}
}

