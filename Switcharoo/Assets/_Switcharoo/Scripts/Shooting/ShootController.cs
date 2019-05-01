using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public ShotType_Base m_shotType;

	[HideInInspector]
    public float m_nextTimeToFire;

    public void Shoot(Transform p_bulletOrigin)
    {
        if (CanShoot())
        {
            m_shotType.Shoot(p_bulletOrigin);
        }
    }

	public void FireChargedShot(Transform p_bulletOrigin, ShotType_Base.ChargePercent p_percentCharged)
	{
		if (CanShoot())
		{
			m_shotType.ChargeShoot(p_bulletOrigin, p_percentCharged);
		}
	}

	public bool CanShoot()
    {
        if (Time.time >= m_nextTimeToFire)
        {
            m_nextTimeToFire = Time.time + 1f / m_shotType.m_fireRate;

            return true;
        }

        return false;
	}
}
