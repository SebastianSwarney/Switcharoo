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
	}

	public override void Update()
	{
		base.Update();

		RotateToMovement();
	}

	public override void InitializeParameters(DamageType_Base p_damageType)
	{
		base.InitializeParameters(p_damageType);

		Launch();
	}

	public void Launch()
	{
		m_rigidbody.AddForce(transform.right * m_moveSpeed, ForceMode2D.Impulse);
	}

	private void RotateToMovement()
	{
		if (m_rigidbody.velocity != Vector2.zero)
		{
			float angle = Mathf.Atan2(m_rigidbody.velocity.y, m_rigidbody.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		m_damageType.OnContact(this, collision.collider, m_obstacleMask, m_damageTargetMask);
	}
}
