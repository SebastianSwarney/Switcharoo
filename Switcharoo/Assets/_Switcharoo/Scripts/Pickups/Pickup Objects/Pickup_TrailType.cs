using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_TrailType : Pickup_Base
{
	public TrailType_Base[] m_trailTypes;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetTrailTypePickup(m_trailTypes[RandomIndex(m_trailTypes.Length)]);
	}
}
