using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Fire")]
public class DamageType_Fire : DamageType_Base
{
	[Header("Fire Properties")]
	public float m_fireDamageInterval;
	public float m_fireEffectLength;
	public float m_fireEffectRadius;

	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		FireBlast(p_bulletRefrence, p_collision, p_obstacleMask, p_damageTargetMask);
	}

	private void FireBlast(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			List<Health> effectedObjects = new List<Health>();

			Collider2D[] colliders = Physics2D.OverlapCircleAll(p_collision.ClosestPoint(p_bulletRefrence.transform.position), m_fireEffectRadius, p_damageTargetMask);

			for (int i = 0; i < colliders.Length; i++)
			{
				effectedObjects.Add(colliders[i].GetComponent<Health>());
			}

			effectedObjects.ForEach((health) => { health.StartCoroutine(TakeFireDamage(health)); });

			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}

	IEnumerator TakeFireDamage(Health p_healthTarget)
	{
		float t = 0;

		while (t < m_fireEffectLength)
		{
			p_healthTarget.TakeDamage(m_damageAmount);

			yield return new WaitForSeconds(m_fireDamageInterval);

			t += m_fireDamageInterval;
		}
	}
}
