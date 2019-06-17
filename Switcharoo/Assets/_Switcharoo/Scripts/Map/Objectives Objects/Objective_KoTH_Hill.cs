using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_KoTH_Hill : MonoBehaviour, IPauseable
{
    public float m_targetTime;
    float m_currentTime;
    public Vector2 m_hillDimensions;
    public PlayerController.PlayerType m_targetPlayer;
    public bool m_eitherPlayer = false;
    PlayerController m_player;
    


    [HideInInspector]
    public bool m_hillComplete;

    [Header("Visuals")]
    public List<SpriteRenderer> m_bars;
    public float m_targetYScale;
    bool m_isPaused;
    

    [Header("Preset Values")]
    public LayerMask m_playerLayer;
    public Color m_robotColor, m_alienColor, m_eitherColor;
    public SpriteRenderer m_hillFieldRender;

    public Color m_robotBarColor, m_alienBarColor, m_neutralBarColor;
    public Color m_robotBarColorComplete, m_alienBarColorComplete, m_neutralBarColorComplete;

    private void Start()
    {
        if (m_hillFieldRender != null)
        {
            m_hillFieldRender.color = (m_targetPlayer == PlayerController.PlayerType.Type0) ? m_robotColor : m_alienColor;
            m_hillFieldRender.color = (m_eitherPlayer) ? m_eitherColor : m_hillFieldRender.color;
            m_hillFieldRender.transform.localScale = new Vector2(m_hillDimensions.x - 2, m_hillDimensions.y);
        }
        foreach(SpriteRenderer sRend in m_bars)
        {
            sRend.color = (m_targetPlayer == PlayerController.PlayerType.Type0) ? m_robotBarColor : m_alienBarColor;
            sRend.color = (m_eitherPlayer) ? m_neutralBarColor : sRend.color;
        }


    }

    private void Update()
    {

        Collider2D playerCol = Physics2D.OverlapBox(transform.position, m_hillDimensions, 0, m_playerLayer);
        if (playerCol != null)
        {
            m_player = playerCol.gameObject.GetComponent<PlayerController>();
        }
        else
        {
            m_player = null;
        }
        if (m_player != null && m_currentTime <= m_targetTime)
        {
            if (!m_isPaused)
            {
                AddTime();
                
            }
            
        }
    }
    void AddTime()
    {

        if (m_player.m_players[(m_targetPlayer == PlayerController.PlayerType.Type0) ? 0 : 1].m_currentRole == PlayerController.PlayerRole.Gunner || m_eitherPlayer)
        {
            m_currentTime += Time.deltaTime;

            foreach (SpriteRenderer bar in m_bars)
            {
                bar.transform.localScale = new Vector3(bar.transform.localScale.x, Mathf.Lerp(0, m_targetYScale, (m_currentTime / m_targetTime)), bar.transform.localScale.z);
            }
            if (m_currentTime >= m_targetTime)
            {
                m_hillComplete = true;
                //this.gameObject.SetActive(false);
                StartCoroutine(ColorChangeDelay());


            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (m_targetPlayer == PlayerController.PlayerType.Type0) ? m_robotColor : m_alienColor;
        if (m_eitherPlayer) Gizmos.color = m_eitherColor;
        Gizmos.DrawCube(transform.position, m_hillDimensions);
    }



    public void ResetHill()
    {
        m_currentTime = 0;
        this.gameObject.SetActive(true);
        m_hillComplete = false;
    }


    public void SetPauseState(bool p_isPaused)
    {
        m_isPaused = p_isPaused;
    }

    IEnumerator ColorChangeDelay()
    {
        yield return new WaitForSeconds(1f);
        foreach (SpriteRenderer bar in m_bars)
        {
            bar.color = (m_targetPlayer == PlayerController.PlayerType.Type0) ? m_robotBarColorComplete : m_alienBarColorComplete;
            bar.color = (m_eitherPlayer) ? m_neutralBarColorComplete : bar.color;
        }
    }
}
