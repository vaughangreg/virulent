using UnityEngine;
using System.Collections;

public class MessagePopulationChanged : Message {
	public MessagePopulationChanged(GameObject aSource) : base(aSource, Dispatcher.POPULATION_CHANGED)
	{
		base.Send();
	}
}

