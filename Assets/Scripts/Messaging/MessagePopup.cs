using UnityEngine;
using System.Collections;

public class MessagePopup : Message
{
	public PopupManager.PresetIDs popupPreset;
	public float life;
	
	public MessagePopup(GameObject aSource, PopupManager.PresetIDs popupPreset, float life) : base(aSource,Dispatcher.POPUP) 
	{
		this.popupPreset = popupPreset;
		this.life = life;
		base.Send();
	}
}

