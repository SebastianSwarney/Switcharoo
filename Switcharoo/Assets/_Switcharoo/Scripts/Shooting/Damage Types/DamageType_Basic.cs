using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Basic")]
public class DamageType_Basic : DamageType_Base
{
	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		BasicDamage(p_bulletRefrence, p_collision, p_obstacleMask, p_damageTargetMask);
	}

	private void BasicDamage(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			p_collision.GetComponent<Health>().TakeDamage(m_damageAmount);
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}
}
