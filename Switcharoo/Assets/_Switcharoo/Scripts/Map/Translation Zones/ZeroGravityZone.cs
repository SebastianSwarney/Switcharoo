using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityZone : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		collision.gameObject.GetComponent<PlayerController>().m_states.m_gravityControllState = PlayerController.GravityState.GravityDisabled;
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		collision.gameObject.GetComponent<PlayerController>().m_states.m_gravityControllState = PlayerController.GravityState.GravityEnabled;
	}
}
