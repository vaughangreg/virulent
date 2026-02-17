using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This script allows you to create check points.  When placed on an empty gameObject, 
/// </summary>
public class CheckPointEvent : MonoBehaviour
{
	public enum eventTypes
	{
		unitSelect,
		componentAdded,
		mouseClick,
		collisionEvent,
		countObjectCreated,
		upgradingObject,
		gameTimer,
		objectDestroyed,
		population,
		energyReceived,
		buddingSiteReady,
		cameraMovementComplete,
	}
	public eventTypes messageToCauseTrigger;
	//Needs to match with names defined in Dispatcher.cs
	public string sourceTag;
	public Component watchedComponent;
	public GameObject sourceGameObject;
	public GameObject[] gameObjToActivate;
	public bool activateChildren = false;
	public MonoBehaviour[] behaviorsToActivate;
	public GameObject[] gameObjsToKill;
	public bool deactivateInsteadOfKill = false;
	public AIMovable[] AIObjectsToActivate;
	public GameObject[] gameObjectsToBeaconize;
	public string[] tagsToBeaconize;
	public float beaconLength = 5f;
	public Vector3 MoveCameraPosition;
	public float MoveCameraSize;
	public int noOfOccurrences = 1;
	public string stringToSendOnCompletion;
	public AudioClip audioClipToSendOnCompletion;
	public float timerLength = 5f;
	
	public bool rescanGrid = false;
	public float rescanRadius = 10f;
	
	public bool cameraDynamicPosition = false;
	public bool cameraOperateFocus = false;
	public MessageCameraFocus focusMessage;
	
	public bool decreaseOccurrenceCountWithPopulation = false;
	public bool increaseOccurrenceCountWithPopulation = false;
	public List<string> populationToWatch;
	
	public bool popGridFromAI = false;
	public int gridToKill = 0;
	
	public bool moveGridOffset = false;
	public Vector3 nextOffset;
	public int[] newGridDimensions = new int[4]{-1, -1, -1,-1};  // must be length == 4
	
	private bool alreadyCompleted = false;
	
	
	[HideInInspector]public List<PopulationGoal> populationGoals = new List<PopulationGoal>();

	/// <summary>
	/// This is where the checkpoints start listening for events.  It listens to whatever the enum is set to...
	/// </summary>
	void Start() {
		ListenForEvents(); 
		if (messageToCauseTrigger == eventTypes.gameTimer) {
			Timer myTimer = gameObject.AddComponent<Timer>();
			myTimer.SetTimer(timerLength, gameObject);
		}
	}
	
	void CameraPositionCheck(GameObject anObject) {
		if (cameraDynamicPosition && (anObject.CompareTag(sourceTag) || anObject == sourceGameObject)) {
			// Since the object could be destroyed, etc.,
			// we'll just update the position.
			focusMessage.targetPosition = anObject.transform.position;
		}
	}

	/// <summary>
	///  Activates game objects when an appropriate unit is selected
	/// </param>
	void _OnUnitSelect(MessageUnitSelect e) { 
		if ((sourceTag == e.source.tag || sourceGameObject == e.source)) {
			CameraPositionCheck(e.source);
			CompleteTasks();
		}
	}

	/// <summary>
	///  Activates game objects when an appropriate unit is pathed
	/// </param>
	void _OnUnitPath(MessageUnitPath e) { 
		if ((sourceTag == e.source.tag || sourceGameObject == e.source)) {
			CameraPositionCheck(e.source);
			CompleteTasks();
		}
	}

	/// <summary>
	///  Doesnt do anything yet
	/// </param>
	void _OnMouseClick (MessageClickMouse e) {
		//test for mouse click position?  not sure how you could use this as a check point since buttons and unit select are separate messages....
	}

