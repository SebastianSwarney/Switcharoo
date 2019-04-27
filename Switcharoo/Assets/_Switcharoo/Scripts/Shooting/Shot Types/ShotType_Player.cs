using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotType_Player : ShotType_Base
{
	public enum ChargePercent {_0, _25, _50, _75, _100}

	[Header("Base Player Properites")]
	public int m_totalAmmo;
	public float chargeDamageIncreaseFactor;

	public override abstract void Shoot(Transform p_bulletOrigin);

	public abstract void ChargeShoot(Transform p_bulletOrigin, ChargePercent p_percentCharged);
}
