using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Explode")]
public class TrailType_Explode : TrailType_Base
{
	[Header("Explosion Properites")]
	public float m_explosionRadius;
	public int m_amountOfExplosionsPerUse;
	public LayerMask m_testmask;

	public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		p_playerRefrence.StartCoroutine(ExplosionTrail(p_playerRefrence, p_movementType));
	}

	IEnumerator ExplosionTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		float explosionInterval = p_movementType.m_movementTime / m_amountOfExplosionsPerUse;

		int amountOfExplosions = 0;

		while (amountOfExplosions < m_amountOfExplosionsPerUse)
		{
			Explode(p_playerRefrence, m_testmask);

			amountOfExplosions++;

			yield return new WaitForSeconds(explosionInterval);
		}
	}

	private void Explode(PlayerController p_playerRefrence, LayerMask p_damageTargetMask)
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(p_playerRefrence.transform.position , m_explosionRadius, p_damageTargetMask);

		DebugExtension.DebugCircle(p_playerRefrence.transform.position , Vector3.forward, Color.yellow, m_explosionRadius, 0.1f);

		foreach (Collider2D collider in colliders)
		{
			collider.GetComponent<Health>().TakeDamage(m_trailDamageAmount);
		}
	}
}
