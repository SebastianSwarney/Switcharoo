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
    public override void InitializeAllSpawners()
    {
        base.InitializeAllSpawners();
        m_delayedSpawn = StartCoroutine(DelaySpawns());
    }

    IEnumerator DelaySpawns()
    {
        yield return new WaitForSeconds(m_timeToStartSpawn);
        StartAllSpawners();
    }
}
