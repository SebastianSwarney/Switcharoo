﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnActivationEvent : UnityEvent { }
public abstract class Pickup_Base : MonoBehaviour, IActivatable
{
    public OnActivationEvent m_playerCollided = new OnActivationEvent();
    [Space(10)]
	public LayerMask m_playerLayer;
    public float m_regenerationTime = 30f;
    Coroutine m_regenCoroutine;

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
                m_playerCollided.Invoke();
                m_regenCoroutine = StartCoroutine(RegenerateObject());
                SetPickup(collision.gameObject.GetComponent<PlayerController>());

				m_itemIconRenderer.color = Color.clear;
				m_isUsed = true;

				
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

    public void ActiveState(bool p_active)
    {
        m_itemIconRenderer.color = (p_active) ? Color.white : Color.clear;
        m_isUsed = !p_active;
    }

    public void ResetMe()
    {
        if (m_regenCoroutine != null)
        {
            StopCoroutine(m_regenCoroutine);
        }
        m_itemIconRenderer.color = Color.white;
        m_isUsed = false;
    }

    IEnumerator RegenerateObject()
    {
        yield return new WaitForSeconds(m_regenerationTime);
        ActiveState(true);
        m_regenCoroutine = null;
    }
}
