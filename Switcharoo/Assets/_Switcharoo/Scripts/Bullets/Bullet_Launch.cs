using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Launch : Bullet_Base
{
	private Rigidbody2D m_rigidbody;

	public override void OnEnable()
	{
		base.OnEnable();

		m_rigidbody = GetComponent<Rigidbody2D>();

		Launch();
	}

	private void Launch()
	{
		m_rigidbody.AddForce(transform.right * m_moveSpeed, ForceMode2D.Impulse);
	}
}
