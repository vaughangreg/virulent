using UnityEngine;

public class MessageTimerComplete: Message {
	public GameObject timerGameObject;
	
	/// <summary>
	/// Sends a message with the gameobject that started the timer
	/// </param>
	public MessageTimerComplete(GameObject gameObjThatStartedTimer) : base(gameObjThatStartedTimer, Dispatcher.TIMER_COMPLETE) 
	{
		timerGameObject = gameObjThatStartedTimer;
		base.Send();
	}
	
}
