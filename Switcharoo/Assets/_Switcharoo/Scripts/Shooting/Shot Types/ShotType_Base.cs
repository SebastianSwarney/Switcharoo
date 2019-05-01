using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotType_Base : ScriptableObject
{
	public enum ChargePercent { _0, _25, _50, _75, _100 }

	[Header("Base Properties")]
	public float m_fireRate;
	public float m_baseDamage;
	public float m_bulletSpeed;

	public Bullet_Base m_bulletType;

    public abstract void Shoot(Transform p_bulletOrigin);

	public abstract void ChargeShoot(Transform p_bulletOrigin, ChargePercent p_percentCharged);
}
