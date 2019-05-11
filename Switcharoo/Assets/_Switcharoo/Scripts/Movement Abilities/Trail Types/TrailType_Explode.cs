using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Explode")]
public class TrailType_Explode : TrailType_Base
{
	[Header("Explosion Properites")]
	public float m_explosionRadius;

	public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		p_playerRefrence.StartCoroutine(ExplosionTrail(p_playerRefrence, p_movementType));
	}

	IEnumerator ExplosionTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		float explosionInterval = p_movementType.m_movementTime / p_movementType.m_amountOfTrailsToSpawn;

		int amountOfExplosions = 0;

		while (amountOfExplosions < p_movementType.m_amountOfTrailsToSpawn)
		{
			Explode(p_playerRefrence);

			amountOfExplosions++;

			yield return new WaitForSeconds(explosionInterval);
		}
	}

	private void Explode(PlayerController p_playerRefrence)
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(p_playerRefrence.transform.position , m_explosionRadius, p_playerRefrence.m_gunnerDamageTargetMask);

		DebugExtension.DebugCircle(p_playerRefrence.transform.position , Vector3.forward, Color.yellow, m_explosionRadius, 0.1f);

		foreach (Collider2D collider in colliders)
		{
			collider.GetComponent<Health>().TakeDamage(m_trailDamageAmount);
		}
	}
}
