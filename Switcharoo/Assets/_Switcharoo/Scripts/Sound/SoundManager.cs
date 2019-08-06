using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;

	public Slider m_masterSlider;
	public Slider m_musicSlider;
	public Slider m_sfxSlider;

	private Bus m_masterBus;
	private Bus m_musicBus;
	private Bus m_sfxBus;

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

		m_masterBus = RuntimeManager.GetBus("bus:/Master");
		m_musicBus = RuntimeManager.GetBus("bus:/Master/Music");
		m_sfxBus = RuntimeManager.GetBus("bus:/Master/SFX");
	}

	private void Update()
	{
		SetBusVolume(m_masterBus, m_masterSlider.value);
		SetBusVolume(m_musicBus, m_musicSlider.value);
		SetBusVolume(m_sfxBus, m_sfxSlider.value);
	}

	public void SetBusVolume(Bus p_bus, float p_volume)
	{
		p_bus.setVolume(p_volume);
	}
}
