using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Base : MonoBehaviour
{
	[Header("Base Properties")]
	public float m_damageAmount;
	public float m_damageInterval;
	public LayerMask m_targetMask;

	private float m_damageTimer;
	[HideInInspector]
	public Rigidbody2D m_rigidbody;
	[HideInInspector]
	public bool m_canDamage;
	[HideInInspector]
	public SpriteRenderer m_renderer;

	public virtual void Start()
	{
		m_renderer = GetComponent<SpriteRenderer>();
		m_damageTimer = m_damageInterval;
		m_rigidbody = GetComponent<Rigidbody2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		m_damageTimer = m_damageInterval;
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (m_damageTimer >= m_damageInterval)
		{
			if (m_canDamage)
			{
				collision.gameObject.GetComponent<Health>().TakeDamage(m_damageAmount);
			}

			m_damageTimer = 0;
		}

		m_damageTimer += Time.deltaTime;
	}
}
