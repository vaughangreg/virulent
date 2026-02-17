using UnityEngine;

/// <summary>
/// Ends the game when an "EndGame" checkpoint is hit.
/// </summary>
public class EndGame : MonoBehaviour {
	public string winningScene;
	public string losingScene;
	
	private bool m_isGameWon;
	private bool m_isGameEnding = false;
	
	/// <summary>
	/// Starts listening.
	/// </summary>
	void Start()
	{
		Dispatcher.Listen(Dispatcher.GAME_OVER, gameObject);
		Dispatcher.Listen(Dispatcher.AUDIO_IDLE, gameObject);
		Dispatcher.Listen(Dispatcher.CHECK_POINT, gameObject);
	}
	
	/// <summary>
	/// Activate the end game status when the request is sent.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageGameOver"/>
	/// </param>
	void _OnGameOver(MessageGameOver m) {
		new MessageAudioRequest(gameObject, MusicManager.State.FadeOut);
		m_isGameWon = m.isGameWon;
		m_isGameEnding = true;
	}
	
	/// <summary>
	/// Go to the appropriate scene when the audio is faded.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageAudioIdle"/>
	/// </param>
	void _OnAudioIdle(MessageAudioIdle m) {
		if (m.fromState == MusicManager.State.FadeOut && m_isGameEnding) {
			Debug.Log("Game is now ending.");
			if (m_isGameWon) Application.LoadLevel(winningScene);
			else Application.LoadLevel(losingScene);
		}
	}
	
	/// <summary>
	/// Send a game over message for "EndGame" checkpoints.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageCheckPoint"/>
	/// </param>
	void _OnCheckPoint(MessageCheckPoint m) {
		if (m.checkPointString == "EndGame" && !m_isGameEnding)
		{
			new MessageGameOver(gameObject, true);
		}
	}
}
