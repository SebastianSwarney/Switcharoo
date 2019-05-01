using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotPattern_Base : ScriptableObject
{
	[Header("Base Properties")]
	public float m_fireRate;
	public int m_ammoCount;

	public abstract void Shoot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType);
}
