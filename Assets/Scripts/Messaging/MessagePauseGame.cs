using UnityEngine;

public class MessagePauseGame : Message {
	public bool paused;
	public GameObject messageSource;
	
	public MessagePauseGame(GameObject aSource, bool pauseGame) : base(aSource, Dispatcher.PAUSE_GAME)
	{
		paused = pauseGame;
		messageSource = aSource;
		base.Send();
	}
	
	public override string ToArgumentString() {
		return paused.ToString();	
	}
}

