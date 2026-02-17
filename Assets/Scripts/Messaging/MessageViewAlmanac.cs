using UnityEngine;
using System.Collections;

public class MessageViewAlmanac : Message
{
	public Almanac.Views viewToShow;
	public string itemToView = null;
	
	public MessageViewAlmanac(GameObject aSource, Almanac.Views aViewToShow) : base(aSource,Dispatcher.VIEW_ALMANAC)
	{
		viewToShow = aViewToShow;
		base.Send();
	}
	public MessageViewAlmanac(GameObject aSource, string anItemToView) : base(aSource,Dispatcher.VIEW_ALMANAC)
	{
		itemToView = anItemToView;
		base.Send();
	}
}

