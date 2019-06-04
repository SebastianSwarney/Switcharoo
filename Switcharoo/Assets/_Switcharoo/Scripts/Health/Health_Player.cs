using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Player : Health
{
	[Header("Player Hit Properites")]
	public float m_invulnerableTime;
	public float m_movementControllLossTime;

	private PlayerController m_player;

	public override void Start()
	{
		base.Start();
		m_player = GetComponent<PlayerController>();
	}

	public override void TakeDamage(float p_damage)
	{
		base.TakeDamage(p_damage);
		if (m_canTakeDamage)
		{
			m_player.m_playerHurt.Invoke();
		}
	}

	public override void Die()
	{
		m_isDead = true;
	}
}
