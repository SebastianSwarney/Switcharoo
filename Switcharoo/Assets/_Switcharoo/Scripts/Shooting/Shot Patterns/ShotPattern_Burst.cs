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

	public override void Shoot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask, PlayerController p_player)
	{
		p_bulletOrigin.GetComponentInParent<ShootController>().StartCoroutine(BurstShot(p_bulletOrigin, p_bulletType, p_damageType, p_damageTargetMask, p_obstacleMask, p_player));
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

	IEnumerator BurstShot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask, PlayerController p_player)
	{
		int amountOfBulletsShot = 0;

		while (amountOfBulletsShot < m_bulletsPerBurst)
		{
			GameObject newBullet = ObjectPooler.instance.NewObject(p_bulletType.gameObject, p_bulletOrigin);

			newBullet.transform.rotation = p_bulletOrigin.rotation;

			for (int i = 0; i < p_player.m_players.Length; i++)
			{
				if (p_player.m_players[i].m_currentRole == PlayerController.PlayerRole.Gunner)
				{
					newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_damageType, m_baseBulletSpeed, m_baseDamage, p_damageTargetMask, p_obstacleMask, p_player.m_players[i].m_type);
				}
			}

			amountOfBulletsShot++;

			yield return new WaitForSeconds(m_burstDelay);
		}
	}
}
