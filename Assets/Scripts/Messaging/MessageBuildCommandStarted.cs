using UnityEngine;

public class MessageBuildCommandStarted : Message {
	public GameObject producerObj;
	public float timeNeededToBuild;
	public string nameOfBuildingObject;
	public ObjectManager.Unit unit;
	
	/// <summary>
	/// Sends a message with the colliding/triggering objects
	/// </param>
	public MessageBuildCommandStarted(GameObject producerObject, string buildCommandName, float timeForBuild) 
		: base(producerObject, Dispatcher.BUILD_COMMAND_START) 
	{ 
		producerObj = producerObject;
		nameOfBuildingObject = buildCommandName;
		timeNeededToBuild = timeForBuild; 
		base.Send();
	}
	
	public MessageBuildCommandStarted(GameObject producerObject, string buildCommandName, float timeForBuild, ObjectManager.Unit unitToMake) 
		: base(producerObject, Dispatcher.BUILD_COMMAND_START) 
	{ 
		producerObj = producerObject;
		nameOfBuildingObject = buildCommandName;
		timeNeededToBuild = timeForBuild; 
		unit = unitToMake;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return nameOfBuildingObject;	
	}
}
