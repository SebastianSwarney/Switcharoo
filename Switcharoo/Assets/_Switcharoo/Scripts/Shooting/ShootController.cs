using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public ShotType_Base m_shotType;

    private float m_nextTimeToFire;

    public void Shoot(Transform p_bulletOrgin)
    {
        if (CanShoot())
        {
            m_shotType.Shoot(p_bulletOrgin);
        }
    }

    private bool CanShoot()
    {
        if (Time.time >= m_nextTimeToFire)
        {
            m_nextTimeToFire = Time.time + 1f / m_shotType.m_fireRate;

            return true;
        }
        return false;
    }
}
