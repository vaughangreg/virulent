using UnityEngine;
using System.Collections;
using System.Threading;
using ZXing;
using ZXing.QrCode;
using System.Collections.Generic;
using System;

public abstract class ADAGEMenuPopup
{
	public abstract void Draw();
}

public class ADAGEConnectionFailurePopup : ADAGEMenuPopup
{
	private Rect drawRect;
	private Rect labelRect;
	private Rect yesRect;
	private Rect noRect;

	private int numAttempts;

	public ADAGEConnectionFailurePopup(int attempts)
	{
		drawRect = new Rect(256f, 192f, 512f, 384f);
		labelRect = new Rect(0f, 0f, 512f, 300f);

		yesRect = new Rect(10,304,241,60);
		noRect = new Rect(261,304,241,60);

		numAttempts = attempts;
	}

	public override void Draw()
	{
		GUI.BeginGroup(drawRect, "", "box");
		{
			GUI.Label(labelRect, string.Format("The system has made {0} failed attempts to connect to ADAGE. Continue in Offline Mode?", numAttempts));

			if(GUI.Button(yesRect, "Yes"))
			{
				Messenger.Broadcast(ADAGE.k_OnGameStart);
			}

			if(GUI.Button(noRect, "No"))
			{
				ADAGEMenu.ShowLogin();
			}
		}
		GUI.EndGroup();
	}
}

public abstract class ADAGEMenuPanel
{
	public abstract void Draw(MonoBehaviour owner = null);
	public abstract IEnumerator Update();
	public abstract void OnEnable(MonoBehaviour owner = null);
	public abstract void OnDisable(MonoBehaviour owner = null);
	
	protected bool visible = false;
	
	public void Enable()
	{
		visible = true;
		OnEnable();
	}
	
	public void Disable()
	{
		visible = false;
		OnDisable();
	}

	public virtual void OnApplicationQuit()
	{

	}

	protected void ClearFocus()
	{
		GUIUtility.keyboardControl = -1;	
	}
}

public class ADAGERegisterPanel : ADAGEMenuPanel
{
	private const string kDefaultUsernameText = "Please Enter Username";
	private const string kDefaultPasswordText = "Please Enter Password";
	private const string kDefaultConfirmText = "Please Confirm Password";
	private const string kDefaultEmailText = "Please Enter Email (Optional)";
	
	private string username;
	private string password;
	private string confirm;
	private string email;
	
	private int keyboardFieldID;
	
	private bool loading;
	private int usernameFieldID;
	private int passwordFieldID;
	private int confirmFieldID;
	private int emailFieldID;
	
	private GUIStyle usernameStyle;
	private GUIStyle passwordStyle;
	private GUIStyle confirmStyle;
	private GUIStyle emailStyle;
	
	private GUIStyle buttonStyle;

	private Rect usernameFieldRect;
	private Rect passwordFieldRect;
	private Rect confirmFieldRect;
	private Rect emailFieldRect;
	private Rect submitButtonRect;
	private Rect backButtonRect;

	public ADAGERegisterPanel()
	{
		username = "";
		password = "";
		confirm = "";
		email = "";

		usernameFieldRect = new Rect(212,284,600,50);
		passwordFieldRect = new Rect(212,344,600,50);
		confirmFieldRect = new Rect(212,404,600,50);
		emailFieldRect = new Rect(212,464,600,50);
		submitButtonRect = new Rect(212,554,290,60);
		backButtonRect = new Rect(522,554,290,60);

		loading = true;
	}
	
	public override void Draw(MonoBehaviour owner = null)
	{
		if(loading)
		{
			InitStyles();
			loading = false;
		}
			
		usernameFieldID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
		username = GUI.TextField(usernameFieldRect, username, usernameStyle);
		
		passwordFieldID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
		if(keyboardFieldID == passwordFieldID || password != kDefaultPasswordText)
		{
			password = GUI.PasswordField(passwordFieldRect, password, '*', passwordStyle);
		}
		else
		{
			password = GUI.TextField(passwordFieldRect, password, passwordStyle);	
		}
		
		confirmFieldID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
		if(keyboardFieldID == confirmFieldID || confirm != kDefaultConfirmText)
		{
			confirm = GUI.PasswordField(confirmFieldRect, confirm, '*', confirmStyle);
		}
		else
		{
			confirm = GUI.TextField(confirmFieldRect, confirm, confirmStyle);	
		}
		
		emailFieldID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
		email = GUI.TextField(emailFieldRect, email, emailStyle);

		if(GUI.Button(submitButtonRect, "Submit", buttonStyle))
		{
			ClearFocus();
			ADAGEMenu.ShowSplash();
			if(IsValid())
			{
				ADAGE.RegisterPlayer(username,email,password,confirm);
			}
		}
		
		if(GUI.Button(backButtonRect, "Back", buttonStyle))
		{
			ClearFocus();
			ADAGEMenu.ShowHome();
		}	
		
		CheckFields();
	}
		
