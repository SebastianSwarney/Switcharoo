using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_MovementAbility : Pickup_Base
{
	public MovementAbility_Base m_movementAbilityPickup;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetMovementPickup(m_movementAbilityPickup);
	}
}
