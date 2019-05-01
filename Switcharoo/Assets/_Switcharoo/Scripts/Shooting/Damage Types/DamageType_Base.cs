using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageType_Base : ScriptableObject
{
	public BulletContactVisual_Base m_contactVisual;

	public abstract void OnContact(Bullet_Base p_bulletRefrence, Collider2D p_collision, LayerMask p_obstacleMask, LayerMask p_damageTargetMask);

	public bool CheckCollisionLayer(LayerMask p_layerMask, Collider2D p_collision)
	{
		if (p_layerMask == (p_layerMask | (1 << p_collision.gameObject.layer)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