	public override IEnumerator Update()
	{
		yield return null;	
	}
	
	public override void OnEnable(MonoBehaviour owner = null)
	{
		
	}
	
	public override void OnDisable(MonoBehaviour owner = null)
	{
		
	}
		
	private bool IsValid()
	{
		if(username.Trim() == "" || username == kDefaultUsernameText)
		{
			ADAGEMenu.ShowError("Username cannot be blank");
			return false;
		}

		if(password.Trim() == "" || password == kDefaultPasswordText)
		{
			ADAGEMenu.ShowError("Password cannot be blank");
			return false;
		}
		
		if(email.Trim() == "" || email == kDefaultEmailText)
		{
			ADAGEMenu.ShowError("Email cannot be blank");
			return false;
		}

		if(password != confirm || confirm == kDefaultConfirmText)
		{
			ADAGEMenu.ShowError("Passwords do not match");
			return false;
		}

		return true;
	}

	private void InitStyles()
	{
		usernameStyle = new GUIStyle(GUI.skin.GetStyle("textfield"));	
		usernameStyle.fontSize = 22;
		usernameStyle.fontStyle = FontStyle.BoldAndItalic;
		usernameStyle.alignment = TextAnchor.MiddleCenter;
		
		passwordStyle = new GUIStyle(GUI.skin.GetStyle("textfield"));	
		passwordStyle.fontSize = 22;
		passwordStyle.fontStyle = FontStyle.BoldAndItalic;
		passwordStyle.alignment = TextAnchor.MiddleCenter;
		
		confirmStyle = new GUIStyle(GUI.skin.GetStyle("textfield"));	
		confirmStyle.fontSize = 22;
		confirmStyle.fontStyle = FontStyle.BoldAndItalic;
		confirmStyle.alignment = TextAnchor.MiddleCenter;
		
		emailStyle = new GUIStyle(GUI.skin.GetStyle("textfield"));	
		emailStyle.fontSize = 22;
		emailStyle.fontStyle = FontStyle.BoldAndItalic;
		emailStyle.alignment = TextAnchor.MiddleCenter;
		
		buttonStyle = new GUIStyle(GUI.skin.GetStyle("button"));	
		buttonStyle.fontSize = 22;
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	private void CheckFields()
	{		
		keyboardFieldID = GUIUtility.keyboardControl;
		
		if(username.Trim() == "" && keyboardFieldID != usernameFieldID)
		{
			username = kDefaultUsernameText;	
			usernameStyle.fontStyle = FontStyle.BoldAndItalic;
		}
		else if(username == kDefaultUsernameText && keyboardFieldID == usernameFieldID)
		{
			username = "";
			usernameStyle.fontStyle = FontStyle.Bold;			
		}
		
		if(password.Trim() == "" && keyboardFieldID != passwordFieldID)
		{
			password = kDefaultPasswordText;	
			passwordStyle.fontStyle = FontStyle.BoldAndItalic;
		}
		else if(password == kDefaultPasswordText && keyboardFieldID == passwordFieldID)
		{
			password = "";
			passwordStyle.fontStyle = FontStyle.Bold;			
		}
		
		if(confirm.Trim() == "" && keyboardFieldID != confirmFieldID)
		{
			confirm = kDefaultConfirmText;	
			confirmStyle.fontStyle = FontStyle.BoldAndItalic;
		}
		else if(confirm == kDefaultConfirmText && keyboardFieldID == confirmFieldID)
		{
			confirm = "";
			confirmStyle.fontStyle = FontStyle.Bold;			
		}
		
		if(email.Trim() == "" && keyboardFieldID != emailFieldID)
		{
			email = kDefaultEmailText;	
			emailStyle.fontStyle = FontStyle.BoldAndItalic;
		}
		else if(email == kDefaultEmailText && keyboardFieldID == emailFieldID)
		{
			email = "";
			emailStyle.fontStyle = FontStyle.Bold;			
		}
	}
}

public class ADAGEQRGroup
{
	public string group = "";
}

public class ADAGEQRPanel : ADAGEMenuPanel
{
	private bool loading;
	private bool decoding;

