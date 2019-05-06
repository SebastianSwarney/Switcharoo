using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Homing : Bullet_Base
{
	private Rigidbody2D m_rigidbody;

	[SerializeField]
	private Transform m_target;

	public float m_rotateSpeed = 100f;

	public override void OnEnable()
	{
		base.OnEnable();
		m_rigidbody = GetComponent<Rigidbody2D>();
	}

	public override void Update()
	{
		base.Update();
		MoveToTarget();

		if (m_target.gameObject.activeSelf == false)
		{
			FindTarget();
		}
	}

	public override void InitializeParameters(DamageType_Base p_damageType)
	{
		base.InitializeParameters(p_damageType);
		FindTarget();
	}

	private void FindTarget()
	{
		Collider2D collider = Physics2D.OverlapCircle(transform.position, 100, m_damageTargetMask); //May need to be made a variable range

		m_target = collider.transform;
	}

	public void MoveToTarget()
	{
		Vector2 direction = m_target.position - transform.position;

		direction.Normalize();

		float rotateAmount = Vector3.Cross(direction, transform.right).z;

		m_rigidbody.angularVelocity = -rotateAmount * m_rotateSpeed;

		m_rigidbody.velocity = transform.right * m_moveSpeed;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		m_damageType.OnContact(this, collision.collider, m_obstacleMask, m_damageTargetMask);
	}
}
