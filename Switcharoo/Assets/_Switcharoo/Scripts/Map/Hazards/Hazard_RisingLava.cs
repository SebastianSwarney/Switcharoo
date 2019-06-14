using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_RisingLava : MonoBehaviour, IActivatable
{

    public List<LerpPoints> m_lerpPoints;
    int m_currentLerpPoint;
    float m_currentTime;
    Vector3 m_startPos;
    Vector3 m_currentLerpStartPos;
    bool m_lavaComplete;

    [Header("Activation settings")]
    public bool m_startActive;
    bool m_isActive;
    private void Start()
    {
        print("oof");
        m_startPos = transform.position;
        m_currentLerpStartPos = Vector3.zero ;
        m_isActive = m_startActive;

    }
    private void Update()
    {
        if (!m_isActive) return;
        if (!m_lavaComplete)
        {
            float percent = m_currentTime / m_lerpPoints[m_currentLerpPoint].m_timeToPoint;
            m_currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(m_currentLerpStartPos + m_startPos, m_lerpPoints[m_currentLerpPoint].m_lerpPoint + m_startPos, percent);
            if (percent >= 1)
            {
                if (m_currentLerpPoint + 1 < m_lerpPoints.Count)
                {
                    m_currentLerpStartPos = m_lerpPoints[m_currentLerpPoint].m_lerpPoint;
                    m_currentLerpPoint += 1;
                    m_currentTime = 0;
                    
                }
                else
                {
                    m_lavaComplete = true;
                }
            }
        }


    }

    private void OnDrawGizmos()
    {
        
        for (int i = 0; i < m_lerpPoints.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_lerpPoints[i].m_lerpPoint + ((Application.isPlaying) ? m_startPos : transform.position), .5f);
            if (i > 0)
            {
                Debug.DrawLine(m_lerpPoints[i-1].m_lerpPoint + ((Application.isPlaying) ? m_startPos : transform.position), (Application.isPlaying) ? m_lerpPoints[i].m_lerpPoint + m_startPos : m_lerpPoints[i].m_lerpPoint + transform.position);
            }
            else
            {
                
                Debug.DrawLine((Application.isPlaying) ? m_startPos : transform.position, m_lerpPoints[i].m_lerpPoint + ((Application.isPlaying) ? m_startPos : transform.position));
            }
        }
    }

    public void ActiveState(bool p_active)
    {
        m_isActive = p_active;
    }

    public void ResetMe()
    {
        m_isActive = m_startActive;
        m_currentLerpPoint = 0;
        m_currentTime = 0;
        m_currentLerpStartPos = Vector3.zero;
        transform.position = m_currentLerpStartPos;
        m_lavaComplete = false;
    }

    [System.Serializable]
    public struct LerpPoints
    {
        public Vector3 m_lerpPoint;
        public float m_timeToPoint;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Health health = other.gameObject.GetComponent<Health_Player>();
            health.m_isDead = true;
        }
    }
}

