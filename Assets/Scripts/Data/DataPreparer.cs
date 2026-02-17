using UnityEngine;

public class DataPreparer : MonoBehaviour {
	public bool shouldCreateGuid = false;
	
	private const string TIMES_RAN = "TimesRan";
	private const string ID_RANDOM = "IdRandom";
	private const int    HASH_LENGTH = 6;
	private const string ID_FORMAT = "{0}-{1:x3}{2:00}"; // Hash-rnd-timesran
	
	void Awake () {
#if UNITY_IPHONE
		// Always increment the times ran.
		int timesRan = PlayerPrefs.GetInt(TIMES_RAN, 0) + 1;
		PlayerPrefs.SetInt(TIMES_RAN, timesRan);
		
		// Only generate the random value ONCE; this
		// should help ensure that we can distinguish
		// between IDs when the software is reinstalled.
		int idRandom = PlayerPrefs.GetInt(ID_RANDOM, 0);
		if (idRandom == 0) {
			idRandom = Random.Range(0, 4095);	// 3 hex digits
			PlayerPrefs.SetInt(ID_RANDOM, idRandom);
		}
		
		string hash = Md5Sum(iPhoneSettings.uniqueIdentifier);
		PlayerPrefs.SetString(DataManager.KEY_SESSION_ID, string.Format(ID_FORMAT, hash.Substring(0, HASH_LENGTH), idRandom, timesRan));
#endif
	}
	
	public string Md5Sum(string strToEncrypt) {
	    System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
	    byte[] bytes = ue.GetBytes(strToEncrypt);
	
	    // encrypt bytes
	    System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
	    byte[] hashBytes = md5.ComputeHash(bytes);
	
	    // Convert the encrypted bytes back to a string (base 16)
	    string hashString = "";
	
	    for (int i = 0; i < hashBytes.Length; i++) {
	        hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
	    }
	
	    return hashString.PadLeft(32, '0');
	}
}
