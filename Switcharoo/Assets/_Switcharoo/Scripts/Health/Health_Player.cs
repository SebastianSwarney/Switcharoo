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
			StartCoroutine(PlayerHit());
		}
	}

	IEnumerator PlayerHit()
	{
		float t = 0;

		m_player.m_states.m_damageState = PlayerController.DamageState.Invulnerable;
		m_player.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;
		m_player.m_velocity = Vector3.zero;

		while (t < m_invulnerableTime)
		{
			t += Time.deltaTime;

			if (t >= m_movementControllLossTime)
			{
				m_player.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
			}

			yield return null;
		}

		m_player.m_states.m_damageState = PlayerController.DamageState.Vulnerable;
	}

	public override void Die()
	{
		m_isDead = true;
	}
}
