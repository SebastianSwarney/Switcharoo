using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Explode")]
public class DamageType_Explode : DamageType_Base
{
	[Header("Explosion Properites")]
	public float m_explosionRadius;
	public float m_explosionDamage;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		Explode(p_bulletRefrence, p_collision, p_obstacleMask, p_damageTargetMask);
	}

	private void Explode(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			p_collision.GetComponent<Health>().TakeDamage(m_damageAmount);
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			Collider2D[] colliders = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_explosionRadius, p_damageTargetMask);

			DebugExtension.DebugCircle(p_collision.ClosestPoint(p_bulletRefrence.transform.position), Vector3.forward, Color.yellow, m_explosionRadius, 0.1f);

			foreach (Collider2D collider in colliders)
			{
				collider.GetComponent<Health>().TakeDamage(m_explosionDamage);
			}

			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}
}
