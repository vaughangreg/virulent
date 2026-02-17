using UnityEngine;

/// <summary>
/// Transitions to the next scene when the given button is pressed.
/// </summary>
public class TransitionButton : MonoBehaviour {
	public Texture2D buttonLabel;
	public Button nextButton;
	public string nextScene = "Level 1";
	
	/// <summary>
	/// Set the label for the button.
	/// </summary>
	void Awake() {
		nextButton.GetComponent<Renderer>().material.SetTexture("_Image", buttonLabel);
	}
	
	/// <summary>
	/// Starts listening for the audio idle message.
	/// </summary>
	void Start () {
		Dispatcher.Listen(Dispatcher.AUDIO_IDLE, gameObject);
	}
	
	/// <summary>
	/// Send an audio request to fade out.
	/// </summary>
	void LateUpdate () {
		if (nextButton.isPressed) {
			nextButton.isEnabled = false;
			new MessageAudioRequest(gameObject, MusicManager.State.FadeOut);
		}
	}
	
	/// <summary>
	/// After fading out, transition.
	/// </summary>
	/// <param name="e">
	/// A <see cref="MessageAudioIdle"/>
	/// </param>
	void _OnAudioIdle(MessageAudioIdle e) {
		if (e.fromState == MusicManager.State.FadeOut) Application.LoadLevel(nextScene);
	}
}
