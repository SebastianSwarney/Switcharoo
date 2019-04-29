using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
	public Sprite[] m_chargeSprites;

	public float m_moveSpeed;
	public float m_damage;

	public LayerMask m_damageTargetMask;

	public LayerMask m_obstacleMask;

	public float m_timeUntillDeactivate = 60;
	private float m_deactivateTimer;

	public BulletContactBehaviour_Base m_contactBehaviour;

	private SpriteRenderer m_spriteRenderer;
	private BoxCollider2D m_collider;

	public virtual void OnEnable()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_collider = GetComponent<BoxCollider2D>();
	}

	public virtual void Update()
	{
		RemoveAfterTime();
	}

	private void RemoveAfterTime()
	{
		m_deactivateTimer += Time.deltaTime;

		if (m_deactivateTimer >= m_timeUntillDeactivate)
		{
			m_deactivateTimer = 0;

			ObjectPooler.instance.ReturnToPool(gameObject);
		}
	}

	public void InitializeParameters(Sprite p_sprite, float p_moveSpeed)
	{
		m_spriteRenderer.sprite = p_sprite;
		m_moveSpeed = p_moveSpeed;
	}
}
