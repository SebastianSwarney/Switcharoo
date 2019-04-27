using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Types/Player Shot Types/Grenade Launcher")]
public class ShotType_Player_GrenadeLauncher : ShotType_Player
{
	public override void Shoot(Transform p_bulletOrigin)
	{
		ShootGrenade(p_bulletOrigin);
	}

	public override void ChargeShoot(Transform p_bulletOrigin, ChargePercent p_percentCharged)
	{
		throw new System.NotImplementedException();
	}

	private void ShootGrenade(Transform p_bulletOrigin)
	{
		GameObject newBullet = ObjectPooler.instance.NewObject(m_bulletType.gameObject, p_bulletOrigin);
	}
}
