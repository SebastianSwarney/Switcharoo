using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public enum IceState { _0 , _25, _50, _75, _100 }
	public enum FireState { _0, _25, _50, _75, _100 }

	[Header("Health Properties")]
	public bool m_isDead;
	public float m_maxHealth;

	[SerializeField]
	private float m_currentHealth;
	private Rigidbody2D m_rigidbody;

	[Header("Ice Properties")]
	public IceState m_currentIceState;

	[Header("Fire Properties")]
	public FireState m_currentFireState;
	public bool m_onFire;

	public int m_minFireHitsPerEffect;
	public int m_maxFireHitsPerEffect;

	public float m_maxFireEffectInterval;
	public float m_minFireEffectInterval;

	public float m_fireDamageAmount;

	private float m_currentFireInterval;
	private int m_currentEffectHitAmount;

	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();

		ResetHealth();

		SetFireProperites();
	}

	private void Update()
	{
		m_rigidbody.velocity = Vector2.down * 3;

		SlowFromIce();
		SetFireProperites();
		SetSelfOnFire();
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

		while (amountOfEffects < m_currentEffectHitAmount)
		{
			amountOfEffects++;

			if (m_currentFireState == FireState._100)
			{
				Debug.Log("I exploaded");

				amountOfEffects = m_currentEffectHitAmount;
			}

			TakeDamage(m_fireDamageAmount);

			DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, 1);

			yield return new WaitForSeconds(m_currentFireInterval);
		}

		m_currentFireState = FireState._0;

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

				break;
		}
	}

	public void SetIceState()
	{
		switch (m_currentIceState)
		{
			case IceState._0:

				m_currentIceState = IceState._25;

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
