using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Patterns/Scatter")]
public class ShotPattern_Scatter : ShotPattern_Base
{
	[Header("Scatter Properties")]
	public float m_bulletSpacing;
	public int m_amountOfBulletsPerShot;

	public override void Shoot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		ShootScatter(p_bulletOrigin, p_bulletType, p_damageType, p_damageTargetMask, p_obstacleMask);
	}

	private void ShootScatter(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		float angleBetweenBullets = m_bulletSpacing / m_amountOfBulletsPerShot;

		for (int i = 0; i < m_amountOfBulletsPerShot; i++)
		{
			Quaternion bulletSpaceQuaternion = Quaternion.Euler(0, 0, (i * angleBetweenBullets) - (angleBetweenBullets * (m_amountOfBulletsPerShot / 2)));

			GameObject newBullet = ObjectPooler.instance.NewObject(p_bulletType.gameObject, p_bulletOrigin);

			newBullet.transform.rotation = p_bulletOrigin.rotation * bulletSpaceQuaternion;

			newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_damageType, m_baseBulletSpeed, m_baseDamage, p_damageTargetMask, p_obstacleMask);
		}
	}
}
