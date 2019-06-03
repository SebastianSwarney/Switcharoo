using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_BulletType : Pickup_Base
{
	public Bullet_Base[] m_bulletTypes;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetBulletTypePickup(m_bulletTypes[RandomIndex(m_bulletTypes.Length)]);
	}
}
