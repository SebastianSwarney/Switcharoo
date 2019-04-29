using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet Contanct Behaviours/Explode")]
public class BulletContactBehaviour_Explode : BulletContactBehaviour_Base
{
	[Header("Explosion Properites")]
	public float m_explosionRadius;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			Collider2D[] colliders = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_explosionRadius, p_damageTargetMask);

			foreach (Collider2D collider in colliders)
			{
				collider.GetComponent<Health>().TakeDamage(p_bulletRefrence.m_damage);
			}
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision))
		{
			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}
}