	private WebCamTexture cameraTexture;
	private Thread qrThread;

	private Rect imagePanelRect;
	private Rect labelRect;
	private Rect backButtonRect;

	private GUIStyle buttonStyle;
	
	private Color32[] c;
	private int W, H;

	private string qrResult = "";
	private string lastQrResult = "";

	public ADAGEQRPanel()
	{
		imagePanelRect = new Rect(362,284,320,240);
		labelRect = new Rect(212,554,290,60);
		backButtonRect = new Rect(522,554,290,60);

		loading = true;
		decoding = false;
	}
	
	public override void Draw(MonoBehaviour owner = null)
	{
		if(loading)
		{
			InitStyles();
			loading = false;
		}
		
		if(cameraTexture != null)
			GUI.DrawTexture(imagePanelRect, cameraTexture, ScaleMode.StretchToFill);

		GUI.Label(labelRect, "Point your camera at the QR code");
				
		if(GUI.Button(backButtonRect, "Back", buttonStyle))
		{
			Disable();
			ADAGEMenu.ShowLogin();
		}	
	}
		
	public override void OnApplicationQuit()
	{
		KillThread();
	}

	public override IEnumerator Update()
	{			
		if(cameraTexture == null)
		{		
#if UNITY_WEBPLAYER
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
			
			if(Application.HasUserAuthorization(UserAuthorization.WebCam))
			{
				InitCamera();
				yield return null;
			}
			else
			{
				ADAGEMenu.ShowLogin();
			}	
#else
			InitCamera();
#endif
		}
		else
		{
			if(!cameraTexture.isPlaying)
				cameraTexture.Play();
			else
			{
				if(W != cameraTexture.width)
					W = cameraTexture.width;
				if(H != cameraTexture.height)
					H = cameraTexture.height;
			}
		}
		
		if (c == null)
		{			
			c = cameraTexture.GetPixels32();
		}

		if(qrResult != "" && !decoding)
		{
			lastQrResult = qrResult;

			try
			{
				ADAGEQRGroup qrData = LitJson.JsonMapper.ToObject<ADAGEQRGroup>(qrResult);
				ADAGEMenu.ShowSplash();
				ADAGE.ConnectWithQR(qrData.group);
			}
			catch
			{
				if (ADAGEMenu.instance != null)
				{
					ADAGEMenu.ShowError("Invalid QR Code");
					StartThread();
				}
			}
			qrResult = "";
		}
		
		yield return null;
	}
	
	public override void OnEnable(MonoBehaviour owner = null)
	{
		InitCamera();	
	}
	
	public override void OnDisable(MonoBehaviour owner = null)
	{
		if (cameraTexture != null)
		{
			cameraTexture.Pause();
		}

		KillThread();
		ClearFocus();
	}
	
	private void InitCamera()
	{
		cameraTexture = new WebCamTexture();
		cameraTexture.requestedHeight = 480;
		cameraTexture.requestedWidth = 640;
		
		if (cameraTexture != null)
		{
			cameraTexture.Play();
			W = cameraTexture.width;
			H = cameraTexture.height;
		}

		StartThread();	
	}

	private void StartThread()
	{
		decoding = true;

		qrThread = new Thread(DecodeQR);
		qrThread.Start();
	}

	private void KillThread()
	{
		decoding = false;
		qrThread.Abort();
	}

