using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup_Base : MonoBehaviour
{
	public LayerMask m_playerLayer;

	public SpriteRenderer m_itemIconRenderer;

	public float m_switchItemTime;
	private float m_switchTimer;

	[HideInInspector]
	public int m_currentItem;
	private int m_lastItem;

	[HideInInspector]
	public int m_amountOfItems;
	private bool m_isUsed;

	public abstract void SetPickup(PlayerController p_playerRefrence);

	public int RandomItemSelection()
	{
		int random = Random.Range(0, m_amountOfItems);
		if (random == m_lastItem)
		{
			random = Random.Range(0, m_amountOfItems);
		}
		m_lastItem = random;
		return random;
	}

	private void Update()
	{
		ChangeItemTimer();
	}

	private void ChangeItemTimer()
	{
		if (!m_isUsed)
		{
			m_switchTimer += Time.deltaTime;

			if (m_switchTimer >= m_switchItemTime)
			{
				m_switchTimer = 0;
				ChangeItem();
			}
		}
	}

	public virtual void ChangeItem()
	{
		m_currentItem = RandomItemSelection();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!m_isUsed)
		{
			if (CheckCollisionLayer(m_playerLayer, collision))
			{
				SetPickup(collision.gameObject.GetComponent<PlayerController>());

				m_itemIconRenderer.color = Color.clear;
				m_isUsed = true;

				//gameObject.SetActive(false);
			}
		}
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
}
