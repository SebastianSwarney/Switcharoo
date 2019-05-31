using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Double Jump")]
public class MovementType_DoubleJump : MovementType_Base
{
	public float m_jumpMultiplier;

	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		DoubleJump(p_playerRefrence, p_trailType, p_damageTargetMask, p_obstacleMask);
	}

	private void DoubleJump(PlayerController p_playerRefrence, TrailType_Base p_trailType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.JumpMaxVelocityMultiplied(m_jumpMultiplier);
		p_trailType.UseTrail(p_playerRefrence, this, p_damageTargetMask, p_obstacleMask);
	}
}
