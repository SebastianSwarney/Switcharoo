using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff Types/Push")]
public class PlayerBuff_Push : PlayerBuff_Base
{
	public float m_pushForce;
	public float m_pushRadius;

	public override void UseBuff(PlayerController p_playerRefrence, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		Push(p_playerRefrence, p_damageTargetMask, p_obstacleMask);
	}

	private void Push(PlayerController p_playerRefrence, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(p_playerRefrence.transform.position, m_pushRadius, p_damageTargetMask);

		foreach (Collider2D collider in colliders)
		{
			Vector2 shootDir = collider.transform.position - p_playerRefrence.transform.position;

			collider.attachedRigidbody.AddForce(shootDir.normalized * m_pushForce, ForceMode2D.Impulse);
		}
	}
}
