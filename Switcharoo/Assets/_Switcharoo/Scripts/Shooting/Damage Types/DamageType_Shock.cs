using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Shock")]
public class DamageType_Shock : DamageType_Base
{
	[Header("Shock Properties")]
	public float m_shockRadius;
	public int m_shockChainAmount;
	public float m_shockDamage;

	public GameObject m_lightingEffect;
	public float m_effectsPerUnit;
	public float m_effectSpacing;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, float p_damageBase, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		Shock(p_bulletRefrence, p_collision, p_damageBase, p_obstacleMask, p_damageTargetMask);
	}

	private void Shock(Bullet_Base p_bulletRefrence, Collider2D p_collision, float p_damageBase, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			p_collision.GetComponent<Health>().TakeDamage(p_damageBase);
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			List<Collider2D> shockedObjects = new List<Collider2D>();

			Collider2D[] initialCast = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_shockRadius, p_damageTargetMask);

			DebugExtension.DebugCircle(p_collision.ClosestPoint(p_bulletRefrence.transform.position), Vector3.forward, Color.blue, m_shockRadius, 0.1f);

			shockedObjects.AddRange(initialCast);

			foreach (Collider2D collider in initialCast)
			{
				SpawnLightingEffects(p_collision.ClosestPoint(p_bulletRefrence.transform.position), collider.transform.position);
			}

			for (int i = 0; i < shockedObjects.Count && i < m_shockChainAmount; i++)
			{
				Collider2D[] childCast = Physics2D.OverlapCircleAll(shockedObjects[i].transform.position, m_shockRadius, p_damageTargetMask);

				DebugExtension.DebugCircle(shockedObjects[i].transform.position, Vector3.forward, Color.blue, m_shockRadius, 0.1f);

				foreach (Collider2D collider in childCast)
				{
					if (!shockedObjects.Contains(collider))
					{
						shockedObjects.Add(collider);
					}
				}

				if (i < shockedObjects.Count - 1)
				{
					SpawnLightingEffects(shockedObjects[i].transform.position, shockedObjects[i + 1].transform.position);
				}
			}

			foreach (Collider2D collider in shockedObjects)
			{
				collider.GetComponent<Health>().TakeDamage(m_shockDamage);
			}

			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}

	public void SpawnLightingEffects(Vector3 p_startPos, Vector3 p_endPos)
	{
		Vector2 reletivePos = p_startPos - p_endPos;

		float theta = Mathf.Atan2(reletivePos.y, reletivePos.x);

		float thetaDegrees = theta * Mathf.Rad2Deg;

		int dst = Mathf.RoundToInt(Vector3.Distance(p_startPos, p_endPos));

		int effectsAmount = Mathf.RoundToInt(dst * m_effectsPerUnit);

		Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);

		for (int i = 0; i < effectsAmount; i++)
		{
			float spacing = i * (m_effectsPerUnit + m_effectSpacing);

			Vector3 spawnPos = p_endPos + pCircle * spacing;
			ObjectPooler.instance.NewObject(m_lightingEffect, spawnPos, Quaternion.Euler(0 ,180 , -thetaDegrees));
		}
	}
}
