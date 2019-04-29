using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet Contanct Behaviours/Standard")]
public class BulletContactBehaviour_Standard : BulletContactBehaviour_Base
{
	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			p_collision.gameObject.GetComponent<Health>().TakeDamage(p_bulletRefrence.m_damage);
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision))
		{
			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}
}
