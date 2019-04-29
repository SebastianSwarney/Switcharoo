using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ConstMove : Bullet_Base
{
    private void Update()
    {
		Move();
    }

	private void Move()
	{
		transform.Translate(transform.right * m_moveSpeed * Time.deltaTime, Space.World);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (m_damageTargetMask == (m_damageTargetMask | (1 << collision.gameObject.layer)))
		{
			collision.gameObject.GetComponent<Health>().TakeDamage(m_damage);
		}

		if (m_obstacleMask == (m_obstacleMask | (1 << collision.gameObject.layer)))
		{
			ObjectPooler.instance.ReturnToPool(gameObject);
		}
	}
}
