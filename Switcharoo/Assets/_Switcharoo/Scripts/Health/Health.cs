using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public bool m_isDead;
	public float m_maxHealth;
	public float m_currentHealth;

	private void Start()
	{
		ResetHealth();
	}

	public void ResetHealth()
	{
		m_currentHealth = m_maxHealth;
	}

	public void HealDamage(float p_healAmount)
	{
		m_currentHealth += p_healAmount;

		if (m_currentHealth > m_maxHealth)
		{
			m_currentHealth = m_maxHealth;
		}
	}

	public void TakeDamage(float p_damage)
	{
		m_currentHealth -= p_damage;

		CheckDamage();
	}

	private void CheckDamage()
	{
		if (m_currentHealth <= 0)
		{
			m_isDead = true;
		}
	}

}
