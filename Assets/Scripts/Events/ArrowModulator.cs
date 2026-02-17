using UnityEngine;

public class ArrowModulator : MonoBehaviour {
	
	public float speed = 0.5f;		//time between each movement
	public float step = 5.0f;		//space between each step
	public GameObject arrow1;
	public GameObject arrow2;
	
	private int m_currentStep = 0;
	
	/// <summary>
	///Enable the children with the parent object.
	/// </summary>
	void OnEnable() {
		gameObject.SetActiveRecursively(true);	
	}
	
	/// <summary>
	///Disable the children with the parent object.
	/// </summary>
	void OnDisable() {
		gameObject.SetActiveRecursively(false);	
	}
	
	/// <summary>
	/// Set the initial positions of the arrows,
	/// enable or disable the children to match the parent,
	/// and InvokeRepeating InvokeNextFrame() at the proper interval.
	/// </summary>
	void Start() {
		arrow1.transform.position = transform.position;
		arrow2.transform.position = transform.position;
		arrow2.transform.Translate(Vector3.back * (step * 2));
		
		if (!this.enabled) gameObject.SetActiveRecursively(false);	
		else gameObject.SetActiveRecursively(true);
		
		InvokeRepeating("InvokeNextFrame", speed, speed);
	}
	
	/// <summary>
	/// Advance the step and translate the arrow to the next position.
	/// </summary>
	void InvokeNextFrame() {
		switch (m_currentStep) {
			case 0:
				m_currentStep = 1;
				arrow1.transform.Translate(Vector3.back * step);
				arrow2.transform.Translate(Vector3.forward * (2 * step));
				break;
			case 1:
				m_currentStep = 2;
				arrow1.transform.Translate(Vector3.back * step);
				arrow2.transform.Translate(Vector3.back * step);
				break;
			case 2:
				m_currentStep = 0;
				arrow1.transform.Translate(Vector3.forward * (2 * step));
				arrow2.transform.Translate(Vector3.back * step);
				break;
		}
	}
}
