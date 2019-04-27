using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotType_Player : ShotType_Base
{
	public int m_totalAmmo;

	public override abstract void Shoot(Transform p_bulletOrigin);
}
