using UnityEngine;
using System.Collections;

public class MessageEnergyReceived : Message
{
	public EnergyContainer energyContainer;
	
	public MessageEnergyReceived(GameObject aSource, EnergyContainer container) : base(aSource, Dispatcher.ENERGY_RECEIVED)
	{
		energyContainer = container;
		base.Send();
	}
	
}

