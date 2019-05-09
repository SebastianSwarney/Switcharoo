using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Arena_Survival : RoomManager_Base
{
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
}
