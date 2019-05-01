using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DamageType_Explode : DamageType_Base
{
	[Header("Explosion Properites")]
	public float m_explosionRadius;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			GameObject newContactVisual = ObjectPooler.instance.NewObject(m_contactVisual.gameObject, p_bulletRefrence.transform);

			newContactVisual.transform.position = p_collision.ClosestPoint(p_bulletRefrence.transform.position);

			Collider2D[] colliders = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_explosionRadius, p_damageTargetMask);

			foreach (Collider2D collider in colliders)
			{
				collider.GetComponent<Health>().TakeDamage(p_bulletRefrence.m_damage);
			}

			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}

	}
}
