using UnityEngine;
using System.IO;
using System.Net;
using System.Text;

//using RestSharp;
//using RestSharp.Deserializers;

/*
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Runtime.InteropServices;
*/

public class RecognizerEmailData : IRecognizer {
	protected const string FTP_SERVER = "terrordome.discovery.wisc.edu";
	protected const string FTP_USER = "ftpuser";
	protected const string FTP_PASSWORD = "frufruH7";
	
	protected bool m_didRecognize = false;
	protected int  m_filesUploaded = 0;

	/// <summary>
	/// Returns true if the recognizer fired.
	/// </summary>
	/// <returns>
	/// True if recognized; false otherwise.
	/// </returns>
	/// <param name='isTouching'>
	/// Finger/mouse state.
	/// </param>
	/// <param name='atPosition'>
	/// Finger/mouse position.
	/// </param>
	public bool ProcessState(bool isTouching, Vector3 atPosition) {
		if (m_didRecognize) return false;

		else if (Input.touchCount == 2 && Input.acceleration.magnitude > 1.5f) {
			SendData();
			m_didRecognize = true;
		}
		/*
		else if (Input.GetMouseButton(1)) {
			SendData();
			m_didRecognize = true;
		}
		*/

		return false;	// Return false because we don't want to call a handler.
	}
	
	/// <summary>
	/// Gets or sets the sensitivity of the recognizer.
	/// </summary>
	/// <value>
	/// The epsilon in screen pixels.
	/// </value>
	public float epsilon { 
		get { return 0.0f; } 
		set { }
	}
	
	/// <summary>
	/// Where the initial touch was recorded.
	/// </summary>
	public Vector3 touchDownPosition { 
		get { return Vector3.zero; }
	}
	
	#region FTP
	protected void SendData() {
		DataManager theDataManager = Camera.main.GetComponent<DataManager>();
		if (!theDataManager) {
			Debug.LogError("No data manager!");
			return;
		}
		theDataManager.CloseFile();
		
		FTPLib.FTP ftp = new FTPLib.FTP(FTP_SERVER, FTP_USER, FTP_PASSWORD);
		
		Debug.Log("=== Sending Data ===");
		try {
			new MessageCheckPoint(null, "Sending data...", null);
			string[] theDirectories = Directory.GetDirectories(theDataManager.dataPath);
			foreach (string aDirectory in theDirectories) {
				string[] theFiles = Directory.GetFiles(aDirectory);
				foreach (string aFile in theFiles) {
					Debug.Log("Checking file " + aFile);
					if (aFile.EndsWith(DataManager.EXTENSION)) {
						string prefix = Path.GetDirectoryName(aFile);
						prefix = Path.GetFileName(prefix);
						
						SendFile(prefix, ftp, aFile);
					}
				}
			}
			new MessageCheckPoint(null, 
			                      System.String.Format("{0} file{1} uploaded.", m_filesUploaded, m_filesUploaded == 1 ? "" : "s"),
			                      null);
		}
		catch (System.Exception e) {
			new MessageCheckPoint(null, e.Message, null);
		}
		finally {
			ftp.Disconnect();
		}
	}
	
	protected void SendFile(string aPrefix,  FTPLib.FTP aFtp, string aFileName) {
		string targetFileName =  aPrefix + "_" + Path.GetFileName(aFileName);
		
		Debug.Log("Upload of " + aFileName + " starting.");
		aFtp.OpenUpload(aFileName, targetFileName);
		while (aFtp.DoUpload() > 0) {
			int perc = Mathf.FloorToInt(((aFtp.BytesTotal) * 100) / aFtp.FileSize);
			Debug.Log(System.String.Format("Upload: {0}/{1} {2}%", aFtp.BytesTotal, aFtp.FileSize, perc));
		}
		
		File.Delete(aFileName);
		++m_filesUploaded;
		Debug.Log("Wrote " + targetFileName);
	}
	#endregion
}
