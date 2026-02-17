using UnityEngine;
using System;
using System.Collections.Generic;
using LitJson;
using Ionic.Zlib;

public class ADAGEUploadFileJob : ADAGEUploadRequestJob
{
	private string accessToken;
	private string data;

	public string Path{ get; protected set; }
		
	public ADAGEUploadFileJob(string token, string filepath, string text) : base("/data_collector.json", 0)
	{
		this.Path = filepath;
		this.data = text;
		this.accessToken = token;
	}
	
	protected override void AddParameters()
	{
		base.AddParameters();

		request.AddHeader("Authorization", "Bearer " + accessToken);
		request.SetText(data);
	}
}

public class ADAGEUploadJob : ADAGEUploadRequestJob
{
	private UploadWrapper data;

	public string accessToken;
		
	public ADAGEUploadJob(UploadWrapper data, string token, int localId) : base("/data_collector.json", localId)
	{
		//this.data = data;	
		this.data = new UploadWrapper(data);
		this.accessToken = token;	
	}

	protected override void AddParameters()
	{
		base.AddParameters();
		
		request.AddHeader("Authorization", "Bearer " + accessToken);

		string json = JsonMapper.ToJson(this.data);
		request.SetText(json);
	}

	public UploadWrapper GetData()
	{
		return data;	
	}
}