	private void InitStyles()
	{		
		buttonStyle = new GUIStyle(GUI.skin.GetStyle("button"));	
		buttonStyle.fontSize = 22;
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	private void DecodeQR()
	{
		// create a reader with a custom luminance source
		BarcodeReader barcodeReader = new BarcodeReader {AutoRotate = false, TryHarder = false};
		
		while (decoding)
		{		
			try
			{
				// decode the current frame
				if(c != null)
				{
					Result result = barcodeReader.Decode(c, W, H);
					if (result != null)
					{
						if(result.Text != lastQrResult)
						{
							decoding = false;
							qrResult = result.Text;
						}
					}
			
					// Sleep a little bit and set the signal to get the next frame
					Thread.Sleep(200);
					c = null;
				}
			}
			catch(System.Exception e)
			{
				Debug.Log (e.ToString());
			}
		}
	}
}

public class ADAGELoginPanel : ADAGEMenuPanel
{
	private const string kDefaultUsernameText = "Please Enter Username";
	private const string kDefaultPasswordText = "Please Enter Password";
	
	private string username;
	private string password;
	
	private int keyboardFieldID;
	
	private bool loading;
	private int usernameFieldID;
	private int passwordFieldID;
	private GUIStyle usernameStyle;
	private GUIStyle passwordStyle;
	private GUIStyle buttonStyle;
	private GUIStyle imageButtonStyle;
	
	private Texture2D facebookButton;

	private Rect usernameFieldRect;
	private Rect passwordFieldRect;
	private Rect backButtonRect;
	private Rect loginButtonRect;
	private Rect registerButtonRect;
	private Rect facebookButtonRect;
	private Rect qrButtonRect;
	
	private int loginAttempts = 0;
	private readonly int maxLoginAttempts = 3;

	public ADAGELoginPanel()
	{
		username = "";
		password = "";	

		usernameFieldRect = new Rect(212,284,600,50);
		passwordFieldRect = new Rect(212,344,600,50);

		/*loginButtonRect = new Rect(212,474,290,60);
		registerButtonRect = new Rect(522,474,290,60);
		facebookButtonRect = new Rect(212,554,290,60);
		qrButtonRect = new Rect(522,554,290,60);*/

		loginButtonRect = new Rect(212,474,188,60);
		registerButtonRect = new Rect(418,474,188,60);
		qrButtonRect = new Rect(624,474,188,60);

		if(ADAGE.AllowFacebook)
		{
			facebookButtonRect = new Rect(315,554,188,60);
			backButtonRect = new Rect(521,554,188,60);
		}
		else
		{
			backButtonRect = new Rect(418,554,188,60);
		}

		facebookButton = Resources.Load("Images/facebook-button") as Texture2D;

		Messenger<int, string>.AddListener(ADAGE.k_OnLoginComplete, OnLoginComplete);

		loading = true;
	}
	
	public override void Draw(MonoBehaviour owner = null)
	{
		if(loading)
		{
			InitStyles();
			CheckFields();
			loading = false;
		}
			
		usernameFieldID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
		username = GUI.TextField(usernameFieldRect, username, usernameStyle);

		passwordFieldID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
		if(keyboardFieldID == passwordFieldID || password != kDefaultPasswordText)
		{
			password = GUI.PasswordField(passwordFieldRect, password, '*', passwordStyle);
		}
		else
		{
			password = GUI.TextField(passwordFieldRect, password, passwordStyle);	
		}

		GUI.enabled = (password != kDefaultPasswordText && username != kDefaultUsernameText && password.Trim().Length != 0 && username.Trim().Length != 0);
		if(GUI.Button(loginButtonRect, "Login", buttonStyle))
		{
			ClearFocus();
			loginAttempts++;
			ADAGEMenu.ShowSplash();
			ADAGE.LoginPlayer(username.Trim(), password.Trim());
		}
		GUI.enabled = true;

		if(GUI.Button(registerButtonRect, "Login as Guest", buttonStyle))
		{
			ClearFocus();
			ADAGEMenu.ShowSplash();
			ADAGE.ConnectAsGuest();
		}
		
		if(GUI.Button(backButtonRect, "Back", buttonStyle))
		{
			ClearFocus();
			ADAGEMenu.ShowHome();
		}

		if(ADAGE.AllowFacebook)
		{
			/*if(GUI.Button(facebookButtonRect, facebookButton, imageButtonStyle))
			{
				ClearFocus();				
			}*/
			if(GUI.Button(facebookButtonRect, "", imageButtonStyle))
			{
				ClearFocus();				
			}
		}
		
		if(GUI.Button(qrButtonRect, "Use QR Code", buttonStyle))
		{
			ClearFocus();
			ADAGEMenu.ShowQR();
		}

		/*if(GUI.Button(startButtonRect, startButtonText, buttonStyle))
		{
			ADAGEMenu.LoadLevel();
		}*/

		CheckFields();
	}
		
