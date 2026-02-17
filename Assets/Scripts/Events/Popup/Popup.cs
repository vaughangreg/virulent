using UnityEngine;

public class Popup : MonoBehaviour {
	protected const float COLLIDER_DEPTH = 0.5f;
	
	[HideInInspector]public Texture2D image;
	[HideInInspector]public string    additionalText;
	[HideInInspector]public string    titleName;
	[HideInInspector]public float     timeToLive;
	[HideInInspector]public bool      isButton;
	[HideInInspector]public PopupManager.ButtonActions action;

	public GameObject imageChildObject;
	public TextMesh   titleTextMesh;
	public TextMesh   additionalTextMesh;
	
	public float  flashRate = 0.2f;
	public Color  baseColor;
	public Color  flashColor;
	
	private Vector3 m_scaleOriginal;
	private float m_passedTime = 0.0f;
	private float orthoScale   = 280;
	
	private float scaleFloat    { get { return Camera.main.orthographicSize / orthoScale; }}
	private Vector3 startPos    { get { return Camera.main.ViewportToWorldPoint(new Vector3(0.86f, 0.89f, 10.0f)); } }
	private Vector3 endPos      { get { return Camera.main.ViewportToWorldPoint(new Vector3(0.86f, 1.2f,  10.0f)); } }
	private float delay         { get { return timeToLive - 1;}}
	private bool m_gameIsPaused { get { return Mathf.Approximately(Time.timeScale, Constants.TIME_MIN); }}
	
	void Awake() {
		m_scaleOriginal    = gameObject.transform.localScale;
		transform.position = startPos;
		
		// Not strictly needed, but it keeps the scene clean.
		transform.parent = Camera.main.transform;
	}
	
	void Start() {
		//Setup Parameters given by the PopupManager
		imageChildObject.GetComponent<MeshRenderer>().material.mainTexture = image;
		
		//Set title text
		titleTextMesh.text = titleName;
		additionalTextMesh.text = additionalText;
	}
	
	void LateUpdate() {
		// Move the popup. Note that we can't use
		// MoveBetweenTwoPoints anymore because it doesn't handle
		// the case where the orthographic size is also changing.
		m_passedTime         += Time.deltaTime;
		transform.localScale  = m_scaleOriginal * scaleFloat;
		
		BoxCollider boxCollider    = (BoxCollider)GetComponent<Collider>();
		Vector3     colliderScaled = boxCollider.size;
		colliderScaled.y = COLLIDER_DEPTH / transform.localScale.y;
		boxCollider.size = colliderScaled;
		
		transform.position               = Virulent.Math.SmoothLerp(startPos, endPos, m_passedTime - delay);
		
		// Make the title text flash
		float flashTime                        = m_passedTime * Time.deltaTime;
		titleTextMesh.GetComponent<Renderer>().material.color  = Color.Lerp(baseColor,flashColor,Mathf.PingPong(flashTime, 1.0f));
	}
	
	void OnTap(InputManager source)   { ButtonAction(source); }
	void OnTouch(InputManager source) { ButtonAction(source); }
	void OnSlide(InputManager source) { ButtonAction(source); }
	
	void ButtonAction(InputManager source) {
		if (isButton && !m_gameIsPaused) {
			switch (action) {
				case PopupManager.ButtonActions.OpenAchievement:
					break;
				case PopupManager.ButtonActions.OpenAlmanac:
					new MessageViewAlmanac(gameObject,titleName);
					break;
			}
		}
	}
}

