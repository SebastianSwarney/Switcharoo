using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Fire")]
public class DamageType_Fire : DamageType_Base
{
	[Header("Fire Properties")]
	public float m_fireEffectLength;
	public float m_fireEffectRadius;
	public int m_fireHitsPerEffect;
	public float m_fireDamageAmount;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		FireBlast(p_bulletRefrence, p_collision, p_obstacleMask, p_damageTargetMask);
	}

	//Does the normal bullet damage and casts to set other targets on fire
	private void FireBlast(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			p_collision.GetComponent<Health>().TakeDamage(m_damageAmount);
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			List<Health> effectedObjects = new List<Health>();

			Collider2D[] colliders = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_fireEffectRadius, p_damageTargetMask);

			DebugExtension.DebugCircle(p_collision.ClosestPoint(p_bulletRefrence.transform.position), Vector3.forward, Color.red, m_fireEffectRadius, 0.5f);

			for (int i = 0; i < colliders.Length; i++)
			{
				effectedObjects.Add(colliders[i].GetComponent<Health>());
			}

			effectedObjects.ForEach((health) => { health.StartCoroutine(TakeFireDamage(health)); });

			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}

	//Deals the fire damage
	IEnumerator TakeFireDamage(Health p_healthTarget)
	{
		if (!p_healthTarget.m_onFire)
		{
			p_healthTarget.m_onFire = true;

			float fireEffectInterval = m_fireEffectLength / m_fireHitsPerEffect;

			int amountOfEffects = 0;

			while (amountOfEffects < m_fireHitsPerEffect)
			{
				p_healthTarget.TakeDamage(m_fireDamageAmount);

				DebugExtension.DebugCircle(p_healthTarget.transform.position, Vector3.forward, Color.red, 1, 0.1f);

				amountOfEffects++;

				yield return new WaitForSeconds(fireEffectInterval);
			}

			p_healthTarget.m_onFire = false;
		}
	}
}
