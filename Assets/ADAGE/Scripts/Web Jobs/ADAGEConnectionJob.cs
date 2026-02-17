using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using LitJson;

public abstract class ADAGEWebJob : WebJob
{
	public static readonly int maxAttempts = 3;

	public int status = 0;

	public ADAGEResponse response;

	public string endPoint = "";
	public string httpMethod;

	public int numAttempts = 0;

	public int localId;

	public ADAGEWebJob(){}

	public ADAGEWebJob(string endPoint, string httpMethod = "POST", int id=0)
	{
		if(Application.isEditor || Debug.isDebugBuild)
		{
			if(ADAGE.Staging)
			{
				this.url = ADAGE.stagingURL;	
			}
			else
			{
				this.url = ADAGE.developmentURL;	
			}
		}
		else
		{
			this.url = ADAGE.productionURL;	
		}

#if (UNITY_EDITOR)

		if(ADAGE.ForceProduction)
			this.url = ADAGE.productionURL;	
		
		if(ADAGE.ForceDevelopment)
			this.url = ADAGE.developmentURL;	

		if(ADAGE.ForceStaging)
			this.url = ADAGE.stagingURL;	

#endif

		this.endPoint = endPoint;
		this.httpMethod = httpMethod;
		this.localId = id;
	}
	  
	public override void Main(WorkerPool boss = null)
	{
		request = new HTTP.Request (this.httpMethod, this.url + this.endPoint);

		AddParameters();

		SendRequest();

		if(!CheckErrors())
			HandleResponse();

		if(boss != null)
			boss.CompleteJob(this);
	}

	protected virtual void AddParameters(){}

	private void SendRequest()
	{
		try
		{
			request.Send();
			status = request.response.status;
		}
		catch(Exception e)
		{
			Debug.Log ("There was an ADAGE Error: " + e.Message.ToString());
			status = 404;
		}
	}

	private bool CheckErrors()
	{
		if(status < 200 || status > 299)
		{
			if(status != 0)
			{
				try
				{
					/*if(!request.response.Text.Contains("["))
						throw new Exception("No error list present");*/

					if(request.response.Text == null || request.response.Text.Trim() == "")
					{
						response = new ADAGEErrorResponse(string.Format ("ADAGE returned a status of {0} with no response text for object {1}.", status, GetType()));
					}
					else
					{
						response = JsonMapper.ToObject<ADAGEErrorResponse>(request.response.Text);
					}
				}
				catch(Exception e)
				{
					string error = string.Format("ADAGE is offline due to {0} causing an error ('{1}') that could not be mapped to ADAGEErrorResponse: '{2}'", GetType(), e.Message, request.response.Text);
					response = new ADAGEErrorResponse(error);
				}
			}
			else
			{
				response = new ADAGEHostError("Could not resolve ADAGE host");
			}

			return true;
		}

		return false;
	}

	protected virtual void HandleResponse()
	{
		response = new ADAGEResponse(request.response.Text);
	}

	public override string ToString ()
	{
		return string.Format ("[{0}]\nResponse: {1}\nStatus:{2}", GetType(), response, status);
	}
}

public abstract class ADAGEGetRequestJob : ADAGEWebJob
{
	public ADAGEGetRequestJob(string endPoint, int localId=0) : base(endPoint, "GET", localId){}

	protected override void AddParameters()
	{
		base.AddParameters();
		
		request.AddHeader("Content-Type", "application/jsonrequest");
	}
}

public abstract class ADAGEPostRequestJob : ADAGEWebJob
{
	public ADAGEPostRequestJob(string endPoint, int localId) : base(endPoint, "POST", localId){}

	protected override void AddParameters()
	{
		base.AddParameters();

		request.AddHeader ("Content-Type", "application/x-www-form-urlencoded");
	}
}

//This seems like a duplicate, but it isn't
public abstract class ADAGEUploadRequestJob : ADAGEWebJob
{
	public ADAGEUploadRequestJob(string endPoint, int localId) : base(endPoint, "POST", localId){}
	
	protected override void AddParameters()
	{
		base.AddParameters();
		
		request.AddHeader ("Content-Type", "application/jsonrequest");
	}
}

/**** For Web Jobs that involve game specific data to ADAGE ****/
public abstract class ADAGEClientJob : ADAGEPostRequestJob
{
	protected string clientId = "";
	protected string clientSecret = "";

	public ADAGEClientJob(string clientId, string clientSecret, string endPoint, int localId) : base(endPoint, localId)
	{
		this.clientId = clientId;
		this.clientSecret = clientSecret;
	}

	protected override void AddParameters()
	{
		base.AddParameters();
		
		request.AddParameter("client_id", this.clientId);
		request.AddParameter("client_secret", this.clientSecret);
	}
}

