using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff Types/Invulnerable")]
public class PlayerBuff_Invulnerable : PlayerBuff_Base
{
	public float m_invulnerableTime;

	public override void UseBuff(PlayerController p_playerRefrence, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(Invulnerable(p_playerRefrence));
	}

	private IEnumerator Invulnerable(PlayerController p_playerRefrence)
	{
		Debug.Log("started");

		p_playerRefrence.m_states.m_damageState = PlayerController.DamageState.Invulnerable;

		float t = 0;

		while (t < m_invulnerableTime)
		{
			t += Time.deltaTime;

			yield return null;
		}

		p_playerRefrence.m_states.m_damageState = PlayerController.DamageState.Vulnerable;
	}
}
