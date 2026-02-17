using UnityEngine;
using System.Collections;

/// <summary>
/// Message that handles SFX and Music Volume; DO NOT use for fading Music
/// </summary>
public class MessageSetVolume : Message
{
	public float volume;
	public MusicManager.VolumeType volumeType;
	
	public MessageSetVolume(GameObject aSource, float aVolume, MusicManager.VolumeType aVolumeType) 
		: base(aSource, Dispatcher.SET_VOLUME) 
	{
		volume = Mathf.Clamp01(aVolume);
		volumeType = aVolumeType;
		base.Send();
	}
	
	public override string ToString() { return System.String.Format("Set volume {0}: {1}", volumeType, volume); }
}

