using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_KoTH : RoomManager_Base
{
    public List<Objective_KoTH_Hill> m_hillsInRoom;
    Objective_KoTH_Hill m_currentActiveHill;


    public float m_timeToHold;
    public override void CheckRoomObjective()
    {
        if (m_timeToHold <= 0 && !m_roomTaskComplete)
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
}
