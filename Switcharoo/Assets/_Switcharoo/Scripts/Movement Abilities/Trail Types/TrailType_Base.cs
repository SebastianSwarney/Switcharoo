using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrailType_Base : ScriptableObject
{
	public float m_trailDamageAmount;

	public abstract void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask);
}
