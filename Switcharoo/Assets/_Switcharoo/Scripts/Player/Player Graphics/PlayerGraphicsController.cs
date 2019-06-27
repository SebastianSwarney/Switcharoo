using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphicsController : MonoBehaviour
{
	[Header("Player Hit Properites")]
	public float m_invulnerableTime;
	public float m_movementControllLossTime;
	public Color m_hurtColor;

	private PlayerController m_player;
	private Animator m_animationController;
	[HideInInspector]
	public int m_currentType;
	private SpriteRenderer m_spriteRenderer;

    private ObjectPooler m_pooler;

    [Header("Particle Systems")]
    public GameObject m_landedParticle;
    public Vector3 m_landedParticleOffset;

	public GameObject m_shieldVisual;

	private void Start()
	{
		m_player = GetComponent<PlayerController>();
		m_animationController = GetComponentInChildren<Animator>();
		m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_pooler = ObjectPooler.instance;
	}

	private void Update()
	{
		m_animationController.SetBool("IsGrounded", m_player.controller.collisions.below);
		m_animationController.SetBool("IsMoving", (m_player.m_directionalInput.x != 0) ? true : false);
		m_spriteRenderer.flipX = m_player.controller.collisions.faceDir == 1 ? true : false;

		CheckPlayerInvunrable();
	}

	private void CheckPlayerInvunrable()
	{
		if (m_player.m_states.m_damageState == PlayerController.DamageState.Invulnerable && !m_player.m_usingMovementAbility)
		{
			m_shieldVisual.SetActive(true);
		}
		else
		{
			m_shieldVisual.SetActive(false);
		}
	}

	public void ResetPlayerGraphics()
	{
		if (m_player.m_playerDataAtRoomStart[0].m_currentRole == PlayerController.PlayerRole.Gunner && m_currentType == 1)
		{
			SwapAnimInt();
			m_animationController.SetTrigger("SwapTrigger");
		}
		else if (m_player.m_playerDataAtRoomStart[1].m_currentRole == PlayerController.PlayerRole.Gunner && m_currentType == 0)
		{
			SwapAnimInt();
			m_animationController.SetTrigger("SwapTrigger");
		}
	}

	public void SwapAnimInt()
	{
		if (m_currentType == 0)
		{
			m_currentType = 1;
		}
		else if (m_currentType == 1)
		{
			m_currentType = 0;
		}

		m_animationController.SetInteger("CurrentType", m_currentType);
	}

	public void TriggerTimeSlow(TimeSlowData p_timeSlowData)
	{
		StartCoroutine(SlowTime(p_timeSlowData.m_timeSlowAmount, p_timeSlowData.m_timeSlowLength));
	}

	private IEnumerator SlowTime(float p_slowAmount, float p_slowLength)
	{
		Time.timeScale = p_slowAmount;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		yield return new WaitForSecondsRealtime(p_slowLength);
		Time.fixedDeltaTime = 0.02f;
		Time.timeScale = 1f;
	}

	public void PlayerHitGraphicsTrigger()
	{
		StartCoroutine(PlayerHit());
	}

	IEnumerator PlayerHit()
	{
		float t = 0;

		m_player.m_states.m_damageState = PlayerController.DamageState.Invulnerable;
		m_player.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;
		m_player.m_velocity = Vector3.zero;

		while (t < m_invulnerableTime)
		{
			t += Time.deltaTime;

			float pingPongValue = Mathf.PingPong(Time.time * 5, 1);

			m_spriteRenderer.color = Color.Lerp(Color.white, m_hurtColor, pingPongValue);

			if (t >= m_movementControllLossTime)
			{
				m_player.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
			}

			yield return null;
		}

		m_player.m_states.m_damageState = PlayerController.DamageState.Vulnerable;
		m_spriteRenderer.color = Color.white;
	}


    #region Particle Systems
    public void LandedParticle()
    {
        m_pooler.NewObject(m_landedParticle, transform.position + m_landedParticleOffset, Quaternion.identity);   
    }
    #endregion
}
