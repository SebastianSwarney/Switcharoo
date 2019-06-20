using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : TranslationZone_Base, IActivatable, IPauseable
{



    public enum MoveDirection { Left, Right };
    public MoveDirection m_moveDir;
    public float m_moveSpeed;

    public int m_conveyorDimensions;


    [Header("Active Properties")]
    public bool m_startActive;
    bool m_isActive;
    bool m_isPaused;



    [Header("Preset Tiles")]
    public GameObject m_tilesLeft;
    public GameObject m_tilesCenter, m_tilesRight;
    public Color m_gizmosColor;
    public bool m_displayGizmos;
    List<Animator> m_tileAnimations = new List<Animator>();

    private void OnDrawGizmos()
    {
        if (!m_displayGizmos) return;

        Vector3 newPos = OriginPosition() + transform.position;
        Gizmos.color = m_gizmosColor;
        Gizmos.DrawCube(newPos, new Vector3(m_conveyorDimensions, 1f, 1f));



    }

    Vector3 OriginPosition()
    {
        Vector3 newPos = new Vector3(0f, 0f);
        if (m_conveyorDimensions % 2 == 0)
        {
            newPos = new Vector3(m_conveyorDimensions - ((m_conveyorDimensions / 2) + .5f), newPos.y, 0);
        }
        else
        {
            newPos = new Vector3(m_conveyorDimensions / 2, newPos.y, 0);
        }


        return newPos;
    }

    private void Start()
    {

        ObjectPooler.instance.AddObjectToPooler(gameObject);
        m_isActive = m_startActive;

        BoxCollider2D col = GetComponent<BoxCollider2D>();

        col.offset = OriginPosition();

        col.size = new Vector2(m_conveyorDimensions, 1f);
        m_displayGizmos = false;


        for (int x = 0; x < m_conveyorDimensions; x++)
        {
            GameObject platformType = null;
            if (x == 0)
            {
                platformType = (m_moveDir == MoveDirection.Left) ? m_tilesLeft : m_tilesRight;
            }
            else if (x == m_conveyorDimensions - 1)
            {
                platformType = (m_moveDir == MoveDirection.Left) ? m_tilesRight : m_tilesLeft;
            }
            else
            {
                platformType = m_tilesCenter;
            }

            GameObject conveyor = ObjectPooler.instance.NewObject(platformType, new Vector3(transform.position.x + x, transform.position.y, 0f), Quaternion.identity);
            conveyor.transform.localScale = new Vector3((m_moveDir == MoveDirection.Left) ? 1 : -1, transform.localScale.y, 1f);
            conveyor.transform.parent = this.transform;
            Animator newAnimator = conveyor.GetComponent<Animator>();
            newAnimator.enabled = m_startActive;
            m_tileAnimations.Add(newAnimator);



        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (m_isPaused) return;
        if (m_isActive)
        {
            if (CheckCollisionLayer(m_playerMask, collision.collider))
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                player.m_velocity += transform.right * m_moveSpeed * ((m_moveDir == MoveDirection.Left) ? -1 : 1);
            }
        }
    }

    #region IActivatable
    public void ActiveState(bool p_active)
    {
        m_isActive = p_active;
        foreach (Animator anim in m_tileAnimations)
        {
            anim.enabled = p_active;
        }
    }

    public void ResetMe()
    {
        m_isActive = !m_startActive;
        foreach (Animator anim in m_tileAnimations)
        {
            anim.enabled = m_startActive;
        }
    }


    #endregion

    public void SetPauseState(bool p_isPaused)
    {
        m_isPaused = p_isPaused;

    }
}
