using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Types/Double Jump")]
public class MovementType_DoubleJump : MovementType_Base
{
	public override void UseAbility(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		DoubleJump(p_playerRefrence, p_trailType);
	}

	private void DoubleJump(PlayerController p_playerRefrence, TrailType_Base p_trailType)
	{
		p_playerRefrence.JumpMaxVelocity();
	}
}
