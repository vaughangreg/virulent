using UnityEngine;
using System.Collections;

public class ChangeLayerAfterTime : MonoBehaviour
{
	
	public float timeToWait = 1.0f;
	private float m_timeWaited = 0;
	
	public LayerMask startLayer;
	public LayerMask endLayer;
	
	public bool overrideInitialLayer = false;
	
	void Start()
	{
		if (gameObject.layer != startLayer && overrideInitialLayer) {
			gameObject.layer = startLayer.value;
			print(startLayer.value);
		}
	}
	
	void Update()
	{
		m_timeWaited += Time.deltaTime;
		if (m_timeWaited >= timeToWait) {
			SwapLayer(gameObject, endLayer);
			Destroy(this);
		}
	}
	
	void SwapLayer(GameObject go, LayerMask newLayer)
	{
		go.layer = newLayer.value;
	}
}

