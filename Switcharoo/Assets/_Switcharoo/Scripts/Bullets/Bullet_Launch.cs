using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Launch : Bullet_Base
{
	public override void Update()
	{
		base.Update();
		RotateToMovement();
	}

	public override void InitializeParameters(DamageType_Base p_damageType, float p_moveSpeed, float p_damageAmount, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		base.InitializeParameters(p_damageType, p_moveSpeed, p_damageAmount, p_damageTargetMask, p_obstacleMask);
		Launch();
	}

	public override void InitializeParameters(DamageType_Base p_damageType, float p_moveSpeed, float p_damageAmount, LayerMask p_damageTargetMask, LayerMask p_obstacleMask, PlayerController.PlayerType m_playerType)
	{
		base.InitializeParameters(p_damageType, p_moveSpeed, p_damageAmount, p_damageTargetMask, p_obstacleMask, m_playerType);
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
		m_damageType.OnContact(this, collision.collider, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);
	}
}
