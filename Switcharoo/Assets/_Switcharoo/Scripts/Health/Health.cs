﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnEnemyHurt : UnityEvent { }


[System.Serializable]
public class OnEnemyFrozen : UnityEvent { }

[System.Serializable]
public class OnEffectActivate : UnityEvent { }

public class Health : MonoBehaviour
{
	public enum IceState { _0 , _25, _50, _75, _100 }
	public enum FireState { _0, _25, _50, _75, _100 }

	[Header("Health Properties")]
	public bool m_isDead;
	public float m_maxHealth;
	public float m_currentHealth;
	public float m_healthDropValue;
	public GameObject m_healthDropObject;

	private Rigidbody2D m_rigidbody;
	[HideInInspector]
	public bool m_canTakeDamage = true;

	[Header("Ice Properties")]
	public IceState m_currentIceState;

	[Header("Fire Properties")]
	public FireState m_currentFireState;
	public bool m_onFire;

	public FireData m_fireData;

	private int m_minFireHitsPerEffect;
	private int m_maxFireHitsPerEffect;

	private float m_maxFireEffectInterval;
	private float m_minFireEffectInterval;

	private float m_fireDamageAmount;

	private float m_currentFireInterval;
	private int m_currentEffectHitAmount;

    public OnEnemyHurt m_enemyHit = new OnEnemyHurt();
    public OnEnemyFrozen m_enemyFrozen = new OnEnemyFrozen();
    public OnEnemyFrozen m_enemyFrozenSound = new OnEnemyFrozen();
    private bool m_soundPlayed;

	public OnEffectActivate m_onFireActivate;
	public OnEffectActivate m_onFireDeactivate;

	public OnEffectActivate m_onIceActivate;
	public OnEffectActivate m_onIceDeactivate;

	public virtual void Start()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();
		ResetHealth();
		SetFireProperites();

		SetFireData(m_fireData);
	}

	private void SetFireData(FireData p_newData)
	{
		m_minFireHitsPerEffect = p_newData.m_minFireHitsPerEffect;
		m_maxFireHitsPerEffect = p_newData.m_maxFireHitsPerEffect;

		m_maxFireEffectInterval = p_newData.m_maxFireEffectInterval;
		m_minFireEffectInterval = p_newData.m_minFireEffectInterval;

		m_fireDamageAmount = p_newData.m_fireDamageAmount;
	}

	private void Update()
	{
		SetFireProperites();
		SetSelfOnFire();
	}
    private void LateUpdate()
    {
        SlowFromIce(); 
    }

    public void ResetHealth()
	{
		m_currentHealth = m_maxHealth;
		m_currentIceState = IceState._0;
		m_currentFireState = FireState._0;
        m_isDead = false;
        m_soundPlayed = false;

        m_onIceDeactivate.Invoke();
	}

	public void HealDamage(float p_healAmount)
	{
		m_currentHealth += p_healAmount;

		if (m_currentHealth > m_maxHealth)
		{
			m_currentHealth = m_maxHealth;
		}
	}

	public virtual void TakeDamage(float p_damage)
	{
		if (m_canTakeDamage)
		{
			m_currentHealth -= p_damage;
			CheckDamage();
            m_enemyHit.Invoke();
		}
	}

	private void CheckDamage()
	{
		if (m_currentHealth <= 0 && !m_isDead)
		{
			Die();
		}
	}

	public virtual void Die()
	{
		m_isDead = true;

        Pickup_Health healthDrop = ObjectPooler.instance.NewObject(m_healthDropObject, transform.position, Quaternion.identity).GetComponent<Pickup_Health>();

        if (healthDrop != null)
        {
            healthDrop.m_healthIncrease = m_healthDropValue;
        }
        
	}

	#region Fire Code
	private void SetSelfOnFire()
	{
		if (m_currentFireState != FireState._0 && !m_onFire)
		{
			StartCoroutine(FireEffect());
		}
	}

	IEnumerator FireEffect()
	{
		m_onFire = true;

		int amountOfEffects = 0;

		m_onFireActivate.Invoke();

		while (amountOfEffects < m_currentEffectHitAmount)
		{
			amountOfEffects++;

			if (m_currentFireState == FireState._100)
			{
				amountOfEffects = m_currentEffectHitAmount;
			}

			TakeDamage(m_fireDamageAmount);

			DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, 1);

			yield return new WaitForSeconds(m_currentFireInterval);
		}

		m_currentFireState = FireState._0;

		m_onFireDeactivate.Invoke();

		m_onFire = false;
	}

	private void SetFireProperites()
	{
		switch (m_currentFireState)
		{
			case FireState._25:

				m_currentFireInterval = m_maxFireEffectInterval;
				m_currentEffectHitAmount = m_minFireHitsPerEffect;

				break;

			case FireState._50:

				m_currentFireInterval = (m_maxFireEffectInterval + m_minFireEffectInterval) * 0.50f;
				m_currentEffectHitAmount = (m_maxFireHitsPerEffect + m_minFireHitsPerEffect) / 2;

				break;

			case FireState._75:

				m_currentFireInterval = m_minFireEffectInterval;
				m_currentEffectHitAmount = m_maxFireHitsPerEffect;

				break;
		}
	}

	public void SetFireState()
	{
		switch (m_currentFireState)
		{
			case FireState._0:

				m_currentFireState = FireState._25;

				return;

			case FireState._25:

				m_currentFireState = FireState._50;

				return;

			case FireState._50:

				m_currentFireState = FireState._75;

				return;

			case FireState._75:

				m_currentFireState = FireState._100;

				return;
		}
	}
	#endregion

	#region Ice Code
	private void SlowFromIce()
	{
        if (m_rigidbody == null) return;
		switch (m_currentIceState)
		{
			case IceState._25:

				m_rigidbody.velocity *= 0.75f;

				break;

			case IceState._50:

				m_rigidbody.velocity *= 0.50f;

				break;

			case IceState._75:

				m_rigidbody.velocity *= 0.25f;

				break;

			case IceState._100:

				m_rigidbody.velocity *= 0f;
                if (!m_soundPlayed)
                {
                    m_enemyFrozenSound.Invoke();
                    
                    m_soundPlayed = true;
                }
                m_enemyFrozen.Invoke();

                break;
		}
	}

	public void SetIceState()
	{
		switch (m_currentIceState)
		{
			case IceState._0:

				m_currentIceState = IceState._25;

				m_onIceActivate.Invoke();

				return;

			case IceState._25:

				m_currentIceState = IceState._50;

				return;

			case IceState._50:

				m_currentIceState = IceState._75;

				return;

			case IceState._75:

				m_currentIceState = IceState._100;

				return;
		}
	}
	#endregion
}
