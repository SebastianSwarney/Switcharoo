using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Player : Health
{
	public override void Die()
	{
		m_isDead = true;
	}
}
