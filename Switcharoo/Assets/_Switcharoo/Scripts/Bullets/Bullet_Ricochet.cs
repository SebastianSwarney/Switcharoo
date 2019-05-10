using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ricochet : Bullet_Base
{
	[Header("Ricochet Properties")]
	public int m_amountOfRicochets;
	private int m_ricochetCount;

	public override void OnEnable()
	{
		base.OnEnable();
		m_ricochetCount = 0;
	}

	public override void Update()
	{
		base.Update();
		Move();
	}

	private void Move()
	{
		transform.Translate(transform.right * m_moveSpeed * Time.deltaTime, Space.World);
	}

	private void Ricochet()
	{
		RaycastHit2D reflectCast = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, m_obstacleMask);

		if (reflectCast)
		{
			Vector3 reflectDir = Vector3.Reflect(transform.right, reflectCast.normal);

			float rotation = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;

			transform.eulerAngles = new Vector3(0, 0, rotation);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (m_damageTargetMask == (m_damageTargetMask | (1 << collision.gameObject.layer)))
		{
			m_damageType.OnContact(this, collision, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);
		}

		if ((m_obstacleMask == (m_obstacleMask | (1 << collision.gameObject.layer))) && m_ricochetCount < m_amountOfRicochets)
		{
			Ricochet();
			m_ricochetCount++;
		}

		if ((m_obstacleMask == (m_obstacleMask | (1 << collision.gameObject.layer))) && m_ricochetCount >= m_amountOfRicochets)
		{
			m_damageType.OnContact(this, collision, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);
		}
	}
}
