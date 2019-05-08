using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Patterns/Single")]
public class ShotPattern_Single : ShotPattern_Base
{
	[Header("Sinlge Shot Properties")]
	public float m_maxBulletSpread;

	public override void Shoot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		ShootSingleShot(p_bulletOrigin, p_bulletType, p_damageType, p_damageTargetMask, p_obstacleMask);
	}

	private void ShootSingleShot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		float bulletRotation = p_bulletOrigin.rotation.z + Random.Range(m_maxBulletSpread, -m_maxBulletSpread);

		Quaternion bulletRotationQuaternion = Quaternion.Euler(0, 0, bulletRotation);

		GameObject newBullet = ObjectPooler.instance.NewObject(p_bulletType.gameObject, p_bulletOrigin);

		newBullet.transform.rotation = p_bulletOrigin.rotation * bulletRotationQuaternion;

		newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_damageType, m_baseBulletSpeed, m_baseDamage, p_damageTargetMask, p_obstacleMask);
	}
}
