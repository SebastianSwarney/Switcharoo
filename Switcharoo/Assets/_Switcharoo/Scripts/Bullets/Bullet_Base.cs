using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
    public float m_moveSpeed = 10;

	private SpriteRenderer m_spriteRenderer;

	private BoxCollider2D m_collider;

	private void OnEnable()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_collider = GetComponent<BoxCollider2D>();
	}

	private void Update() 
    {
        transform.Translate(transform.right * m_moveSpeed * Time.deltaTime, Space.World);
    }

	public void InitializeParameters(Sprite p_sprite)
	{
		m_spriteRenderer.sprite = p_sprite;

		Vector2 colliderSize = m_spriteRenderer.sprite.bounds.size;
		m_collider.size = colliderSize;
		m_collider.offset = new Vector2(colliderSize.x / 2, 0);
	}
}
