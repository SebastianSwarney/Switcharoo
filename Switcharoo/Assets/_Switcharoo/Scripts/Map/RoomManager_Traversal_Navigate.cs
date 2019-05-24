﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Traversal_Navigate : RoomManager_Base
{

    public Objective_KoTH_Hill m_targetPosition;
    public override void CheckRoomObjective()
    {
        if (m_targetPosition.m_hillComplete)
        {
            m_roomTaskComplete = true;
            m_targetPosition.gameObject.SetActive(false);

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
}