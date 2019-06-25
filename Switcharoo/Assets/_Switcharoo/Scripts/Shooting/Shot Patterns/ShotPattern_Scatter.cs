using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Patterns/Scatter")]
public class ShotPattern_Scatter : ShotPattern_Base
{
	[Header("Scatter Properties")]
	public float m_bulletSpacing;
	public int m_amountOfBulletsPerShot;

    [Range(0, 15)]
    public float m_randomRotation;
    public float m_verticalOffset;

	public override void Shoot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		ShootScatter(p_bulletOrigin, p_bulletType, p_damageType, p_damageTargetMask, p_obstacleMask);
	}

	public override void Shoot(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask, PlayerController p_player)
	{
		ShootScatter(p_bulletOrigin, p_bulletType, p_damageType, p_damageTargetMask, p_obstacleMask, p_player);
	}

	private void ShootScatter(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		float angleBetweenBullets = m_bulletSpacing / m_amountOfBulletsPerShot;

        float verticalOffset = -m_verticalOffset;
        float vOIncrease = (m_verticalOffset*2) / m_amountOfBulletsPerShot;
        

		for (int i = 0; i < m_amountOfBulletsPerShot; i++)
		{
			Quaternion bulletSpaceQuaternion = Quaternion.Euler(0, 0, (i * angleBetweenBullets) - (angleBetweenBullets * (m_amountOfBulletsPerShot / 2)));

			GameObject newBullet = ObjectPooler.instance.NewObject(p_bulletType.gameObject, p_bulletOrigin);

			newBullet.transform.rotation = p_bulletOrigin.rotation * bulletSpaceQuaternion;
            float randomRotation = Random.Range(-m_randomRotation, m_randomRotation);
            newBullet.transform.eulerAngles = new Vector3(0f, 0f, newBullet.transform.eulerAngles.z + randomRotation);

			newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_damageType, m_baseBulletSpeed, m_baseDamage, p_damageTargetMask, p_obstacleMask);
            newBullet.transform.position = p_bulletOrigin.transform.position + (p_bulletOrigin.up.normalized * verticalOffset);
            verticalOffset += vOIncrease;
            
		}
	}

	private void ShootScatter(Transform p_bulletOrigin, Bullet_Base p_bulletType, DamageType_Base p_damageType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask, PlayerController p_player)
	{
		float angleBetweenBullets = m_bulletSpacing / m_amountOfBulletsPerShot;

		float verticalOffset = -m_verticalOffset;
		float vOIncrease = (m_verticalOffset * 2) / m_amountOfBulletsPerShot;


		for (int i = 0; i < m_amountOfBulletsPerShot; i++)
		{
			Quaternion bulletSpaceQuaternion = Quaternion.Euler(0, 0, (i * angleBetweenBullets) - (angleBetweenBullets * (m_amountOfBulletsPerShot / 2)));

			GameObject newBullet = ObjectPooler.instance.NewObject(p_bulletType.gameObject, p_bulletOrigin);

			newBullet.transform.rotation = p_bulletOrigin.rotation * bulletSpaceQuaternion;
			float randomRotation = Random.Range(-m_randomRotation, m_randomRotation);
			newBullet.transform.eulerAngles = new Vector3(0f, 0f, newBullet.transform.eulerAngles.z + randomRotation);

			for (int i2 = 0; i2 < p_player.m_players.Length; i2++)
			{
				if (p_player.m_players[i2].m_currentRole == PlayerController.PlayerRole.Gunner)
				{
					newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_damageType, m_baseBulletSpeed, m_baseDamage, p_damageTargetMask, p_obstacleMask, p_player.m_players[i2].m_type);
				}
			}
			newBullet.transform.position = p_bulletOrigin.transform.position + (p_bulletOrigin.up.normalized * verticalOffset);
			verticalOffset += vOIncrease;

		}
	}
}
