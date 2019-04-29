using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ConstMove : Bullet_Base
{
	public override void Update()
	{
		base.Update();

		Move();
	}

	private void Move()
	{
		transform.Translate(transform.right * m_moveSpeed * Time.deltaTime, Space.World);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		m_contactBehaviour.OnContact(this, collision, m_obstacleMask, m_damageTargetMask);
	}
}