/**** For Web Jobs that involve connecting to ADAGE (Login, Register, Guest, QR) ****/
public abstract class ADAGEConnectionJob : ADAGEClientJob
{
	protected string email = "";
	protected string password = "";
	
	public ADAGEConnectionJob(string email, string password, string clientId, string clientSecret, string endPoint, int localId) : base(clientId, clientSecret, endPoint, localId)
	{
		this.email = email;
		this.password = password;
	}

	protected override void AddParameters()
	{
		base.AddParameters();
		
		request.AddParameter("email", this.email);
		request.AddParameter("password", this.password);
	}
	
	protected override void HandleResponse()
	{
		response = JsonMapper.ToObject<ADAGEAccessTokenResponse>(request.response.Text);
	}
}

public class ADAGELoginJob : ADAGEConnectionJob
{			
	public ADAGELoginJob(string clientId, string clientSecret, string email, string password, int localId) : base(email, password, clientId, clientSecret, "/auth/authorize_unity", localId){}
	
	protected override void AddParameters()
	{
		base.AddParameters();
		
		request.AddParameter("grant_type", "password");
	}
}

public class ADAGERegistrationJob : ADAGEConnectionJob
{	
	private string player_name;
	private string password_confirm;
			
	public ADAGERegistrationJob(string clientId, string clientSecret, string player_name, string email, string password, string password_confirm, int localId) : base(email, password, clientId, clientSecret, "/auth/register", localId)
	{
		this.player_name = player_name;
		this.password_confirm = password_confirm;
	}

	protected override void AddParameters()
	{
		base.AddParameters();
		
		request.AddParameter("player_name", this.player_name);
		request.AddParameter("password_confirm", this.password_confirm);
		request.AddParameter("grant_type", "password");
	}
}

public class ADAGEGuestConnectionJob : ADAGEClientJob
{	
	private string group;

	public ADAGEGuestConnectionJob(string clientId, string clientSecret, int localId, string group = "") : base(clientId, clientSecret, "/auth/guest", localId)
	{
		this.group = group.Trim();
	}

	protected override void AddParameters()
	{
		base.AddParameters();

		if(group != "")
			request.AddParameter("group", this.group);
	}
	
	protected override void HandleResponse()
	{
		response = JsonMapper.ToObject<ADAGEAccessTokenResponse>(request.response.Text);
	}
}

public class ADAGERequestUserJob : ADAGEGetRequestJob
{	
	public bool onCompleteLoad = false;
	private string access_token; 
		
	public ADAGERequestUserJob(string access_token, bool loadOnComplete, int localId) : base("/auth/adage_user.json", localId)
	{
		this.access_token = access_token;
		this.onCompleteLoad = loadOnComplete;
	}
	
	protected override void AddParameters()
	{
		base.AddParameters();

		request.AddHeader("Authorization", "Bearer " + access_token);
	}

	protected override void HandleResponse()
	{
		response = JsonMapper.ToObject<ADAGEUserResponse>(request.response.Text);
	}
}

public class ADAGEFacebookConnectionJob: WebJob
{	
	public int status;
	public string response = "Nothing";
	
	private string clientId = "";
	private string clientSecret = "";
	
	private string cookie;
			
	public ADAGEFacebookConnectionJob(string clientId, string clientSecret, string cookie)
	{
		this.clientId = clientId;
		this.clientSecret = clientSecret;
		this.cookie = cookie;
		
		
		if(Application.isEditor || Debug.isDebugBuild)
		{
			if(ADAGE.Staging)
			{
				this.url = ADAGE.stagingURL;	
			}
			else
			{
				this.url = ADAGE.developmentURL;	
			}
		}
		else
		{
			this.url = ADAGE.productionURL;	
		}		
	}
	
	public override void Main(WorkerPool boss = null) 
	{	
		request = new HTTP.Request ("POST", this.url + "/auth/authorize_unity_fb");
			
		request.AddParameter("client_id", this.clientId);
		request.AddParameter("client_secret", this.clientSecret);
		request.AddParameter("grant_type", "fakebook");
		//request.AddParameter("omniauth", cookie);
		request.SetText(cookie);

			

		// Add request headers
		//request.AddHeader ("Content-Type", "application/x-www-form-urlencoded");
		request.AddHeader("Content-Type", "application/jsonrequest");
		//request.AddHeader("Content-Type", "application/json");
		
		Debug.Log(request.uri);
		// Send request
		request.Send ();
		
			
	
			// Dump request response to debug console
			
		status = request.response.status;
		response = request.response.Text;
		Debug.Log ("RESPONSE ***********************************: " + response);
			
		


				
				
		if(boss != null)
			boss.CompleteJob(this);
    }

}