using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_BulletType : Pickup_Base
{
	public Bullet_Base m_bulletType;

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetBulletTypePickup(m_bulletType);
	}
}