	public override IEnumerator Update()
	{
		yield return null;	
	}
	
	public override void OnEnable(MonoBehaviour owner = null)
	{

	}
	
	public override void OnDisable(MonoBehaviour owner = null)
	{
		
	}
		
	private void InitStyles()
	{
		usernameStyle = new GUIStyle(GUI.skin.GetStyle("textfield"));	
		usernameStyle.fontSize = 22;
		usernameStyle.fontStyle = FontStyle.BoldAndItalic;
		usernameStyle.alignment = TextAnchor.MiddleCenter;
		
		passwordStyle = new GUIStyle(GUI.skin.GetStyle("textfield"));	
		passwordStyle.fontSize = 22;
		passwordStyle.fontStyle = FontStyle.BoldAndItalic;
		passwordStyle.alignment = TextAnchor.MiddleCenter;
		
		buttonStyle = new GUIStyle(GUI.skin.GetStyle("button"));	
		buttonStyle.fontSize = 22;
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		
		imageButtonStyle = new GUIStyle(GUI.skin.GetStyle("button"));	
		imageButtonStyle.alignment = TextAnchor.MiddleCenter;
		imageButtonStyle.normal.background = facebookButton;
		imageButtonStyle.hover.background = facebookButton;
		imageButtonStyle.focused.background = facebookButton;
		imageButtonStyle.active.background = facebookButton;
		imageButtonStyle.imagePosition = ImagePosition.ImageOnly;
		imageButtonStyle.contentOffset = Vector2.zero;
		imageButtonStyle.padding = new RectOffset(0,0,0,0);
	}
	
	private void CheckFields()
	{		
		keyboardFieldID = GUIUtility.keyboardControl;

		if(username.Trim() == "" && keyboardFieldID != usernameFieldID)
		{
			username = kDefaultUsernameText;	
			usernameStyle.fontStyle = FontStyle.BoldAndItalic;
		}
		else if(username == kDefaultUsernameText && keyboardFieldID == usernameFieldID)
		{
			username = "";
			usernameStyle.fontStyle = FontStyle.Bold;	
		}
		
		if(password.Trim() == "" && keyboardFieldID != passwordFieldID)
		{
			password = kDefaultPasswordText;	
			passwordStyle.fontStyle = FontStyle.BoldAndItalic;
		}
		else if(password == kDefaultPasswordText && keyboardFieldID == passwordFieldID)
		{
			password = "";
			passwordStyle.fontStyle = FontStyle.Bold;			
		}
	}

	private void OnLoginComplete(int id, string token)
	{
		username = "";
		password = "";
	}

	private void OnADAGELoginFailed()
	{
		if(loginAttempts >= maxLoginAttempts)
		{
			ADAGEMenu.ShowPopup(new ADAGEConnectionFailurePopup(loginAttempts));
		}
	}

	public void SetUsername(string username)
	{
		this.username = username;
	}
}

public class ADAGEHomePanel : ADAGEMenuPanel
{	
	private int keyboardFieldID;
	
	private bool loading;
	private GUIStyle buttonStyle;

	private Rect currentUsersRect;
	private Rect loginButtonRect;
	private Rect registerButtonRect;
	private Rect startButtonRect;

	private string startButtonText;

	public ADAGEHomePanel()
	{
		loginButtonRect = new Rect(522,284,290,125);
		registerButtonRect = new Rect(522,419,290,125);
		startButtonRect = new Rect(212,614,600,50);
				
		currentUsersRect = new Rect(212,284,290,260);

		loading = true;
	}
	
