using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
	public float m_moveSpeed;

	public LayerMask m_damageTargetMask;
	public LayerMask m_obstacleMask;

	[SerializeField]
	private float m_timeUntillDeactivate = 60;
	private float m_deactivateTimer;

	public DamageType_Base m_damageType;

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

	public virtual void InitializeParameters(DamageType_Base p_damageType)
	{
		m_damageType = p_damageType;
	}
}
