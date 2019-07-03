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
		m_rigidbody.velocity = transform.right * m_moveSpeed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		
		if (collision.tag == "Enemy")
		{
			if (collision.gameObject.GetComponent<AiController>().m_entityType == m_type)
			{
				m_damageType.OnContact(this, collision, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);
			}
			else
			{
				collision.gameObject.GetComponent<AiController>().BulletResitant();
			}
		}
		else if (collision.tag == "Player")
		{
			m_damageType.OnContact(this, collision, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);
		}

		if (CheckCollisionLayer(m_obstacleMask, collision))
		{
			m_damageType.OnContact(this, collision, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);
		}
	}
}
