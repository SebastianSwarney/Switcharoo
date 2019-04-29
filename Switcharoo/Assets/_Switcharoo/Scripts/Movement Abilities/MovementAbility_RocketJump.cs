using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Abilities/Rocket Jump")]
public class MovementAbility_RocketJump : MovementAbility_Base
{
	//The rocket jump could possibly be done with one animation curve but there is more controll with two so im leaving it as is for now but I might want to revisit this

	[Header("Rocket Jump Startup Properties")]
	public float m_rocketJumpStartupTime;
	public float m_rocketJumpStartupHeight;
	public AnimationCurve m_rocketJumpStartupCurve;
	public float m_rocketJumpPauseTime;

	[Header("Rocket Jump Properties")]
	public float m_rocketJumpTime;
	public float m_rocketJumpHeight;
	public AnimationCurve m_rocketJumpCurve;

	public override void UseAbility(PlayerController p_playerRefrence)
	{
		p_playerRefrence.StartCoroutine(UseRocketJump(p_playerRefrence));
	}

	IEnumerator UseRocketJump(PlayerController p_playerRefrence)
	{
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;

		Vector3 initialPosition = p_playerRefrence.transform.position;

		Vector3 startupTarget = new Vector3(p_playerRefrence.transform.position.x, p_playerRefrence.transform.position.y + m_rocketJumpStartupHeight, p_playerRefrence.transform.position.z);

		float t1 = 0;

		while (t1 < m_rocketJumpStartupTime)
		{
			t1 += Time.deltaTime;

			float progress = m_rocketJumpStartupCurve.Evaluate(t1 / m_rocketJumpStartupTime);

			Vector3 targetPosition = Vector3.Lerp(initialPosition, startupTarget, progress);

			PhysicsSeekTo(p_playerRefrence, targetPosition);

			yield return null;
		}

		yield return new WaitForSeconds(m_rocketJumpPauseTime);

		Vector3 rocketJumpTarget = new Vector3(p_playerRefrence.transform.position.x, p_playerRefrence.transform.position.y + m_rocketJumpHeight, p_playerRefrence.transform.position.z);

		float t2 = 0;

		while (t2 < m_rocketJumpTime)
		{
			t2 += Time.deltaTime;

			float progress = m_rocketJumpCurve.Evaluate(t2 / m_rocketJumpTime);

			Vector3 targetPosition = Vector3.Lerp(startupTarget, rocketJumpTarget, progress);

			PhysicsSeekTo(p_playerRefrence, targetPosition);

			yield return null;
		}

		p_playerRefrence.m_velocity = Vector3.zero;

		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
	}
}
