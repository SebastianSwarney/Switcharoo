using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_MovementType : Pickup_Base
{
	public MovementType_Base[] m_movementTypes;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetMovementTypePickup(m_movementTypes[RandomIndex(m_movementTypes.Length)]);
	}
}
