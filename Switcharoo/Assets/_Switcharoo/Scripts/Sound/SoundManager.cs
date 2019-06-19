using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;

	[Range(0f, 1f)]
	public float m_masterVolume = 1f;

	private Bus m_masterBus;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}

		//m_masterBus = RuntimeManager.GetBus("bus:/Master");
	}

	public void SetBusVolume(Bus p_bus, float p_volume)
	{
		p_bus.setVolume(p_volume);
	}


}
