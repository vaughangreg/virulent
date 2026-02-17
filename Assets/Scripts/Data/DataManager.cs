using UnityEngine;
using System.IO;

// NOTE:
// Comment out the #if statements below
// to enable collection in the Unity Editor.

/// <summary>
/// Records player information and makes it available as a file.
/// Note: On IOS, this requires changing the Info.plist file to
/// have the Boolean key UIFileSharingEnabled set to 'true'. In
/// order to avoid adding this every build, change it, and then
/// use 'Append' instead of 'Replace' when building for IOS.
/// </summary>
public class DataManager : MonoBehaviour {
	public const string EXTENSION = ".data";
	public       string guid;
	
	/// <summary>
	/// Should be sync'd with the Dispatcher types we don't care about.
	/// </summary>
	public string[] ignoredEventTypes = {
		// Happens every frame; don't care.
		Dispatcher.RENDER_POST,
	};
	
	private const string FORMAT_DATE = "yyyy-MM-dd HH_mm_ss";
	private const string FORMAT_TIME = "HH:mm:ss.fffffff";
	private TextWriter m_out;
	
	/// <summary>
	/// The current formatted date.
	/// </summary>
	public string dateNow {
		get {
			System.DateTime time = System.DateTime.Now;
			return time.ToString(FORMAT_DATE);
		}
	}
	
	/// <summary>
	/// The current formatted time.
	/// </summary>
	public string timeNow {
		get {
			System.DateTime time = System.DateTime.Now;
			return time.ToString(FORMAT_TIME);
		}
	}
	
	public const string KEY_SESSION_ID = "SessionId";
	#region Initialization/Destruction
	/// <summary>
	/// Register ourself as the dispatcher data component.
	/// </summary>
	public void Awake() {
		Random.seed = (int)System.DateTime.UtcNow.Ticks;
		
		guid = PlayerPrefs.GetString(KEY_SESSION_ID, "GUID NOT SET"); //"00000000-0000-0000-0000-000000000000");
		
		CreateDataPaths();
		Dispatcher.data = this;
	}
	
	/// <summary>
	/// Called when the scene changes or the app quits.
	/// </summary>
	public void OnDestroy() {
		CloseFile();	
	}
	
	public void OnApplicationQuit() {
		CloseFile();
	}
	
	/// <summary>
	/// Close the data file.
	/// </summary>
	public void CloseFile() {
		if (m_out != null) {
			m_out.Close();
			Dispatcher.data = null;
			m_out = null;
		}
	}
	
	public string dataPath;
	/// <summary>
	/// Creates a data path appropriate to the platform.
	/// </summary>
	protected void CreateDataPaths() {
#if UNITY_EDITOR
		//Debug.Log("********** NO DATA BEING COLLECTED **********");
		return;	
#else	
		if (Debug.isDebugBuild) {
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				// Note: This relies on the document path not changing
				// on Apple's side of things.
				dataPath = Application.dataPath + "/../../Documents";	
			}
			else dataPath = Application.dataPath;
			
			string filePath = System.String.Format("{0}/{4}/Virulent_{1} {2}{3}", dataPath, 
			                                       Application.loadedLevelName, 
			                                       dateNow, 
			                                       EXTENSION, guid);
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			m_out = new StreamWriter(filePath);
			m_out.WriteLine("# Session ID: " + guid);
			m_out.WriteLine("# Time\tSource\tInstance ID\tPosition\tMessage Type\tMessage Contents");
			Debug.Log("********** Collecting data for " + guid + " **********");
			// Debug.Log("Opened data path at " + filePath);
		}
#endif
	}
	#endregion
	
	/// <summary>
	/// Records a message.
	/// </summary>
	/// <param name="aTarget">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="aMessage">
	/// A <see cref="Message"/>
	/// </param>
	public void Record(Message aMessage) {
#if UNITY_EDITOR
		return;	
#else
		if (!Debug.isDebugBuild) return;
		foreach (string anIgnoredType in ignoredEventTypes) {
			if (anIgnoredType == aMessage.listenerType) return;	
		}
		string source;
		int sourceId;
		
		if (aMessage.source == null) {
			source = "(null)";
			sourceId = -1;
		}
		else {
			source = aMessage.source.name;
			sourceId = aMessage.source.GetInstanceID();
		}
		
		string sourcePosition = (aMessage.source == null) 
			? "(null)" : aMessage.source.transform.position.ToString();

		m_out.WriteLine(System.String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", 
		                                     timeNow, source, sourceId,
		                                     sourcePosition,
		                                     aMessage.listenerType, 
		                                     aMessage.ToArgumentString()));
#endif
	}
}
