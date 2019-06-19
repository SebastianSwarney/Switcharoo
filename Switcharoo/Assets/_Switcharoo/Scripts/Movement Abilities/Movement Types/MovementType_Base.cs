using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementType_Base : ScriptableObject
{
	public Sprite m_uiSprite;
	public float m_movementTime;
	public float m_amountOfTrailsToSpawn;
	public int m_ammoCount;

	public abstract void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType, PlayerBuff_Base p_buffType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask);

	public void PhysicsSeekTo(PlayerController p_playerRefrence, Vector3 p_targetPosition)
	{
		Vector3 deltaPosition = p_targetPosition - p_playerRefrence.transform.position;
		p_playerRefrence.m_velocity = deltaPosition / Time.deltaTime;
	}
}
