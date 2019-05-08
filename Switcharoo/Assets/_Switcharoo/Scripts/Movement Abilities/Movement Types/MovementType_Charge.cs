using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Charge")]
public class MovementType_Charge : MovementType_Base
{
	[Header("Charge Properties")]
	public AnimationCurve m_chargeCurve;
	public float m_chargeDistanceX;
	public Vector2 m_chargeHitBoxSize;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.StartCoroutine(UseCharge(p_playerRefrence, p_trailType));
	}

	IEnumerator UseCharge(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;

		float t = 0;

		Vector3 initialPosition = p_playerRefrence.transform.position;

		Vector3 chargeTarget = new Vector3(initialPosition.x + (m_chargeDistanceX * Mathf.Sign(p_playerRefrence.m_aimDirection.x)), initialPosition.y, initialPosition.z);

		p_trailType.UseTrail(p_playerRefrence, this);

		while (t < m_movementTime)
		{
			t += Time.deltaTime;

			float progress = m_chargeCurve.Evaluate(t / m_movementTime);

			Vector3 targetPosition = Vector3.Lerp(initialPosition, chargeTarget, progress);

			PhysicsSeekTo(p_playerRefrence, targetPosition);

			float hitBoxTargetX = (p_playerRefrence.controller.collisions.faceDir > 0) ? p_playerRefrence.transform.position.x + (p_playerRefrence.controller.col.bounds.size.x / 2) + m_chargeHitBoxSize.x / 2 : p_playerRefrence.transform.position.x - (p_playerRefrence.controller.col.bounds.size.x / 2) - m_chargeHitBoxSize.x / 2;

			Vector2 hitBoxTargetPosition = new Vector2(hitBoxTargetX, p_playerRefrence.transform.position.y);

			Collider2D[] colliders = Physics2D.OverlapBoxAll(hitBoxTargetPosition, m_chargeHitBoxSize, 0f);

			DebugExtension.DebugBounds(new Bounds(hitBoxTargetPosition, m_chargeHitBoxSize));

			yield return null;
		}

		p_playerRefrence.m_velocity = Vector3.zero;

		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
	}
}
