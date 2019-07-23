using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Homing : Bullet_Base
{
	[Header("Homing Properties")]
	public float m_rotateSpeed = 100f;
	private Transform m_target;

    [Header("Particle systems")]
    public GameObject m_trailParticle;
    GameObject m_currentTrail;
    ParticleSelfDestruct m_selfDestruct;
    public Transform m_trailEmitterPosition;

    ParticleSystem.MainModule m_particleMain;
    float m_currentTime;

    public override void OnEnable()
    {
        base.OnEnable();
        if (m_currentTrail == null && m_trailParticle != null)
        {
            m_currentTrail = ObjectPooler.instance.NewObject(m_trailParticle, m_trailEmitterPosition.position, Quaternion.identity,false);
            m_particleMain = m_currentTrail.GetComponent<ParticleSystem>().main;
            m_selfDestruct = m_currentTrail.GetComponent<ParticleSelfDestruct>();
            m_selfDestruct.m_destructTime = Mathf.Infinity;
            m_particleMain.duration = Mathf.Infinity;
            m_currentTime = 0;
            m_currentTrail.SetActive(true);
        }
    }
    public override void Update()
	{
		base.Update();

		if (m_target != null)
		{
			MoveToTarget();

			if (m_target.gameObject.activeSelf == false)
			{
				FindTarget();
			}
		}
		else
		{
			MoveForward();
		}

        if (m_currentTrail != null)
        {
            m_currentTime += Time.deltaTime;
            m_currentTrail.transform.position = m_trailEmitterPosition.position;
        }
        
    }

	public override void InitializeParameters(DamageType_Base p_damageType, float p_moveSpeed, float p_damageAmount, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		base.InitializeParameters(p_damageType, p_moveSpeed, p_damageAmount, p_damageTargetMask, p_obstacleMask);
		FindTarget();
	}

	public override void InitializeParameters(DamageType_Base p_damageType, float p_moveSpeed, float p_damageAmount, LayerMask p_damageTargetMask, LayerMask p_obstacleMask, PlayerController.PlayerType m_playerType)
	{
		base.InitializeParameters(p_damageType, p_moveSpeed, p_damageAmount, p_damageTargetMask, p_obstacleMask, m_playerType);
		FindTarget();
	}

	private void FindTarget()
	{
		Collider2D collider = Physics2D.OverlapCircle(transform.position, 100, m_damageTargetMask); //May need to be made a variable range

		if (collider != null)
		{
			m_target = collider.transform;
		}
	}

	private void MoveForward()
	{
		m_rigidbody.velocity = transform.right * m_moveSpeed;
	}

	public void MoveToTarget()
	{
		Vector2 direction = m_target.position - transform.position;
		direction.Normalize();
		float rotateAmount = Vector3.Cross(direction, transform.right).z;
		m_rigidbody.angularVelocity = -rotateAmount * m_rotateSpeed;
		m_rigidbody.velocity = transform.right * m_moveSpeed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		m_damageType.OnContact(this, collision, m_bulletDamageAmount, m_obstacleMask, m_damageTargetMask);

        if (m_currentTrail != null)
        {
            m_particleMain.duration = m_currentTime;
            m_currentTrail = null;
            m_selfDestruct.m_destructTime = m_currentTime + 1f;
        }

    }
}
