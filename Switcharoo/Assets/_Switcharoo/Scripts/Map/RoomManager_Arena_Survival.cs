using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Arena_Survival : RoomManager_Base
{
    
    [Header("Arena Survival Properties")]
    public int m_enemiesLeft;
    public override void CheckRoomObjective()
    {
        if (!m_roomTaskComplete && m_enemiesLeft <= 0)
        {
            m_roomTaskComplete = true;
            foreach(AI_Spawner_Manager_Base spawn in m_roomAiManager)
            {
                spawn.StopAllSpawners();
            }

        }
    }

    public override void EnemyKilled(AiController p_enemy)
    {
        m_enemiesLeft -= 1;
    }
}
