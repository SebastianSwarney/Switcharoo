using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_MovementAbility : Pickup_Base
{
	public ShotType_Player m_movementAbilityPickup;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetWeaponPickup(m_movementAbilityPickup);
	}
}
