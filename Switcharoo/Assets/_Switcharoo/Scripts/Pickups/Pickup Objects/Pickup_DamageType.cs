using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_DamageType : Pickup_Base
{
	public DamageType_Base[] m_damageTypes;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetDamageTypePickup(m_damageTypes[RandomIndex(m_damageTypes.Length)]);
	}
}
