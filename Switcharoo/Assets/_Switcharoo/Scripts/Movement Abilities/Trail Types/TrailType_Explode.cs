using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Explode")]
public class TrailType_Explode : TrailType_Base
{
	[Header("Explosion Properites")]
	public GameObject m_explosionVisual;
	public float m_explosionRadius;


    public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(ExplosionTrail(p_playerRefrence, p_movementType, p_damageTargetMask, p_obstacleMask));
	}

	IEnumerator ExplosionTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		float explosionInterval = p_movementType.m_movementTime / p_movementType.m_amountOfTrailsToSpawn;

		int amountOfExplosions = 0;

		while (amountOfExplosions < p_movementType.m_amountOfTrailsToSpawn)
		{
			Explode(p_playerRefrence.transform.position, p_damageTargetMask, p_playerRefrence);
			amountOfExplosions++;

			yield return new WaitForSeconds(explosionInterval);

			if (!p_playerRefrence.m_usingMovementAbility)
			{
				amountOfExplosions = (int)p_movementType.m_amountOfTrailsToSpawn;
			}
		}
	}

	private void Explode(Vector3 p_explosionOrigin, LayerMask p_damageTargetMask, PlayerController p_playerRefrence)
	{
		DebugExtension.DebugCircle(p_explosionOrigin, Vector3.forward, Color.yellow, m_explosionRadius, 0.1f);

		GameObject newObject = ObjectPooler.instance.NewObject(m_explosionVisual, p_explosionOrigin, Quaternion.identity);
		ParticleSystem newParticleSystem = newObject.GetComponent<ParticleSystem>();
		newParticleSystem.Play();
		ParticleSystem.ShapeModule shapeModule = newParticleSystem.shape;
		shapeModule.radius = m_explosionRadius - 1f;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(p_explosionOrigin, m_explosionRadius, p_damageTargetMask);

		foreach (Collider2D collider in colliders)
		{
			if (collider.tag == "Enemy")
			{
				if (collider.gameObject.GetComponent<AiController>().m_entityType == p_playerRefrence.m_currentRunnerType)
				{
					collider.GetComponent<Health>().TakeDamage(m_trailDamageAmount);
				}
			}
			else if (collider.tag == "EnemySpawner")
			{
				if (collider.gameObject.GetComponent<AI_Spawner>().m_spawnerType == p_playerRefrence.m_currentRunnerType)
				{
					collider.GetComponent<Health>().TakeDamage(m_trailDamageAmount);
				}
			}
		}
	}
}
