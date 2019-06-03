using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Jetpack")]
public class MovementType_Jetpack : MovementType_Base
{
	[Header("Jetpack Properties")]
	public float m_jetpackDistanceY;
	public float m_jetpackHoverTime;
	public AnimationCurve m_jetpackCurve;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType, PlayerBuff_Base p_buffType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(UseJetpack(p_playerRefrence, p_trailType, p_buffType, p_damageTargetMask, p_obstacleMask));
	}

	IEnumerator UseJetpack(PlayerController p_playerRefrence, TrailType_Base p_trailType, PlayerBuff_Base p_buffType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;
		p_playerRefrence.m_usingMovementAbility = true;

		float t1 = 0;

		Vector3 initialPosition = p_playerRefrence.transform.position;
		Vector3 jetpackTarget = new Vector3(initialPosition.x, initialPosition.y + m_jetpackDistanceY, initialPosition.z);

		p_trailType.UseTrail(p_playerRefrence, this, p_damageTargetMask, p_obstacleMask);

		while (t1 < m_movementTime)
		{
			t1 += Time.deltaTime;
			float progress = m_jetpackCurve.Evaluate(t1 / m_movementTime);
			Vector3 targetPosition = Vector3.Lerp(initialPosition, jetpackTarget, progress);
			PhysicsSeekTo(p_playerRefrence, targetPosition);

			yield return null;

			if (!p_playerRefrence.m_usingMovementAbility)
			{
				t1 = m_movementTime;
			}
		}

		float t2 = 0;

		while (t2 < m_jetpackHoverTime)
		{
			t2 += Time.deltaTime;
			p_playerRefrence.m_velocity = Vector3.zero;

			yield return null;

			if (!p_playerRefrence.m_usingMovementAbility)
			{
				t2 = m_jetpackHoverTime;
			}
		}

		p_buffType.UseBuff(p_playerRefrence, p_damageTargetMask, p_obstacleMask);

		p_playerRefrence.m_velocity = Vector3.zero;
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
		p_playerRefrence.m_usingMovementAbility = false;
	}

}
