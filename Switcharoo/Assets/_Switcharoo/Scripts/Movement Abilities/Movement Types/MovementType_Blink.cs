using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Blink")]
public class MovementType_Blink : MovementType_Base
{
	[Header("Blink Properties")]
	public float m_blinkDistance;
	public int m_blinkAmount;
	public float m_pauseBetweenBlinkTime;
	public AnimationCurve m_blinkCurve;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.StartCoroutine(UseBlink(p_playerRefrence, p_trailType));
	}

	IEnumerator UseBlink(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;

		int blinksUsed = 0;

		while (blinksUsed < m_blinkAmount)
		{
			p_trailType.UseTrail(p_playerRefrence, this);

			float t = 0;

			Vector3 initialPosition = p_playerRefrence.transform.position;
			Vector3 blinkTarget = (p_playerRefrence.m_aimDirection.normalized * m_blinkDistance) + p_playerRefrence.transform.position;

			while (t < m_movementTime)
			{
				t += Time.deltaTime;

				Debug.DrawLine(p_playerRefrence.transform.position, blinkTarget);

				float progress = m_blinkCurve.Evaluate(t / m_movementTime);

				Vector3 targetPosition = Vector3.Lerp(initialPosition, blinkTarget, progress);

				PhysicsSeekTo(p_playerRefrence, targetPosition);

				yield return null;
			}

			p_playerRefrence.m_velocity = Vector3.zero;

			blinksUsed++;

			yield return new WaitForSeconds(m_pauseBetweenBlinkTime);
		}

		p_playerRefrence.m_velocity = Vector3.zero;

		p_playerRefrence.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
	}
}
