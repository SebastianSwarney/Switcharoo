using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Types/Ice")]
public class DamageType_Ice : DamageType_Base
{
	public override void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, float p_damageBase, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		SetIceDamage(p_bulletRefrence, p_collision, p_damageBase, p_obstacleMask, p_damageTargetMask);
	}

	private void SetIceDamage(Bullet_Base p_bulletRefrence, Collider2D p_collision, float p_damageBase, LayerMask p_obstacleMask, LayerMask p_damageTargetMask)
	{
		if (CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			Health targetHealth = p_collision.GetComponent<Health>();

			targetHealth.TakeDamage(p_damageBase);
			targetHealth.SetIceState();
		}

		if (CheckCollisionLayer(p_obstacleMask, p_collision) || CheckCollisionLayer(p_damageTargetMask, p_collision))
		{
			ObjectPooler.instance.ReturnToPool(p_bulletRefrence.gameObject);
		}
	}
}
