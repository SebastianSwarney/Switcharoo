using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityZone : TranslationZone_Base
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckCollisionLayer(m_playerMask, collision.collider))
		{
			collision.gameObject.GetComponent<PlayerController>().m_states.m_gravityControllState = PlayerController.GravityState.GravityDisabled;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (CheckCollisionLayer(m_playerMask, collision.collider))
		{
			collision.gameObject.GetComponent<PlayerController>().m_states.m_gravityControllState = PlayerController.GravityState.GravityEnabled;
		}
	}
}
