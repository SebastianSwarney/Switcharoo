using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationFunctions : MonoBehaviour
{
	private PlayerController m_player;

	private void Start()
	{
		m_player = GetComponentInParent<PlayerController>();
	}

	public void AnimationJump()
	{
		m_player.JumpMaxVelocity();
	}

	public void AnimationDisableSwapping()
	{
		m_player.DisableSwapping();
	}

	public void AnimationEnableSwapping()
	{
		m_player.EnableSwapping();
	}
}
