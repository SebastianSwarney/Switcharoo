using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_ShotPattern : Pickup_Base
{
	public ShotPattern_Base m_shotPattern;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetShotPatternPickup(m_shotPattern);
	}
}
