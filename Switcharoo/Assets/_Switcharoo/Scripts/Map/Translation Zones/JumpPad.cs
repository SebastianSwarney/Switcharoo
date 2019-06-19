using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
	public float m_jumpMultiplier;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		collision.gameObject.GetComponent<PlayerController>().JumpMaxVelocityMultiplied(m_jumpMultiplier);
	}
}
