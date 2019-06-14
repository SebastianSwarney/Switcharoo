using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : TranslationZone_Base, IActivatable, IPauseable
{
    public float m_moveSpeed;

    [Header("Active Properties")]
    public bool m_startActive;
    bool m_isActive;
    bool m_isPaused;


    private void Start()
    {
        ObjectPooler.instance.AddObjectToPooler(gameObject);
        m_isActive = m_startActive;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (m_isPaused) return;
        if (m_isActive)
        {
			if (CheckCollisionLayer(m_playerMask, collision.collider))
			{
				PlayerController player = collision.gameObject.GetComponent<PlayerController>();
				player.m_velocity += transform.right * m_moveSpeed;
			}
        }
    }

    #region IActivatable
    public void ActiveState(bool p_active)
    {
        m_isActive = p_active;
    }

    public void ResetMe()
    {
        m_isActive = !m_startActive;
    }


    #endregion

    public void SetPauseState(bool p_isPaused)
    {
        m_isPaused = p_isPaused;
    }
}
