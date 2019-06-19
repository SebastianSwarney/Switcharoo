using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityZone : TranslationZone_Base
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CheckCollisionLayer(m_playerMask, collision))
		{
			collision.gameObject.GetComponent<PlayerController>().m_states.m_gravityControllState = PlayerController.GravityState.GravityDisabled;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (CheckCollisionLayer(m_playerMask, collision))
		{
			collision.gameObject.GetComponent<PlayerController>().m_states.m_gravityControllState = PlayerController.GravityState.GravityEnabled;
		}
	}
}
