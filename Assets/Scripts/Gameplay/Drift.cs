using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Moves the object towards a point while rotating.
/// </summary>
public class Drift : MonoBehaviour {
	public float lifeTime = 10.0f;
	public float maxLifeTime = 10.0f;
	public bool shouldSelfDestruct = true;
	public bool shouldFade = true;
	public bool shouldChangeY = true;
	public GameObject deathPrefab;
	public Renderer targetRenderer;
	public bool shouldShadeChildren = false;
	
	public float driftAmount = 4.0f;
	public bool shouldRotate = true;
	public Color sourceColor;
	public Color targetColor = Color.black;
	
	public Quaternion sourceRotation;
	public Quaternion targetRotation;
	
	private List<Renderer> childRenderers = new List<Renderer>();
	private List<Color> childColors = new List<Color>();
	
	/// <summary>
	/// Sets up initial conditions
	/// </summary>
	void Start() {
		if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
		foreach (Transform i in transform) {
			if (i.GetComponent<Renderer>()) {
				childRenderers.Add(i.GetComponent<Renderer>());
				childColors.Add(i.GetComponent<Renderer>().material.color);
			}
		}
		
		sourceColor = targetRenderer.material.color;
		sourceRotation = transform.rotation;
		targetRotation = Quaternion.Euler(0.0f, 360.0f * Random.value, 0.0f);
		if (shouldChangeY) transform.Translate(0.0f, -15.0f, 0.0f, Space.World);
	}
	
	/// <summary>
	/// Moves
	/// </summary>
	void Update() {
		lifeTime -= Time.deltaTime;
		
		transform.Translate((driftAmount * Random.value - driftAmount * 0.5f) * Time.deltaTime,
		                    0.0f,
		                    (driftAmount * Random.value - driftAmount * 0.5f) * Time.deltaTime);
		if (shouldRotate) transform.rotation = Quaternion.Slerp(targetRotation, sourceRotation, lifeTime / maxLifeTime);
		
		if (shouldFade) {
			targetRenderer.material.color = Color.Lerp(targetColor, sourceColor, lifeTime / maxLifeTime);
			if (shouldShadeChildren) {
				for (int i = 0; i < childRenderers.Count; i++) {
					//Debug.Log(childRenderers[i].gameObject.name);
					childRenderers[i].material.color = Color.Lerp(targetColor, childColors[i], lifeTime / maxLifeTime);
				}
			} 
			else {
				foreach (Transform aChild in transform) {
					if (aChild.GetComponent<Renderer>() && aChild.GetComponent<Renderer>() != targetRenderer) {
						aChild.GetComponent<Renderer>().material.color = targetRenderer.material.color;
					}
				}
			}
			
		}
		if (shouldSelfDestruct && lifeTime <= 0) Destroy(gameObject);
	}
}
