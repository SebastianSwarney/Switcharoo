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
	public float m_chargeDamage;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType, PlayerBuff_Base p_buffType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(UseCharge(p_playerRefrence, p_trailType, p_buffType, p_damageTargetMask, p_obstacleMask));
	}

	IEnumerator UseCharge(PlayerController p_playerRefrence, TrailType_Base p_trailType, PlayerBuff_Base p_buffType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;
		p_playerRefrence.m_usingMovementAbility = true;

		p_playerRefrence.m_movementAbilityUsed.Invoke();

		float t = 0;

		Vector3 initialPosition = p_playerRefrence.transform.position;
		Vector3 chargeTarget = new Vector3(initialPosition.x + (m_chargeDistanceX * Mathf.Sign(p_playerRefrence.m_runnerAimInput.x)), initialPosition.y, initialPosition.z);

		p_trailType.UseTrail(p_playerRefrence, this, p_damageTargetMask, p_obstacleMask);

		while (t < m_movementTime)
		{
			t += Time.deltaTime;
			float progress = m_chargeCurve.Evaluate(t / m_movementTime);
			Vector3 targetPosition = Vector3.Lerp(initialPosition, chargeTarget, progress);
			PhysicsSeekTo(p_playerRefrence, targetPosition);

			float hitBoxTargetX = (p_playerRefrence.controller.collisions.faceDir > 0) ? p_playerRefrence.transform.position.x + (p_playerRefrence.controller.col.bounds.size.x / 2) + m_chargeHitBoxSize.x / 2 : p_playerRefrence.transform.position.x - (p_playerRefrence.controller.col.bounds.size.x / 2) - m_chargeHitBoxSize.x / 2;
			Vector2 hitBoxTargetPosition = new Vector2(hitBoxTargetX, p_playerRefrence.transform.position.y);

			Collider2D[] colliders = Physics2D.OverlapBoxAll(hitBoxTargetPosition, m_chargeHitBoxSize, 0f, p_damageTargetMask);

			foreach (Collider2D collider in colliders)
			{
				collider.GetComponent<Health>().TakeDamage(m_chargeDamage);
			}

			DebugExtension.DebugBounds(new Bounds(hitBoxTargetPosition, m_chargeHitBoxSize));

			yield return null;

			if (!p_playerRefrence.m_usingMovementAbility)
			{
				t = m_movementTime;
			}
		}

		p_buffType.UseBuff(p_playerRefrence, p_damageTargetMask, p_obstacleMask);

		p_playerRefrence.m_velocity = Vector3.zero;
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
		p_playerRefrence.m_usingMovementAbility = false;
	}
}
