using UnityEngine;
using System.Collections;

public class MessageBuddingSiteReady : Message
{
	public BuddingSite site;
	public MessageBuddingSiteReady(GameObject aSource, BuddingSite sourceSite) : base(aSource, Dispatcher.BUDDING_SITE_READY)
	{
		//Debug.Log("Budding Site Ready message sent");
		site = sourceSite;
		base.Send();
	}
}

