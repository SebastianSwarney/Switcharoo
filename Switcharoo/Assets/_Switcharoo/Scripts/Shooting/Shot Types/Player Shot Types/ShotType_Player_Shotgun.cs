using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Types/Player Shot Types/Shotgun")]
public class ShotType_Player_Shotgun : ShotType_Player
{
	[Header("Shotgun Properties")]
	public float m_maxBulletSpacing;
	public float m_minBulletSpacing;
	public int m_amountOfBulletsPerShot;

	public override void Shoot(Transform p_bulletOrigin)
	{
		ShootShotgun(p_bulletOrigin, m_maxBulletSpacing);
	}
	
	#region Charge Shot
	public override void ChargeShoot(Transform p_bulletOrigin, ChargePercent p_percentCharged)
	{
		switch (p_percentCharged)
		{
			case ChargePercent._0:

				ShootShotgun(p_bulletOrigin, m_maxBulletSpacing);

				break;

			case ChargePercent._25:

				ShootShotgun(p_bulletOrigin, (m_maxBulletSpacing + m_minBulletSpacing) * 0.75f);

				break;

			case ChargePercent._50:

				ShootShotgun(p_bulletOrigin, (m_maxBulletSpacing + m_minBulletSpacing) * 0.50f);

				break;

			case ChargePercent._75:

				ShootShotgun(p_bulletOrigin, (m_maxBulletSpacing + m_minBulletSpacing) * 0.25f);

				break;

			case ChargePercent._100:

				ShootShotgun(p_bulletOrigin, m_minBulletSpacing);

				break;
		}
	}
	#endregion

	private void ShootShotgun(Transform p_bulletOrigin, float p_bulletSpacing)
	{
		float angleBetweenBullets = p_bulletSpacing / m_amountOfBulletsPerShot;

		for (int i = 0; i < m_amountOfBulletsPerShot; i++)
		{
			Quaternion bulletSpaceQuaternion = Quaternion.Euler(0, 0, (i * angleBetweenBullets) - (angleBetweenBullets * (m_amountOfBulletsPerShot / 2)));

			GameObject newBullet = ObjectPooler.instance.NewObject(m_bulletType.gameObject, p_bulletOrigin);

			newBullet.transform.rotation = p_bulletOrigin.rotation * bulletSpaceQuaternion;

			newBullet.GetComponent<Bullet_Base>().InitializeParameters(m_bulletType.m_chargeSprites[0]);
		}
	}
}
