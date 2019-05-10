using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
	public WeaponComposition m_currentWeaponComposition;

	public LayerMask m_damageTargetMask;
	public LayerMask m_obstacleMask;

	[HideInInspector]
    public float m_nextTimeToFire;
	public int m_ammoCount;
	private bool isPlayer;

	public void Shoot(Transform p_bulletOrigin)
    {
        if (CanShoot())
        {
            m_currentWeaponComposition.m_shotPattern.Shoot(p_bulletOrigin, m_currentWeaponComposition.m_bulletType, m_currentWeaponComposition.m_damageType, m_damageTargetMask, m_obstacleMask);
        }
    }

	public void Reload()
	{
		if (m_currentWeaponComposition.m_shotPattern.m_ammoCount > 0)
		{
			isPlayer = true;
		}

		m_ammoCount = m_currentWeaponComposition.m_shotPattern.m_ammoCount;
	}

	public bool CanShoot()
	{
		if (isPlayer)
		{
			if (Time.time >= m_nextTimeToFire && m_ammoCount > 0)
			{
				m_nextTimeToFire = Time.time + 1f / m_currentWeaponComposition.m_shotPattern.m_fireRate;

				m_ammoCount--;

				return true;
			}

			return false;

		}
		else
		{
			if (Time.time >= m_nextTimeToFire)
			{
				m_nextTimeToFire = Time.time + 1f / m_currentWeaponComposition.m_shotPattern.m_fireRate;

				return true;
			}

			return false;
		} 
	}

	[System.Serializable]
	public struct WeaponComposition
	{
		public ShotPattern_Base m_shotPattern;
		public Bullet_Base m_bulletType;
		public DamageType_Base m_damageType;
	}
}
