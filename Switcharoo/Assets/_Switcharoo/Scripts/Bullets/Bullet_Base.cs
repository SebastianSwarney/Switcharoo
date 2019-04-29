using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
	public Sprite[] m_chargeSprites;

	public float m_moveSpeed = 10;

	private SpriteRenderer m_spriteRenderer;
	private BoxCollider2D m_collider;

	public virtual void OnEnable()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_collider = GetComponent<BoxCollider2D>();
	}

	public void InitializeParameters(Sprite p_sprite, float p_moveSpeed)
	{
		m_spriteRenderer.sprite = p_sprite;

		Vector2 colliderSize = m_spriteRenderer.sprite.bounds.size;
		m_collider.size = colliderSize;
		m_collider.offset = new Vector2(colliderSize.x / 2, 0);

		m_moveSpeed = p_moveSpeed;
	}
}
