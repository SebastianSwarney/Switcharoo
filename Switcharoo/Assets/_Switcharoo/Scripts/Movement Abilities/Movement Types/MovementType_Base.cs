﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementType_Base : ScriptableObject
{
	public float m_movementTime;

	public abstract void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType);

	public void PhysicsSeekTo(PlayerController p_playerRefrence, Vector3 p_targetPosition)
	{
		Vector3 deltaPosition = p_targetPosition - p_playerRefrence.transform.position;
		p_playerRefrence.m_velocity = deltaPosition / Time.deltaTime;
	}
}