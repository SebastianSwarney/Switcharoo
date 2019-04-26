using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public ShotType_Base m_shotType;

    private float m_nextTimeToFire;

    [SerializeField]
    private int m_ammoCount;

    private void Start() 
    {
        InitalizeValues();
    }
    
    public void InitalizeValues()
    {
        m_ammoCount = m_shotType.m_ammo;
    }

    public void Shoot(Transform p_bulletOrgin)
    {
        if (CanShoot())
        {
            m_shotType.Shoot(p_bulletOrgin);
        }
    }

    private bool CanShoot()
    {
        if (m_shotType.m_isPlayer)
        {
            if (Time.time >= m_nextTimeToFire && m_ammoCount > 0)
            {
                m_nextTimeToFire = Time.time + 1f / m_shotType.m_fireRate;

                m_ammoCount--;

                return true;
            }
            return false;
        } 
        else
        {
            if (Time.time >= m_nextTimeToFire)
            {
                m_nextTimeToFire = Time.time + 1f / m_shotType.m_fireRate;

                return true;
            }

            return false;
        }
    }
}
