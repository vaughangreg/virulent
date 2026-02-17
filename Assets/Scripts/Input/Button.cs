using UnityEngine;
using System.Collections;

/// <summary>
/// Emulate a 2D button with an in-game object. Object should have a ButtonShader for its material.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Selectable))]
public class Button : MonoBehaviour {
	public AudioClip touchFx;
	public AudioClip tapFx;
	public AudioClip cancelFx;
	
	public Color     enabledTint;
	public Color     disabledTint;
	
	/// <summary>
	/// Is the button pressed? If it was, it's not on the next frame.
	/// </summary>
	protected bool m_isPressed = false;
	public bool isPressed {
		get {
			bool result = m_isPressed;
			if (result) new MessageClickButton(gameObject);
			m_isPressed = false;
			return result;
		}
	}
	
	/// <summary>
	/// Enable or disable the button.
	/// </summary>
	protected bool m_isEnabled = true;
	public bool isEnabled {
		set {
			GetComponent<Renderer>().material.SetColor("_Tint", value ? enabledTint : disabledTint);
			m_isEnabled = value;
		}
	}
	
	/// <summary>
	/// Cooling down prevents interaction but doesn't change enabled color.
	/// </summary>
	protected bool m_isCoolingDown = false;
	public bool isCoolingDown {
		get { return m_isCoolingDown; }
		set { m_isCoolingDown = value; }
	}
	
	/// <summary>
	/// Change the swipe overlay amount.
	/// </summary>
	public float countdown {
		set {
			// 0.0 is no overlay.
			GetComponent<Renderer>().material.SetFloat("_Cutoff", (value == 0) ? 0.0f : (1.0f - value));
		}
	}
	
	/// <summary>
	/// Position the button just in front of the camera.
	/// </summary>
	void Awake() {
		transform.position = new Vector3(transform.position.x,
		                                 Camera.main.transform.position.y - Camera.main.nearClipPlane - 0.1f,
		                                 transform.position.z);
	}
	
	/// <summary>
	/// Ensure the button is 'up'.
	/// </summary>
	void OnDeselect() {
		GetComponent<Renderer>().material.SetFloat("_IsDown", 0.0f);
	}
	
	#region Events
	/// <summary>
	/// Marks button as pressed.
	/// </summary>
	void OnTouch(InputManager source) {
		if (!m_isEnabled || m_isCoolingDown) return;
		GetComponent<Renderer>().material.SetFloat("_IsDown", 1.0f);
		if (source.selection != gameObject && touchFx) MusicManager.PlaySfx(touchFx);
	}
	
	/// <summary>
	/// Marks the button as up.
	/// </summary>
	void OnTap() {
		if (!m_isEnabled || m_isCoolingDown) return;
		GetComponent<Renderer>().material.SetFloat("_IsDown", 0.0f);
		m_isPressed = true;
		if (tapFx) MusicManager.PlaySfx(tapFx);
	}
	
	/// <summary>
	/// The same as starting to touch the button.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStart(InputManager source) {
		Debug.Log("Slide started.", gameObject);
		if (!m_isEnabled || m_isCoolingDown) return;
		OnTouch(source);
	}
	
	/// <summary>
	/// Deselect if we move beyond the button boundary.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStay(InputManager source) {
		Debug.Log("Still sliding.", gameObject);
		Vector3 position = source.position;
		if (source.ObjectAtScreen(position) != gameObject) {
			GetComponent<Renderer>().material.SetFloat("_IsDown", 0.0f);
			//source.ClearSelectionIfNeeded();
		} else if (source.ObjectAtScreen(position) == gameObject) {
			GetComponent<Renderer>().material.SetFloat("_IsDown", 1.0f);
		}
	}
	
	/// <summary>
	/// Tap if we're still on the button.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStop(InputManager source) {
		Debug.Log("Slide stopped.", gameObject);
		Vector3 position = source.position;
		if (source.ObjectAtScreen(position) == gameObject) {
			OnTap();
		}
		else {
			GetComponent<Renderer>().material.SetFloat("_IsDown", 0.0f);
			source.ClearSelectionIfNeeded();
			if (cancelFx) MusicManager.PlaySfx(cancelFx);
		}
	}
	
	#endregion
}
