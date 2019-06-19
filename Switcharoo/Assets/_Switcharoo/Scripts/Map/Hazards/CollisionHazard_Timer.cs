using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Timer : CollisionHazard_Base
{
	[Header("Timer Hazard Properties")]
	public float m_timeOn;
	public float m_timeOff;

	private bool m_isOn;
	private float m_timer;

	private void Update()
	{
        if (m_paused) return;
        HazardTimer();
	}

	private void HazardTimer()
	{
		if (m_isOn)
		{
			m_renderer.color = Color.white;
			m_damageTimer = m_damageInterval;
			m_canDamage = true;

			m_timer += Time.deltaTime;

			if (m_timer >= m_timeOn)
			{
				m_timer = 0f;
				m_isOn = false;
			}
		}

		if (!m_isOn)
		{
			m_renderer.color = Color.clear;
			m_canDamage = false;

			m_timer += Time.deltaTime;

			if (m_timer >= m_timeOff)
			{
				m_timer = 0f;
				m_isOn = true;
			}
		}
	}
}
