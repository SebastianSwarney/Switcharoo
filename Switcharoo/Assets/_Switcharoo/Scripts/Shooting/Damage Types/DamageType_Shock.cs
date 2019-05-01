using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Shock")]
public class DamageType_Shock : DamageType_Base
{
	[Header("Shock Properties")]
	public float m_shockRadius;
	public int m_shockChainAmount;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		Shock(p_bulletRefrence, p_collision, p_obstacleMask, p_damageTargetMask);
	}

	private void Shock(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			List<Collider2D> shockedObjects = new List<Collider2D>();

			Collider2D[] initialCast = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_shockRadius, p_damageTargetMask);

			shockedObjects.AddRange(initialCast);

			for (int i = 0; i < shockedObjects.Count && i < m_shockChainAmount; i++)
			{
				Collider2D[] childCast = Physics2D.OverlapCircleAll(shockedObjects[i].transform.position, m_shockRadius, p_damageTargetMask);

				foreach (Collider2D collider in childCast)
				{
					if (!shockedObjects.Contains(collider))
					{
						shockedObjects.Add(collider);
					}
				}
			}

			foreach (Collider2D collider in shockedObjects)
			{
				collider.GetComponent<Health>().TakeDamage(m_damageAmount);
			}

			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}
}
