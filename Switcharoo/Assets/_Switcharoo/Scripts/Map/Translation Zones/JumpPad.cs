using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : TranslationZone_Base, IPauseable
{
	public float m_jumpMultiplier;
    bool m_paused;


    private void OnCollisionEnter2D(Collision2D collision)
	{
        if (m_paused) return;

		if (CheckCollisionLayer(m_playerMask, collision.collider))
		{
			collision.gameObject.GetComponent<PlayerController>().JumpMaxVelocityMultiplied(m_jumpMultiplier);
		}
	}
    public void SetPauseState(bool p_isPaused)
    {
        m_paused = p_isPaused;
    }
}
