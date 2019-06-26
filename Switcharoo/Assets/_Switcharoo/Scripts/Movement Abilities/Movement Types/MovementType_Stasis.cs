using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Stasis")]
public class MovementType_Stasis : MovementType_Base
{
	[Header("Stasis Properties")]
	public float m_stasisRadius;

	public GameObject m_stasisVisual;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType, PlayerBuff_Base p_buffType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(UseStasis(p_playerRefrence, p_trailType, p_buffType, p_damageTargetMask, p_obstacleMask));
	}

	private IEnumerator UseStasis(PlayerController p_playerRefrence, TrailType_Base p_trailType, PlayerBuff_Base p_buffType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.m_usingMovementAbility = true;

		List<Collider2D> castObjects = new List<Collider2D>();
		Vector3 stasisOrigin = p_playerRefrence.transform.position;

		GameObject newVisual = ObjectPooler.instance.NewObject(m_stasisVisual, stasisOrigin, Quaternion.identity);

		newVisual.transform.localScale = new Vector3(m_stasisRadius * 2, m_stasisRadius * 2, 1);

		float t = 0;

		p_trailType.UseTrail(p_playerRefrence, this, p_damageTargetMask, p_obstacleMask);

		while (t < m_movementTime)
		{
			t += Time.deltaTime;

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

			DebugExtension.DebugCircle(stasisOrigin, Vector3.forward, Color.blue, m_stasisRadius);

			yield return null;

			if (!p_playerRefrence.m_usingMovementAbility)
			{
				t = m_movementTime;
			}
		}

		p_buffType.UseBuff(p_playerRefrence, p_damageTargetMask, p_obstacleMask);

		ObjectPooler.instance.ReturnToPool(newVisual);

		p_playerRefrence.m_states.m_gravityControllState = PlayerController.GravityState.GravityEnabled;
		p_playerRefrence.m_usingMovementAbility = false;
	}
}
