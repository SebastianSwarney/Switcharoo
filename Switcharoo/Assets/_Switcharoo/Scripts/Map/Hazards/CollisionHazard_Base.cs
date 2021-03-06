﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Base : MonoBehaviour, IPauseable
{
	[Header("Base Properties")]
	public float m_damageAmount;
	public float m_damageInterval;
	public LayerMask m_targetMask;

	[HideInInspector]
	public float m_damageTimer;
	[HideInInspector]
	public Rigidbody2D m_rigidbody;
	[HideInInspector]
	public bool m_canDamage = true;
	[HideInInspector]
	public SpriteRenderer m_renderer;
    
    [HideInInspector]
    public bool m_paused;

	public virtual void Start()
	{
		m_renderer = GetComponent<SpriteRenderer>();
		m_damageTimer = m_damageInterval;
		m_rigidbody = GetComponent<Rigidbody2D>();

		m_canDamage = true;

        ObjectPooler.instance.AddObjectToPauser(gameObject);
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
				if (CheckCollisionLayer(m_targetMask, collision.collider))
				{
					collision.gameObject.GetComponent<Health>().TakeDamage(m_damageAmount);
				}
			}

			m_damageTimer = 0;
		}

		m_damageTimer += Time.deltaTime;
	}

	public bool CheckCollisionLayer(LayerMask p_layerMask, Collider2D p_collision)
	{
		if (p_layerMask == (p_layerMask | (1 << p_collision.gameObject.layer)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

    public void SetPauseState(bool p_isPaused)
    {
        PauseMe(p_isPaused);
    }

    public virtual void PauseMe(bool p_paused)
    {
        m_paused = p_paused;
    }
}
