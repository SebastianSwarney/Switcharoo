using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphicsController : MonoBehaviour
{
	[Header("Player Hit Properites")]
	public float m_invulnerableTime;
	public float m_movementControllLossTime;

	private PlayerController m_player;

	private void Start()
	{
		m_player = GetComponent<PlayerController>();
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

			if (t >= m_movementControllLossTime)
			{
				m_player.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
			}

			yield return null;
		}

		m_player.m_states.m_damageState = PlayerController.DamageState.Vulnerable;
	}
}
