using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Grapple")]
public class MovementType_Grapple : MovementType_Base
{
	[Header("Grapple Properties")]
	public AnimationCurve m_grappleCurve;
	public LayerMask m_grappleMask;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.StartCoroutine(UseGrapple(p_playerRefrence, p_trailType));
	}

	IEnumerator UseGrapple(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;

		float t = 0;

		Vector3 initialPosition = p_playerRefrence.transform.position;

		RaycastHit2D hit = Physics2D.Raycast(initialPosition, p_playerRefrence.m_aimDirection, Mathf.Infinity, m_grappleMask);

		Vector3 grappleTarget = hit.point;

		p_trailType.UseTrail(p_playerRefrence, this);

		while (t < m_movementTime)
		{
			t += Time.deltaTime;

			float progress = m_grappleCurve.Evaluate(t / m_movementTime);

			Vector3 targetPosition = Vector3.Lerp(initialPosition, grappleTarget, progress);

			PhysicsSeekTo(p_playerRefrence, targetPosition);

			yield return null;

			if (p_playerRefrence.controller.collisions.left || p_playerRefrence.controller.collisions.right || p_playerRefrence.controller.collisions.above || p_playerRefrence.controller.collisions.below)
			{
				t = m_movementTime;
			}
		}

		p_playerRefrence.m_velocity = Vector3.zero;

		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
	}
}