	/// <summary>
	///  Activates game objects when an appropriate component is added to a gameObject 
	/// </param>
	void _OnComponentAdded (MessageComponentAdded e) { 
		if ((sourceTag == e.toThisGameObject.tag || sourceGameObject == e.toThisGameObject)
		    && (e.thisComponentWasAdded.GetType () == watchedComponent.GetType ())) 
		{
			CameraPositionCheck(e.toThisGameObject);
			CompleteTasks();
		}
	}

	/// <summary>
	///  Activates game objects when a specific collision/trigger is set off
	/// </param>
	void _OnCollisionEvent (MessageCollisionEvent e) {
		if ((sourceTag == e.collisionObj1.tag || sourceTag == e.collisionObj2.tag 
		     || sourceGameObject == e.collisionObj1 || sourceGameObject == e.collisionObj2)) 
		{
			if (cameraDynamicPosition) {
				CameraPositionCheck(e.collisionObj1);
				CameraPositionCheck(e.collisionObj2);
			}
			CompleteTasks();
		}
	}
	
	/// <summary>
	/// Activates stuff when an object with a tag or a specific object is destroyed.
	/// </summary> 
	void _OnObjectDestroyed (MessageObjectDestroyed e) {
		if (decreaseOccurrenceCountWithPopulation && populationToWatch.Contains(e.destroyedObject.tag)) {
			CompleteTasks();
			return;
		}
		if (e.destroyedObject.tag == sourceTag || e.destroyedObject == sourceGameObject) {
			CameraPositionCheck(e.source);
			CompleteTasks();
		}
	}
	
	
	/// <summary>
	/// Handles object creation stuff, like population and personal counts
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageObjectCreated"/>
	/// </param>
	void _OnObjectCreated(MessageObjectCreated e) {
		if (increaseOccurrenceCountWithPopulation && populationToWatch.Contains(e.createdObject.tag)) {
			noOfOccurrences++;
			return;
		}
		if (e.createdObject.tag == sourceTag) {
			if (noOfOccurrences == 0) {
				ActivateGameObjects(gameObjToActivate);
				ActivateBehaviors(behaviorsToActivate);
				ActivateAIObjects(AIObjectsToActivate);
				CreateBeacons(gameObjectsToBeaconize, tagsToBeaconize, beaconLength);
				new MessageCheckPoint (gameObject, stringToSendOnCompletion, audioClipToSendOnCompletion);
			}
				noOfOccurrences--;
		}
		else if (sourceTag == "mRNA") {
			if(e.createdObject.tag == "N mRNA" || e.createdObject.tag == "M mRNA" || e.createdObject.tag == "G mRNA" || e.createdObject.tag == "LP mRNA") { 
			//Debug.Log(e.createdObject.tag + "created");
				if (noOfOccurrences == 0) {
					//Debug.Log("EVENT FIRED");
					ActivateGameObjects (gameObjToActivate);
					ActivateBehaviors (behaviorsToActivate);
					ActivateAIObjects (AIObjectsToActivate);
					CreateBeacons(gameObjectsToBeaconize, tagsToBeaconize, beaconLength);
					new MessageCheckPoint (gameObject, stringToSendOnCompletion, audioClipToSendOnCompletion);
				}
				noOfOccurrences--;
			}
		}
	}
	/// <summary>
	/// Waits for goal populations to be met
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageObjectCreated"/>
	/// </param>
	void _OnPopulationChanged(MessagePopulationChanged m) {
		foreach(PopulationGoal p in populationGoals) {
			if (!p.MetGoal()) return;
		}
		//print("Population Checkpoint Met");
		CompleteTasks();
	}
	
	void _OnUpgradingObject(MessageUpgradingObject e) {
		if (e.upgradedObject.tag == sourceTag || e.upgradedObject == sourceGameObject) {
			CameraPositionCheck(e.upgradedObject);
			CompleteTasks();
		}
	}
	
	void _OnCameraMovementComplete(MessageCameraMovementComplete m) { 
		//Debug.Log("hello");
		CompleteTasks(); 
	}
	
