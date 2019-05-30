using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Traversal_Navigate : RoomManager_Base
{
    
    [Header("Traversal - Navigate Properties")]
    public Objective_KoTH_Hill m_targetPosition;
    public override void CheckRoomObjective()
    {
        if (m_targetPosition.m_hillComplete)
        {
            m_roomTaskComplete = true;
            m_targetPosition.gameObject.SetActive(false);

            if (m_stopEnemySpawnsOnComplete)
            {
                m_roomAiManager[0].StopAllSpawners();
            }
        }
    }

    public override void EnemyKilled(AiController p_enemy)
    {
    }

    public override void ResetRoom()
    {
        base.ResetRoom();
        m_targetPosition.ResetHill();

    }
}
