using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Explode")]
public class DamageType_Explode : DamageType_Base
{
	[Header("Explosion Properites")]
	public GameObject m_explosionVisual;
	public float m_explosionRadius;
	public float m_explosionDamage;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, float p_damageBase, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		Explode(p_bulletRefrence, p_collision, p_damageBase, p_obstacleMask, p_damageTargetMask);
	}

	private void Explode(Bullet_Base p_bulletRefrence, Collider2D p_collision, float p_damageBase, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			p_collision.GetComponent<Health>().TakeDamage(p_damageBase);
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			Collider2D[] colliders = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_explosionRadius, p_damageTargetMask);

			DebugExtension.DebugCircle(p_collision.ClosestPoint(p_bulletRefrence.transform.position), Vector3.forward, Color.yellow, m_explosionRadius, 0.1f);

			GameObject newObject = ObjectPooler.instance.NewObject(m_explosionVisual, p_collision.ClosestPoint(p_bulletRefrence.transform.position), Quaternion.identity);
			ParticleSystem newParticleSystem = newObject.GetComponent<ParticleSystem>();
			newParticleSystem.Play();
			ParticleSystem.ShapeModule shapeModule = newParticleSystem.shape;
			shapeModule.radius = m_explosionRadius - 1f;

			foreach (Collider2D collider in colliders)
			{
				if (collider.tag == "Enemy")
				{
					if (collider.gameObject.GetComponent<AiController>().m_entityType == p_bulletRefrence.m_type)
					{
						collider.GetComponent<Health>().TakeDamage(m_explosionDamage);
					}
					else
					{
						collider.gameObject.GetComponent<AiController>().BulletResitant();
					}
				}
				else if (collider.tag == "Player")
				{
					collider.GetComponent<Health>().TakeDamage(m_explosionDamage);
				}
				else if (collider.tag == "EnemySpawner")
				{
					if (collider.gameObject.GetComponent<AI_Spawner>().m_spawnerType == p_bulletRefrence.m_type)
					{
						collider.GetComponent<Health>().TakeDamage(m_explosionDamage);
					}
					else
					{
						collider.gameObject.GetComponent<AI_Spawner>().BulletResitant();
					}
				}
			}

			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}
}
