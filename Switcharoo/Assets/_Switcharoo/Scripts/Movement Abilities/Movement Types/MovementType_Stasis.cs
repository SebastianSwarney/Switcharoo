using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Stasis")]
public class MovementType_Stasis : MovementType_Base
{
	[Header("Stasis Properties")]
	public float m_stasisRadius;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.StartCoroutine(UseStasis(p_playerRefrence, p_trailType));
	}

	private IEnumerator UseStasis(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		List<Collider2D> castObjects = new List<Collider2D>();

		Vector3 stasisOrigin = p_playerRefrence.transform.position;

		float t = 0;

		p_trailType.UseTrail(p_playerRefrence, this);

		while (t < m_movementTime)
		{
			t += Time.deltaTime;

			DebugExtension.DebugCircle(stasisOrigin, Vector3.forward, Color.blue, m_stasisRadius);

			Collider2D[] colliders = Physics2D.OverlapCircleAll(stasisOrigin, m_stasisRadius);

			castObjects.AddRange(colliders);

			if (castObjects.Contains(p_playerRefrence.controller.col))
			{
				p_playerRefrence.m_states.m_gravityControllState = PlayerController.GravityState.GravityDisabled;
			}
			else
			{
				p_playerRefrence.m_states.m_gravityControllState = PlayerController.GravityState.GravityEnabled;
			}

			castObjects.Clear();

			yield return null;
		}

		p_playerRefrence.m_states.m_gravityControllState = PlayerController.GravityState.GravityEnabled;
	}
}
