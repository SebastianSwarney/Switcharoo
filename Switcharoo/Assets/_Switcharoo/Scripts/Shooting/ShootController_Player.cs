using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController_Player : MonoBehaviour
{
	public ShotType_Player m_shotTypePlayer;

	public float m_chargeUpTime;

	[SerializeField]
    private int m_ammoCount;
	[HideInInspector]
	public float m_nextTimeToFire;
	[SerializeField]
	private float m_chargeValue;
	private float m_chargeTimer;

	private void Start()
	{
		InitalizeValues();
	}

    public void InitalizeValues()
    {
        m_ammoCount = m_shotTypePlayer.m_totalAmmo;
    }

	public void Shoot(Transform p_bulletOrigin)
	{
		if (CanShoot())
		{
			m_shotTypePlayer.Shoot(p_bulletOrigin);
		}
	}

	public void Reload()
    {
        m_ammoCount = m_shotTypePlayer.m_totalAmmo;
    }

	public bool CanShoot()
	{
		if (Time.time >= m_nextTimeToFire && m_ammoCount > 0)
		{
			m_nextTimeToFire = Time.time + 1f / m_shotTypePlayer.m_fireRate;

			m_ammoCount--;

			return true;
		}
		return false;
	}

	public void ChargeUpShot()
	{
		m_chargeTimer += Time.deltaTime;

		m_chargeValue = Mathf.Lerp(0, 100, m_chargeTimer / m_chargeUpTime);
	}

	public void FireChargedShot(Transform p_bulletOrigin)
	{
		if (CanShoot())
		{
			m_shotTypePlayer.ChargeShoot(p_bulletOrigin, CheckChargePercent(m_chargeValue));
		}

		m_chargeTimer = 0;
		m_chargeValue = 0;
	}

	private ShotType_Player.ChargePercent CheckChargePercent(float p_chargeValue)
	{
		if (p_chargeValue < 25)
		{
			return ShotType_Player.ChargePercent._0;
		}

		if (p_chargeValue >= 25 && p_chargeValue < 50)
		{
			return ShotType_Player.ChargePercent._25;
		}

		if (p_chargeValue >= 50 && p_chargeValue < 75)
		{
			return ShotType_Player.ChargePercent._50;
		}

		if (p_chargeValue >= 75 && p_chargeValue < 100)
		{
			return ShotType_Player.ChargePercent._75;
		}

		if (p_chargeValue >= 100)
		{
			return ShotType_Player.ChargePercent._100;
		}

		return ShotType_Player.ChargePercent._0;
	}

}
