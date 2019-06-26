using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Homing : Bullet_Base
{
	[Header("Homing Properties")]
	public float m_rotateSpeed = 100f;
	private Transform m_target;

	public override void Update()
	{
		base.Update();

		if (m_target != null)
		{
			MoveToTarget();

			if (m_target.gameObject.activeSelf == false)
			{
				FindTarget();
			}
		}
		else
		{
			MoveForward();
		}
	}

	public override void InitializeParameters(DamageType_Base p_damageType, float p_moveSpeed, float p_damageAmount, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		base.InitializeParameters(p_damageType, p_moveSpeed, p_damageAmount, p_damageTargetMask, p_obstacleMask);
		FindTarget();
	}

	public override void InitializeParameters(DamageType_Base p_damageType, float p_moveSpeed, float p_damageAmount, LayerMask p_damageTargetMask, LayerMask p_obstacleMask, PlayerController.PlayerType m_playerType)
	{
		base.InitializeParameters(p_damageType, p_moveSpeed, p_damageAmount, p_damageTargetMask, p_obstacleMask, m_playerType);
		FindTarget();
	}

	private void FindTarget()
	{
		Collider2D collider = Physics2D.OverlapCircle(transform.position, 100, m_damageTargetMask); //May need to be made a variable range

		if (collider != null)
		{
			m_target = collider.transform;
		}
	}

	private void MoveForward()
	{
		m_rigidbody.velocity = transform.right * m_moveSpeed;
	}

	public void MoveToTarget()
	{
		Vector2 direction = m_target.position - transform.position;
		direction.Normalize();
		float rotateAmount = Vector3.Cross(direction, transform.right).z;
		m_rigidbody.angularVelocity = -rotateAmount * m_rotateSpeed;
		m_rigidbody.velocity = transform.right * m_moveSpeed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		m_damageType.OnContact(this, collision, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);
	}
}
