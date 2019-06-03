using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_BuffType : Pickup_Base
{
	public PlayerBuff_Base[] m_buffTypes;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetBuffPickup(m_buffTypes[RandomIndex(m_buffTypes.Length)]);
	}
}