	public override void Draw(MonoBehaviour owner = null)
	{
		if(loading)
		{
			InitStyles();
			loading = false;
		}
		
		GUILayout.BeginArea(currentUsersRect, "", "box");
		{
			GUILayout.BeginVertical();
			{
				int badUser = -1;
				foreach(KeyValuePair<int, ADAGEUser> user in ADAGE.users)
				{
					if(GUILayout.Button(user.Value.playerName, buttonStyle, GUILayout.Height(60)))
					{
						badUser = user.Key;
					}
				}

				if(badUser > -1)
				{
					ADAGE.users.Remove(badUser);
					badUser = -1;
				}
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();

		GUI.enabled = (ADAGE.users != null && ADAGE.users.Count < ADAGE.k_MaxUsers);
		if(GUI.Button(loginButtonRect, "Login", buttonStyle))
		{
			ClearFocus();
			ADAGEMenu.ShowLogin();
		}
		GUI.enabled = true;
		
		if(GUI.Button(registerButtonRect, "Register", buttonStyle))
		{
			ClearFocus();
			ADAGEMenu.ShowRegistration();	
		}

		GUI.enabled = (ADAGE.AllowGuestLogin || (ADAGE.users != null && ADAGE.users.Count > 0)); 
		if(GUI.Button(startButtonRect, startButtonText, buttonStyle))
		{
			Messenger.Broadcast(ADAGE.k_OnGameStart);
		}
		GUI.enabled = true;
	}
	
	public override IEnumerator Update()
	{
		if(ADAGE.AllowGuestLogin && ADAGE.users != null && ADAGE.users.Count == 0)
			startButtonText = "Start Game as Guest";
		else
			startButtonText = "Start Game";

		yield return null;	
	}
	
	public override void OnEnable(MonoBehaviour owner = null)
	{
		
	}
	
	public override void OnDisable(MonoBehaviour owner = null)
	{
		
	}
	
	private void InitStyles()
	{		
		buttonStyle = new GUIStyle(GUI.skin.GetStyle("button"));	
		buttonStyle.fontSize = 22;
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
	}
}

public class ADAGESplashPanel : ADAGEMenuPanel
{			
	private Rect drawRect;
	private Texture2D splash;

	public ADAGESplashPanel()
	{
		drawRect = new Rect(0f,0f,1024f,769f);
		splash = Resources.Load("Images/GLS_Splash") as Texture2D;
	}
	
	public override void Draw(MonoBehaviour owner = null)
	{	
		GUI.DrawTexture(drawRect, splash);
	}
	
	public override IEnumerator Update()
	{		
		yield return null;	
	}
	
	public override void OnEnable(MonoBehaviour owner = null)
	{
		
	}
	
	public override void OnDisable(MonoBehaviour owner = null)
	{
		
	}
}

public class ADAGEMenu : MonoBehaviour 
{ 	
	public static readonly Vector2 defaultResolution = new Vector2(1024,768);
	
	private const int errorDuration = 4;
	
	public static ADAGEMenu instance;

	public static void Deactivate()
	{
		instance.gameObject.SetActive(false);
	}

	public static void SetCurrentUser(ADAGEUser user)
	{
		instance.loginPanel.SetUsername(user.email);
	//	instance.currentUser = user;
	}

	public static bool IsNull
	{
		get
		{
			if( instance == null || instance.gameObject == null ) {
				return true;
			}
			return !instance.gameObject.activeInHierarchy;
		}
	}

	public static void ShowHome()
	{
		if(IsNull)
			return;

		if(instance.currentPanel != instance.homePanel)
		{
			TurnOffCurrentPanel();
			
			instance.currentPanel = instance.homePanel;
			instance.currentPanel.Enable();
		}
		
		instance.popup = null;
	}

	public static void ShowLogin()
	{
		if(IsNull)
			return;

		if(instance.currentPanel != instance.loginPanel)
		{
			TurnOffCurrentPanel();
			
			instance.currentPanel = instance.loginPanel;
			instance.currentPanel.Enable();
		}

		instance.popup = null;
	}
	
	public static void ShowQR()
	{
		if(IsNull)
			return;

		if(instance.currentPanel != instance.qrPanel)
		{
			TurnOffCurrentPanel();
			
			instance.currentPanel = instance.qrPanel;
			instance.currentPanel.Enable();
		}
	}
	
	public static void ShowRegistration()
	{
		if(IsNull)
			return;

		if(instance.currentPanel != instance.registrationPanel)
		{
			TurnOffCurrentPanel();
			
			instance.currentPanel = instance.registrationPanel;
			instance.currentPanel.Enable();
		}

		instance.popup = null;
	}

	public static void ShowSplash()
	{
		if(IsNull)
			return;

		if(instance.currentPanel != instance.splashPanel)
		{
			TurnOffCurrentPanel();
			
			instance.currentPanel = instance.splashPanel;
			instance.currentPanel.Enable();
		}
		
		instance.popup = null;
	}

	public static void ShowLast()
	{
		if(IsNull)
			return;

		if(instance.previousPanel == null)
		{
			instance.previousPanel = instance.currentPanel;
			return;
		}

		if(instance.currentPanel != instance.previousPanel)
		{
			if(instance.currentPanel != null)
				instance.currentPanel.Disable();

			ADAGEMenuPanel temp = instance.previousPanel;
			instance.previousPanel = instance.currentPanel;
			instance.currentPanel = temp;
			instance.currentPanel.Enable();
		}
		
		instance.popup = null;
	}

	private static void TurnOffCurrentPanel()
	{
		if(IsNull)
			return;

		if(instance.currentPanel != null)
		{
			instance.currentPanel.Disable();
			instance.previousPanel = instance.currentPanel;
		}
	}

	public static void ShowError(string error)
	{
		ShowLast();
		instance.error = error;		
	}
		
	public static void ShowPopup(ADAGEMenuPopup popup)
	{
		instance.popup = popup;
	}

	//setting force to true allows you to load the level as long as there is a level to load, regardless of the level you are currently on
	/*public static void LoadLevel(bool force = false)
	{
		if(instance.OnLoginLoadLevel)
		{
			if(instance.nextLevel != -1 && (force || (Application.loadedLevel != instance.nextLevel && Application.loadedLevel == 0)))
			{
				Application.LoadLevel(instance.nextLevel);
			}
			else
			{
				Debug.LogWarning("ADAGEMenu object is set to load a level upon successful login, but no level was provided in the inspector.");
			}
		}
		
		instance.gameObject.SetActive(false);
	}*/
	
	public Texture2D backgroundImage;
	public Texture2D overlay;
	public bool stretchBackgroundAspect;
	
	public GUISkin skin;
		
	private Texture2D logo;
	private GUIStyle invisibleButtonStyle;
	
	private Rect screenRect;
	private GUIStyle screenStyle;
	
	private Rect backgroundRect;
	private Rect overlayRect;
	private Rect menuRect;
	
	private Rect logoRect;
	private Rect errorRect;

	private bool loading;
	private string error;
	private GUIStyle errorStyle;
	private float errorTime;

	private ADAGEMenuPanel previousPanel;
	private ADAGEMenuPanel currentPanel;
	private ADAGESplashPanel splashPanel;
	private ADAGEHomePanel homePanel;
	private ADAGELoginPanel loginPanel;
	private ADAGEQRPanel qrPanel;
	private ADAGERegisterPanel registrationPanel;

	private ADAGEMenuPopup popup;

	public void Awake()
	{
		if(instance != null) 
		{
			Debug.LogWarning("You have multiple copies of the ADAGEMenu object running. Overriding...");
		}
		DontDestroyOnLoad(this);
		instance = this;

		Messenger.AddListener(ADAGE.k_OnGameStart, OnGameStart);
		Messenger<string>.AddListener(ADAGE.k_OnError, ShowError);
		Messenger<string>.AddListener(ADAGE.k_OnConnectionTimeout, OnConnectionTimeout);
		Messenger<int, string>.AddListener(ADAGE.k_OnLoginComplete, OnLogin);

		homePanel = new ADAGEHomePanel();
		loginPanel = new ADAGELoginPanel();
		qrPanel = new ADAGEQRPanel();
		splashPanel = new ADAGESplashPanel();
		registrationPanel = new ADAGERegisterPanel();
		popup = null;
		
		menuRect = new Rect(112,84,800,600);
		logoRect = new Rect(312,104,400,150);
		errorRect = new Rect(212,614,600,50);
		screenRect = new Rect(0,0,defaultResolution.x,defaultResolution.y);
		backgroundRect = overlayRect = screenRect;
		
		logo = Resources.Load("Images/GLS_Logo_Dark") as Texture2D;
		
		error = "";
		errorTime = 0f;
		
		loading = true;
	}

	void OnGUI()
	{		
		Vector3 scale = new Vector3(Screen.width/(defaultResolution.x * 1.0f), Screen.height/(defaultResolution.y * 1.0f), 1f);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);

		if(loading)
		{
			InitStyles();
			loading = false;
		}		
			
		GUI.skin = skin;

		if(currentPanel != null)
			screenRect = GUI.Window (0, screenRect, Draw, "");
	}
		
	void Draw(int windowID)
	{	
		if(backgroundImage != null)
			GUI.DrawTexture(backgroundRect, backgroundImage);
		
		if(overlay != null)
			GUI.DrawTexture(overlayRect, overlay);

		GUI.Box(menuRect, "");

		GUI.DrawTexture(logoRect, logo);

		if(GUI.Button(logoRect, "", invisibleButtonStyle))
			ADAGEMenu.ShowHome();

		GUI.enabled = (popup == null);
			currentPanel.Draw();
		GUI.enabled = true;

		GUI.Label(errorRect, instance.error, errorStyle);

		if(popup != null)
		{
			if(overlay != null)
				GUI.DrawTexture(overlayRect, overlay);

			popup.Draw();
		}
	}
	
	void Update()
	{		
		if(error.Trim() != "" && error != null)
		{
			errorTime += Time.deltaTime;
			
			if(errorTime >= errorDuration)
			{
				error = "";	
				errorTime = 0f;
			}
		}

		if(currentPanel != null)
			StartCoroutine(currentPanel.Update());	
	}
		
	void OnApplicationQuit()
	{
		currentPanel.OnApplicationQuit();
	}

	public void OnLevelWasLoaded(int level) 
	{
		if(Application.loadedLevelName != "ADAGELogin")
			gameObject.SetActive(false);
		else
			gameObject.SetActive(true);
	}

	private void OnGameStart()
	{
		//gameObject.SetActive(false);
	}

	private void OnLogin(int id, string token)
	{
		ShowSplash();
		//ShowHome();
	}

	private void OnConnectionTimeout(string connectionType)
	{
		popup = new ADAGEConnectionFailurePopup(ADAGEWebJob.maxAttempts);
	}

	private void InitStyles()
	{		
		invisibleButtonStyle = new GUIStyle(GUI.skin.GetStyle("button"));	
		invisibleButtonStyle.normal.background = null;
		invisibleButtonStyle.hover.background = null;
		invisibleButtonStyle.focused.background = null;
		invisibleButtonStyle.active.background = null;

		errorStyle = new GUIStyle(GUI.skin.GetStyle("label"));	
		errorStyle.fontSize = 22;
		errorStyle.fontStyle = FontStyle.BoldAndItalic;
		errorStyle.alignment = TextAnchor.MiddleLeft;
		errorStyle.normal.textColor = new Color(0.8f,0.2f,0.2f,1f);		
	}
	
	private void CheckResolution()
	{
		int screenHeight = Screen.height;
		int screenWidth = Screen.width;
		
		if(stretchBackgroundAspect)
			backgroundRect = new Rect(0,0,screenWidth,screenHeight);
		else if(backgroundImage != null)
		{
			float width = backgroundImage.width;
			float height = backgroundImage.height;
			
			float screenAspect = screenWidth / (screenHeight * 1.0f);
			float imageAspect = width / (height * 1.0f);
					
			if(screenAspect == imageAspect)
			{
				backgroundRect = new Rect(0,0,screenWidth,screenHeight);	
			}
			else
			{
				float multiplier;
				float diff;
				float modDimension;
				float modPosition;
								
				float widthRatio = width / (screenWidth * 1.0f);
				float heightRatio = height / (screenHeight * 1.0f);
				
				if(widthRatio < heightRatio)
				{
					//the heights are closer together
					multiplier = screenHeight / (height * 1.0f);		
					modDimension = width * multiplier;	
					diff = screenWidth - modDimension;
					modPosition = diff / 2.0f;	
					backgroundRect = new Rect(modPosition,0,modDimension,screenHeight);
				}
				else
				{
					multiplier = screenWidth / (width * 1.0f);	
					modDimension = height * multiplier;
					diff = screenHeight - modDimension;
					modPosition = diff / 2.0f;
					backgroundRect = new Rect(0,modPosition,screenWidth,modDimension);								
				}
			}
		}
			
		overlayRect = new Rect(0,0,screenWidth,screenHeight);
	}
}
