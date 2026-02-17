using UnityEngine;
using System.Collections;

public class MessageSetSelectableLayers : Message
{
	public int layers;
	
	public MessageSetSelectableLayers(int aLayers) : base(null, Dispatcher.SET_SELECTABLE_LAYERS)
	{
		layers = aLayers;
		base.Send();
	}
}

