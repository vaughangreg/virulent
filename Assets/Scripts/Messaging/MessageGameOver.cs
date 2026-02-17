using UnityEngine;

public class MessageGameOver : Message {
	public bool isGameWon;
	
	public MessageGameOver(GameObject aSource, bool gameWasWon) 
		: base (aSource, Dispatcher.GAME_OVER) 
	{
		isGameWon = gameWasWon;
		base.Send();
	}
	public override string ToArgumentString() {
		return isGameWon.ToString();	
	}
}

