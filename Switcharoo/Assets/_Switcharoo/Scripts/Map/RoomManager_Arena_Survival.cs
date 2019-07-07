using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Arena_Survival : RoomManager_Base
{

    
    public override void CheckRoomObjective()
    {
        bool roomComplete = true;
        foreach(AI_Spawner spawner in m_roomAiManager[0].m_spawnersInRoom)
        {
            if (spawner.gameObject.activeSelf)
            {
                roomComplete = false;
            }
        }
        foreach (GameObject spawner in m_roomAiManager[0].m_placedEnemies)
        {
            if (spawner.activeSelf)
            {
                roomComplete = false;
            }
        }
        if (roomComplete)
        {
            m_roomTaskComplete = true;
        }

    }

    public override void EnemyKilled(AiController p_enemy)
    {
        
    }
}
