using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
    /*  
    This spawner manager will allow the spawners in the room to start 
    spawning only after a set amount of time, through a timer (coroutine) 
    */
///<Summary>


public class AI_Spawner_Manager_TimedStart : AI_Spawner_Manager_Base
{
    public float m_timeToStartSpawn;

    Coroutine m_delayedSpawn;



    [Header("Trigger Properties")]
    public string m_playerTag = "Player";
    public bool m_isTrigger = false;
    Collider2D[] cols;
    
    public override void DeintializeAllSpawners()
    {
        StopCoroutine(m_delayedSpawn);
    }

    public override void InitializeAllSpawners()
    {
        base.InitializeAllSpawners();
        if (m_isTrigger)
        {
            cols = GetComponents<Collider2D>();
        }
        m_delayedSpawn = StartCoroutine(DelaySpawns(m_timeToStartSpawn));
    }

    public override void ResetSpawners()
    {
        DeintializeAllSpawners();
        if (m_isTrigger)
        {
            foreach(Collider2D col in cols)
            {
                col.enabled = true;
            }
        }
        else
        {
            m_delayedSpawn = StartCoroutine(DelaySpawns(m_timeToStartSpawn));
        }
    }

    IEnumerator DelaySpawns(float p_time)
    {
        yield return new WaitForSeconds(p_time);
        if (!m_isTrigger)
        {
            StartAllSpawners();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == m_playerTag)
        {
            StartAllSpawners();
            foreach(Collider2D col in cols)
            {
                col.enabled = false;
            }
        }
    }
}
