using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Timer : CollisionHazard_Base
{
	[Header("Timer Hazard Properties")]
	public float m_activeInterval;

	private float m_timer;

	private void Update()
	{
		HazardTimer();
	}

	private void HazardTimer()
	{
		m_timer = Mathf.PingPong(Time.time, m_activeInterval);

		if (m_timer > m_activeInterval / 2)
		{
			m_renderer.color = Color.white;
			m_canDamage = true;
		}
		else if (m_timer < m_activeInterval / 2)
		{
			m_renderer.color = Color.clear;
			m_canDamage = false;
		}
	}
}
