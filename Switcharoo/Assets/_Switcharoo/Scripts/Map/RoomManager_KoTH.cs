using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_KoTH : RoomManager_Base
{
    
    [Header("KOTH Properties")]
    public List<Objective_KoTH_Hill> m_hillsInRoom;
    Objective_KoTH_Hill m_currentActiveHill;

    public override void CheckRoomObjective()
    {
        bool allRoomsComplete = true;
        foreach (Objective_KoTH_Hill m_hill in m_hillsInRoom)
        {
            if (!m_hill.m_hillComplete) allRoomsComplete = false;
        }
        if (allRoomsComplete)
        {
            m_roomTaskComplete = true;
            foreach (Objective_KoTH_Hill hill in m_hillsInRoom)
            {
                hill.gameObject.SetActive(false);
            }
            if (m_stopEnemySpawnsOnComplete)
            {
                foreach (AI_Spawner_Manager_Base spawns in m_roomAiManager)
                {
                    spawns.StopAllSpawners();
                }
            }
        }


    }

    public override void EnemyKilled(AiController p_enemy)
    {
    }

    public override void ResetRoom()
    {
        base.ResetRoom();
        foreach(Objective_KoTH_Hill currentHill in m_hillsInRoom)
        {
            currentHill.ResetHill();
        }
    }
}
