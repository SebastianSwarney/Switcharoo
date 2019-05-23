using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Health : Pickup_Base
{
	public float m_healthIncrease;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.GetComponent<Health>().HealDamage(m_healthIncrease);
	}
}
