using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour, IPauseable
{
	[Header("Movement Properties")]
	public float m_moveSpeedMultiplier = 1;
	[HideInInspector]
	public float m_moveSpeed;

	[Header("Collision Properties")]
	public LayerMask m_damageTargetMask;
	public LayerMask m_obstacleMask;

	[Header("Deactivate Properties")]
	private float m_timeUntillDeactivate = 60;
	private float m_deactivateTimer;

	[HideInInspector]
	public DamageType_Base m_damageType;
	[HideInInspector]
	public float m_bulletDamageAmount;
	[HideInInspector]
	public Rigidbody2D m_rigidbody;

	public virtual void OnEnable()
	{
		m_deactivateTimer = 0;
		m_rigidbody = GetComponent<Rigidbody2D>();
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

	public virtual void InitializeParameters(DamageType_Base p_damageType, float p_moveSpeed, float p_damageAmount, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		m_moveSpeed = p_moveSpeed * m_moveSpeedMultiplier;
		m_damageType = p_damageType;
		m_bulletDamageAmount = p_damageAmount;
		m_damageTargetMask = p_damageTargetMask;
		m_obstacleMask = p_obstacleMask;
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
		if (p_isPaused)
		{
			m_rigidbody.simulated = false;
		}
		else
		{
			m_rigidbody.simulated = true;
		}
	}
}
