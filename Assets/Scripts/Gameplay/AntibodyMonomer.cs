using UnityEngine;
using System.Collections;

public class AntibodyMonomer : MonoBehaviour {
	public float degreeWiggle = 15.0f;
	public float scaleWiggle = 0.1f;
	public float rotationTime = 1.0f; //Time in seconds
	public float scaleTime = 1.5f;
	public AudioClip deathClip;
	
	Vector3 rotationStart;
	Vector3 rotationGoal;
	float rotationTimeCounter;
	float rotationTimeModify = 0.0f;
	float rotationMultiplier = 1.0f;
	
	Vector3 scaleBase;
	Vector3 scaleStart;
	Vector3 scaleGoal;
	float scaleTimeCounter;
	float scaleTimeModify = 0.0f;
	
	bool inKillMode = false;
	float deathTime = 0;
	
	public float deathTimeWait = 3.0f; //How long it takes us to eat capsids
	
	/// <summary>
	/// Randomizes the wiggle values, and initializes the death location.
	/// </summary>
	void Start() {
		rotationStart = transform.localRotation.eulerAngles;
		//Randomize starting rotation goal
		if (Mathf.Round(Random.Range(0.0f,1.1f)) < 0.5) {
			rotationMultiplier = -1.0f;
		}
		rotationGoal = rotationStart + new Vector3(0,rotationMultiplier * degreeWiggle,0);
		//rotationOrigin = transform.localPosition + new Vector3(0,0,0.5f*transform.localScale.z);
		
		scaleBase = transform.localScale;
		scaleStart = scaleBase;
		scaleGoal = SetScaleGoal();
		
		UnitSpawnEvent spawnevent = new UnitSpawnEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitSpawnEvent> (spawnevent);
	}
	
	void Update() {
		WiggleRotation();
		if (inKillMode) KillModeUpdate();
		// WE ARE NO LONGER USING SCALING LIKE THIS
		/*
		else WiggleScale();
		*/
	}
	
	/// <summary>
	/// Makes the monomer wiggle it's rotation back and forth with a slerp
	/// </summary>
	void WiggleRotation() {
		//Make the Monomer Wiggle
		rotationTimeCounter += Time.deltaTime;
		if (rotationTimeCounter > rotationTime + rotationTimeModify) {
			//Reset Wiggle
			rotationMultiplier *= -1;
			rotationStart = rotationGoal;
			rotationGoal = rotationStart + new Vector3(0,2 * rotationMultiplier * degreeWiggle,0);
			rotationTimeCounter = 0;
			rotationTimeModify = Random.value * 2.0f;
		}
		else {
			//Slerp towards goal location...
			transform.localRotation = Quaternion.Euler(Vector3.Slerp(rotationStart,rotationGoal,rotationTimeCounter/(rotationTime+rotationTimeModify)));
		}
	}
	
	/// <summary>
	/// Makes the scale adjust itself for wiggle effects
	/// </summary>
	void WiggleScale() {
		scaleTimeCounter += Time.deltaTime;
		if (scaleTimeCounter > scaleTime + scaleTimeModify) {
			//Reset Scale Wiggle
			scaleStart = scaleGoal;
			scaleGoal = SetScaleGoal();
			scaleTimeCounter = 0;
			scaleTimeModify = Random.value * 2.0f;
		}
		else {
			transform.localScale = Vector3.Slerp(scaleStart,scaleGoal,scaleTimeCounter/(scaleTime+scaleTimeModify));
		}
	}
	
	/// <summary>
	/// Update that pulls captured capsid into B-Cell
	/// </summary>
	void KillModeUpdate() {
		// Move towards deathLocation over 3 seconds, wobbling a bit?
		deathTime += Time.deltaTime;
		if (deathTime > deathTimeWait) {
			GameObject.Destroy(gameObject);
		}
		else {
			float delta = Time.deltaTime / deathTimeWait;
			
			transform.Translate(new Vector3(0.0f, -10.0f * delta, 64.0f * delta),
		                         Space.Self);
		}
	}
	
	/// <summary>
	/// Randomizes a scale goal based on the scaleWiggle
	/// </summary>
	/// <returns>
	/// New scale goal
	/// </returns>
	Vector3 SetScaleGoal() {
		//Uniform scaling fix
		float scale = scaleBase.x + Random.Range(-scaleWiggle,scaleWiggle);
		return new Vector3(scale, 1.0f, scale);
	}
	
	/// <summary>
	/// Prepare us for kill mode.
	/// </summary>
	void DeathPrep() {
		// print("monomer death prepped");
		// Activate Capsid kill mode...
		inKillMode = true;
		// Set some cosmetic stuff
		degreeWiggle = 7;
		rotationTime = 0.3f;
		
		
		UnitDeathEvent deathevent = new UnitDeathEvent(gameObject.tag.ToString(),gameObject.GetInstanceID());
		ADAGE.UpdatePositionalContext (gameObject.transform.position, gameObject.transform.rotation.eulerAngles, 0);
		ADAGE.LogData<UnitDeathEvent> (deathevent);
		if (deathClip) MusicManager.PlaySfx(deathClip);
	}
	
	void TakingDamage() {
		
	}
}
