using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Arena_Eliminate : RoomManager_Base
{
    
    [Header("Arena Eliminate Properties")]
    public List<GameObject> m_targetEnemies;
    public override void CheckRoomObjective()
    {
        if (m_roomTaskComplete) return;

        bool m_allEnemiesDisabled = true;
        foreach(GameObject currentEnemy in m_targetEnemies)
        {
            if (currentEnemy.activeSelf == true)
            {
                m_allEnemiesDisabled = false;
            }
        }
        m_roomTaskComplete = m_allEnemiesDisabled;

        if (m_stopEnemySpawnsOnComplete)
        {
            m_roomAiManager[0].StopAllSpawners();
        }
    }

    public override void EnemyKilled(AiController p_enemy)
    {
        
    }

}
