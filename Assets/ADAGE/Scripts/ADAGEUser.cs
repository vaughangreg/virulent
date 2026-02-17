using UnityEngine;
using System;
using System.Collections;
using LitJson;
using System.IO;

public class ADAGEUser 
{
	public ADAGEUser(){
		playerName = "EMPTY";
		username = "EMPTY";
		email = "EMPTY@EMPTY.COM";
		adageId = "EMPTY";
		adageAccessToken = "EMPTY";
		adageRefreshToken = "EMPTY";
		fbAccessToken = "EMPTY";
		fbExpiresAt = new DateTime();
		adageExpiresAt = new DateTime();
		guest = false;
		dataWrapper = new UploadWrapper();
		localWrapper = new UploadWrapper();
		pContext = new ADAGEPositionalContext();
		vContext = new ADAGEVirtualContext(Application.loadedLevelName);
	}
	public string playerName {get; set;} //what to display on the UI
	public string username {get; set; } //what the unique user name is. With FB username can be different from playerName
	public string email {get; set;}
	public string adageId {get; set;}
    public string adageAccessToken { get; set; }
	public string adageRefreshToken { get; set; }
	public string fbAccessToken {get; set; }
	public string facebookId {get; set;}
	public bool guest {get; set;}
	
	
	public DateTime fbExpiresAt {get; set; }
	public DateTime adageExpiresAt {get; set; }

	[SkipSerialization]
	//data buffers for this user
	public UploadWrapper		   dataWrapper;
	[SkipSerialization]
	public UploadWrapper 		   localWrapper;

	[SkipSerialization]
	public ADAGEPositionalContext pContext;
	[SkipSerialization]
	public ADAGEVirtualContext    vContext;

	
	//Is this a valid user
	public bool valid()
	{
		return !playerName.Equals("EMPTY") && !adageAccessToken.Equals("EMPTY");	
	}
	
	public void Clear()
	{
		playerName = "EMPTY";
		username = "EMPTY";
		email = "EMPTY@EMPTY.COM";
		adageId = "EMPTY";
		adageAccessToken = "EMPTY";
		adageRefreshToken = "EMPTY";
		fbAccessToken = "EMPTY";
		fbExpiresAt = new DateTime();
		adageExpiresAt = new DateTime();
		guest = false;
	}

	public void ClearLocalData()
	{
		localWrapper.Clear();
	}

	public void ClearUploadData()
	{
		dataWrapper.Clear();
	}

}
