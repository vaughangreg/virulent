using UnityEngine;
using System.Collections;


/// <summary>
/// Shared GUI resources, including styles and constants.
/// </summary>
public class GuiResources : MonoBehaviour {

	
	/// <summary>
	/// These should match the Units enum in ObjectManager.
	/// </summary>
	public Texture2D[] uiObjectTextures;
	
	public const float BUTTON_SIZE_IN_PIXELS = 55.0f;
	public const float POST_RENDER_LINE_HEIGHT = -0.1f; // Was 0.25f;
	public const string NAME_ENERGY_BAR = "Energy Bar";
	
	public static float SCALE;
	
	public static Vector2 feedbackButtonSize = new Vector2(139, 45);
	
	public GUIStyle goalStyle;
	public GUIStyle levelInfoStyle;
	public GUIStyle commandTooltipStyle;
	public GUIStyle blankStyle = new GUIStyle();
	public GUIStyle wideBlankStyle = new GUIStyle();
	public GUIStyle statsStyle;
	public GUIStyle buttonStyle;
	public GUIStyle almanacItemStyle;
	public GUIStyle almanacArrowRight;
	public GUIStyle almanacArrowLeft;
	
	public Material lineMaterial;
	public Material lineCompleteMaterial;
	public GameObject buttonPrefab;
	
	protected static GuiResources m_singleton = null;
	public static GuiResources instance {
		get { return m_singleton; }
	}
	

	

	
	#region Initializers
	/// <summary>
	/// Check for errors & disable black bars from appearing whon rotating.
	/// </summary>
	void Awake() {
#if UNITY_WEBPLAYER
	goalStyle.fontSize = 10;
	levelInfoStyle.fontSize = 12;
	commandTooltipStyle.fontSize = 10;
	wideBlankStyle.fontSize = 12;
	statsStyle.fontSize = 12;
	buttonStyle.fontSize = 12;
	
	
#endif
		if (m_singleton == null) m_singleton = this;
		if (uiObjectTextures.Length != (int)ObjectManager.Unit.Count) Debug.LogError("GuiResources: uiObjectTextures doesn't match Units enum.", gameObject);
		if (buttonPrefab == null) Debug.LogError("GuiResources: No button prefab.", gameObject);
		
		//SETUP SCALING FACTOR
		SCALE = Camera.main.pixelWidth / 1024;
		
		#if UNITY_IPHONE
			
			/*
			iPhoneKeyboard.autorotateToPortrait = false;
			iPhoneKeyboard.autorotateToPortraitUpsideDown = false;
			iPhoneKeyboard.autorotateToLandscapeLeft = false;
			iPhoneKeyboard.autorotateToLandscapeRight = false;
			*/
		#endif
	}
	#endregion
	
	#region Factory methods
	/// <summary>
	/// Return a button with the appropriate unit texture.
	/// </summary>
	/// <param name="aType">
	/// A <see cref="Unit"/>
	/// </param>
	/// <returns>
	/// A <see cref="GameObject"/>
	/// </returns>
	public GameObject ButtonForUnit(ObjectManager.Unit aType) {
		GameObject result = (GameObject)Instantiate(buttonPrefab);
		result.GetComponent<Renderer>().material.SetTexture("_Image", uiObjectTextures[(int)aType]);
		result.name = "Button " + aType;
		return result;
	}
	#endregion
	
	#region Utility methods
	// Screen <0, 0> is bottom-left
	// GUI <0, 0> is upper-left
	/// <summary>
	/// Gets the screen position at most distanceInPixels to the edges.
	/// </summary>
	/// <returns>
	/// The bounded screen position.
	/// </returns>
	/// <param name='source'>
	/// Source.
	/// </param>
	/// <param name='distanceInPixels'>
	/// Distance in pixels.
	/// </param>
	public Vector3 GetScreenPosition(Vector3 source, float distanceInPixels) {
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(source) + Vector3.up * distanceInPixels;

//		screenPosition.x = Mathf.Max(GuiResources.BUTTON_SIZE_IN_PIXELS / 2.0f + distanceInPixels, screenPosition.x);
//		screenPosition.x = Mathf.Min(Screen.width - GuiResources.BUTTON_SIZE_IN_PIXELS / 2.0f - distanceInPixels, screenPosition.x);
		
//		screenPosition.y = Mathf.Min(Screen.height - GuiResources.BUTTON_SIZE_IN_PIXELS / 2.0f - distanceInPixels, screenPosition.y);
//		screenPosition.y = Mathf.Max(GuiResources.BUTTON_SIZE_IN_PIXELS / 2.0f + distanceInPixels, screenPosition.y);
		
		return screenPosition;
	}
	
	public static Vector3 ClampWorldToScreen(Vector3 aPosition) {
		Camera camera = Camera.main;
		Vector3 worldBottomLeft = camera.ViewportToWorldPoint(Vector3.zero);
		Vector3 worldUpperRight = camera.ViewportToWorldPoint(new Vector3(1.0f, 0.934f));	// Matches HUD Y
		
		Vector3 result = new Vector3(Mathf.Min(Mathf.Max(worldBottomLeft.x, aPosition.x), worldUpperRight.x),
		                             aPosition.y,
		                             Mathf.Min(Mathf.Max(worldBottomLeft.z, aPosition.z), worldUpperRight.z));
		return result;
	}
	
	/// <summary>
	/// Centers the source rect at the origin; used only for GUI rects.
	/// </summary>
	/// <returns>
	/// The at GUI point.
	/// </returns>
	/// <param name='sourceRect'>
	/// Source rect.
	/// </param>
	/// <param name='origin'>
	/// Origin.
	/// </param>
	public Rect CenterAtGuiPoint(Rect sourceRect, Vector3 origin) {
		Rect targetRect = new Rect(sourceRect.x + origin.x,
			Screen.height - sourceRect.y - origin.y,
			sourceRect.width, sourceRect.height);
		return targetRect;
		
	}
	
	//Returns a relative scaling factor for gui elements based on ipad resolution (1024x768)
	public static float GetScale() {
		return SCALE;
	}
	#endregion
	
}
