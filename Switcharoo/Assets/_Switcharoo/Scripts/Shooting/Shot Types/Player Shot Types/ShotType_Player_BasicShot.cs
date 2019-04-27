using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Types/Player Shot Types/Basic Shot")]
public class ShotType_Player_BasicShot : ShotType_Player
{
	public override void Shoot(Transform p_bulletOrigin)
	{
		ShootBasicShot(p_bulletOrigin, m_bulletType.m_chargeSprites[0]);
	}

	#region Charge Shot
	public override void ChargeShoot(Transform p_bulletOrigin, ChargePercent p_percentCharged)
	{
		switch (p_percentCharged)
		{
			case ChargePercent._0:

				ShootBasicShot(p_bulletOrigin, m_bulletType.m_chargeSprites[0]);

				break;

			case ChargePercent._25:

				ShootBasicShot(p_bulletOrigin, m_bulletType.m_chargeSprites[1]);

				break;

			case ChargePercent._50:

				ShootBasicShot(p_bulletOrigin, m_bulletType.m_chargeSprites[2]);

				break;

			case ChargePercent._75:

				ShootBasicShot(p_bulletOrigin, m_bulletType.m_chargeSprites[3]);

				break;

			case ChargePercent._100:

				ShootBasicShot(p_bulletOrigin, m_bulletType.m_chargeSprites[4]);

				break;
		}
	}
	#endregion

	private void ShootBasicShot(Transform p_bulletOrigin, Sprite p_bulletSprite)
	{
		GameObject newBullet = ObjectPooler.instance.NewObject(m_bulletType.gameObject, p_bulletOrigin);
		newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_bulletSprite);
	}
}
