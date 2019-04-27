using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController_Player : ShootController
{
	[SerializeField]
    private int m_ammoCount;

	private void Start()
	{
		InitalizeValues();
	}

    public void InitalizeValues()
    {
        //m_ammoCount = m_shotType.m_ammo;
    }

    public void Reload()
    {
        //m_ammoCount = m_shotType.m_ammo;
    }

	public override bool CanShoot()
	{
		if (Time.time >= m_nextTimeToFire && m_ammoCount > 0)
		{
			m_nextTimeToFire = Time.time + 1f / m_shotType.m_fireRate;

			m_ammoCount--;

			return true;
		}
		return false;
	}
}