	/// <summary>
	/// This is what happens when it receives a Timer Complete command with this GameObject as a value
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageTimerComplete"/>
	/// </param>
	void _OnTimerComplete (MessageTimerComplete e) {
		//Debug.Log(e.timerGameObject.name);
		if (e.timerGameObject == gameObject) {
			CameraPositionCheck(e.timerGameObject);
			CompleteTasks();
		}
		
	}
	
	/// <summary>
	/// Checks if source tags or objects have recieved energy and completes if so 
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageEnergyReceived"/>
	/// </param>
	void _OnEnergyReceived(MessageEnergyReceived m)
	{
		//Break if required energy is not met
		if (m.energyContainer.energyAvailable[0] != noOfOccurrences) return;
		
		print("next step");
		if (sourceTag.Length > 0) { //Check tag
			if (m.source.gameObject.CompareTag(sourceTag)) {
				CompleteTasks();
				print("tag success");
			}	
		}
		if (sourceGameObject) { //Source object
			if (m.source == sourceGameObject) {
				CompleteTasks();
				print("source success");
			}
		}
	}
	
	void _OnBuddingSiteReady(MessageBuddingSiteReady m)
	{
		CompleteTasks();
	}
	
	
	/// <summary>
	///  Activates game objects  
	/// </param>
	void ActivateGameObjects (GameObject[] incomingGameObjs)
	{
		foreach (GameObject i in incomingGameObjs) {
			if (i != null) {
				i.active = true;
				//Activate the object's children if the option was set
				if (i.transform.childCount > 0 && activateChildren) {
					i.SetActiveRecursively(true);
				}
			}
		}
	}
	/// <summary>
	///  Activates game objects  
	/// </param>
	void ActivateAIObjects (AIMovable[] incomingAIObjects)
	{
		foreach (AIMovable i in incomingAIObjects) {
			if (i != null) i.enabled = true;
			
		}
	}
	/// <summary>
	/// Activates the monobehaviors.
	/// </summary>
	/// <param name='incomingBehaviors'>
	/// Incoming behaviors.
	/// </param>
	void ActivateBehaviors (MonoBehaviour[] incomingBehaviors)
	{
		foreach (MonoBehaviour i in incomingBehaviors) {
			if (i != null) i.enabled = true;
		}
	}
	
	/// <summary>
	/// This destroys any objects that you have listed to be destroyed on this event.
	/// </summary>
	/// <param name="objectsToKill">
	/// A <see cref="GameObject[]"/>
	/// </param>
	void KillObjects (GameObject[] objectsToKill) 
	{
		foreach(GameObject i in objectsToKill) 
		{
			if (i != null)  {
				if (deactivateInsteadOfKill) i.SetActiveRecursively(false);
				else Destroy(i);
			}
		}
	}
	
	void CreateBeacons (GameObject[] objsToBeacon, string[] tagsToBeacon, float timeTilDeath)
	{ 
		if (objsToBeacon.Length > 0 && objsToBeacon[0] != null) new MessageBeaconSignal(objsToBeacon, new string[0]{}, timeTilDeath);
		if (tagsToBeacon.Length > 0 && tagsToBeacon[0] != null) new MessageBeaconSignal(new GameObject[0]{}, tagsToBeacon, timeTilDeath);
	}
	
	/// <summary>
	/// Changes the camera position and orthographic size.
	/// </summary>
	/// <param name="New camera position (can't be Vector3.zero).">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="Orthographic size of the camera.">
	/// A <see cref="System.Single"/>
	/// </param>
	void ChangeCameraView(Vector3 cameraPosition, float cameraSize) {
		if (cameraPosition != Vector3.zero) {
			Camera.main.transform.position = cameraPosition;
			Camera.main.orthographicSize = cameraSize;
		}
	}
	
