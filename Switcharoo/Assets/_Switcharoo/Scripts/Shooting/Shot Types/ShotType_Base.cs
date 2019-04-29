using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotType_Base : ScriptableObject
{
	[Header("Base Properties")]
	public float m_fireRate;
	public float m_baseDamage;
	public float m_bulletSpeed;

	public Bullet_Base m_bulletType;

    public abstract void Shoot(Transform p_bulletOrigin);
}
