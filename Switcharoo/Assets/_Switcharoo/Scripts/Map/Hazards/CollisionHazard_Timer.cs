using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ActivateEvent : UnityEvent { }

public class CollisionHazard_Timer : CollisionHazard_Base
{
	[Header("Timer Hazard Properties")]
	public float m_timeOn;
	public float m_timeOff;
	public float m_startDelay;

	public ActivateEvent m_onEvent;
	public ActivateEvent m_offEvent;

	private bool m_isOn;
	private float m_timer;
	private bool m_isStarted;

	public override void Start()
	{
		base.Start();

		StartCoroutine(StartDelay());
	}

	private void Update()
	{
        if (m_paused) return;
        HazardTimer();
	}

	IEnumerator StartDelay()
	{
		m_isStarted = false;
		float t = 0f;

		while (t < m_startDelay)
		{
			t += Time.deltaTime;

			yield return null;
		}
		m_isStarted = true;
	}

	private void HazardTimer()
	{
		if (m_isStarted)
		{
			if (m_isOn)
			{
				TurnOn();

				m_damageTimer = m_damageInterval;
				m_timer += Time.deltaTime;

				if (m_timer >= m_timeOn)
				{
					m_timer = 0f;
					m_isOn = false;
				}
			}

			if (!m_isOn)
			{
				TurnOff();

				m_timer += Time.deltaTime;

				if (m_timer >= m_timeOff)
				{
					m_timer = 0f;
					m_isOn = true;
				}
			}
		}
	}

	private void TurnOff()
	{
		m_offEvent.Invoke();
		//m_renderer.color = Color.clear;
		m_canDamage = false;
	}

	private void TurnOn()
	{
		m_onEvent.Invoke();
		//m_renderer.color = Color.white;
		m_canDamage = true;
	}
}
