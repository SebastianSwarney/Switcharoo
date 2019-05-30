using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_KoTH_Hill : MonoBehaviour
{
    public float m_targetTime;
    float m_currentTime;
    
    public PlayerController.PlayerType m_targetPlayer;
    public bool m_eitherPlayer = false;
    PlayerController m_player;
    public string m_playerTag = "Player";

    public bool m_hillComplete;
    private void Update()
    {
        if (m_player != null)
        {
            AddTime();
        }
    }
    void AddTime()
    {

        if (m_player.m_players[(m_targetPlayer == PlayerController.PlayerType.Type0) ? 0 : 1].m_currentRole == PlayerController.PlayerRole.Runner || m_eitherPlayer)
        {
            m_currentTime += Time.deltaTime;
            if (m_currentTime >= m_targetTime)
            {
                m_hillComplete = true;
                this.gameObject.SetActive(false);
                
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_player != null) return;
        if (other.gameObject.tag == m_playerTag)
        {
            m_player = other.gameObject.GetComponent<PlayerController>();
        }
    }

    public void ResetHill()
    {
        m_currentTime = 0;
        this.gameObject.SetActive(true);
        m_hillComplete = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_player == null) return;
        if (collision.gameObject.tag == m_playerTag)
        {
            m_player = null;
        }
    }
}
