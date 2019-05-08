using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Patterns/Burst")]
public class ShotPattern_Burst : ShotPattern_Base
{
	[Header("Burst Shot Properties")]
	public float m_bulletsPerBurst;
	public float m_burstDelay;

	public override void Shoot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_bulletOrigin.GetComponentInParent<ShootController>().StartCoroutine(BurstShot(p_bulletOrigin, p_bulletType, p_damageType, p_damageTargetMask, p_obstacleMask));
	}

	IEnumerator BurstShot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		int amountOfBulletsShot = 0;

		while (amountOfBulletsShot < m_bulletsPerBurst)
		{
			GameObject newBullet = ObjectPooler.instance.NewObject(p_bulletType.gameObject, p_bulletOrigin);

			newBullet.transform.rotation = p_bulletOrigin.rotation;

			newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_damageType, m_baseBulletSpeed, m_baseDamage, p_damageTargetMask, p_obstacleMask);

			amountOfBulletsShot++;

			yield return new WaitForSeconds(m_burstDelay);
		}
	}
}