	/// <summary>
	/// This is where all of the various tasks for each event are taken care of.
	/// </summary>
	void CompleteTasks () {
		if (alreadyCompleted) return;
		//Debug.Log("completing tasks");
		if (messageToCauseTrigger != CheckPointEvent.eventTypes.energyReceived) noOfOccurrences--;
		//Debug.Log(noOfOccurrences);
		if (noOfOccurrences <= 0 || messageToCauseTrigger == CheckPointEvent.eventTypes.energyReceived) {
			ActivateGameObjects (gameObjToActivate);
			ActivateBehaviors (behaviorsToActivate);
			ActivateAIObjects (AIObjectsToActivate);
			KillObjects(gameObjsToKill);
			ChangeCameraView(MoveCameraPosition, MoveCameraSize);
			CreateBeacons(gameObjectsToBeaconize, tagsToBeaconize, beaconLength); 
			new MessageCheckPoint (gameObject, stringToSendOnCompletion, audioClipToSendOnCompletion);
			if (popGridFromAI) AiHelper.PopGrid(gridToKill);
			if (moveGridOffset) {
				if (newGridDimensions[0] != -1) AiHelper.MoveGrid(nextOffset, newGridDimensions);
				else AiHelper.MoveGrid(nextOffset, new int[0]);
			}
			if (cameraOperateFocus) focusMessage.SendManually(gameObject);
			if (rescanGrid) AiHelper.RescanRectArea(transform.position, rescanRadius);
			alreadyCompleted = true;
		} 
	}
	

	/// <summary>
	///  Listens for events based on what enum is set to
	/// </param>
	void ListenForEvents ()
	{
		if (messageToCauseTrigger == eventTypes.unitSelect) {
			Dispatcher.Listen (Dispatcher.UNIT_SELECT, gameObject);
		} else if (messageToCauseTrigger == eventTypes.mouseClick) {
			Dispatcher.Listen (Dispatcher.CLICK_MOUSE, gameObject);
		} else if (messageToCauseTrigger == eventTypes.componentAdded) {
			Dispatcher.Listen (Dispatcher.COMPONENT_ADDED, gameObject);
		} else if (messageToCauseTrigger == eventTypes.collisionEvent) {
			Dispatcher.Listen (Dispatcher.COLLISION_HAPPENS, gameObject);
		} else if (messageToCauseTrigger == eventTypes.countObjectCreated) {
			Dispatcher.Listen (Dispatcher.OBJECT_CREATED, gameObject);
		} else if (messageToCauseTrigger == eventTypes.upgradingObject) {
			Dispatcher.Listen (Dispatcher.OBJECT_UPGRADING, gameObject);
		} else if (messageToCauseTrigger == eventTypes.gameTimer) {
			Dispatcher.Listen (Dispatcher.TIMER_COMPLETE, gameObject);
		} else if (messageToCauseTrigger == eventTypes.objectDestroyed) {
			Dispatcher.Listen (Dispatcher.OBJECT_DESTROYED, gameObject); 
		} else if (messageToCauseTrigger == CheckPointEvent.eventTypes.population) {
			Dispatcher.Listen(Dispatcher.POPULATION_CHANGED, gameObject);
		} else if (messageToCauseTrigger == CheckPointEvent.eventTypes.energyReceived) {
			Dispatcher.Listen(Dispatcher.ENERGY_RECEIVED, gameObject);
		} else if (messageToCauseTrigger == CheckPointEvent.eventTypes.buddingSiteReady) {
			Dispatcher.Listen(Dispatcher.BUDDING_SITE_READY, gameObject);
		} else if (messageToCauseTrigger == CheckPointEvent.eventTypes.cameraMovementComplete) {
			Dispatcher.Listen(Dispatcher.CAMERA_MOVE_COMPLETE, gameObject);
		}
		
		if (decreaseOccurrenceCountWithPopulation && messageToCauseTrigger != eventTypes.objectDestroyed) Dispatcher.Listen (Dispatcher.OBJECT_DESTROYED, gameObject);
		if (increaseOccurrenceCountWithPopulation && messageToCauseTrigger != eventTypes.objectDestroyed) Dispatcher.Listen (Dispatcher.OBJECT_CREATED, gameObject);
	}
}
