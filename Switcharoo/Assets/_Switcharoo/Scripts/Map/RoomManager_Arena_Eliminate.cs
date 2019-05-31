using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Arena_Eliminate : RoomManager_Base
{
    
    [Header("Arena Eliminate Properties")]
    public List<Health> m_targetEnemies;
    public override void CheckRoomObjective()
    {
        if (m_roomTaskComplete) return;

        bool m_allEnemiesDisabled = true;
        foreach(Health currentEnemy in m_targetEnemies)
        {
            if (currentEnemy.m_isDead == false)
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
    public override void ResetRoom()
    {
        base.ResetRoom();
        foreach (Health currentEnemy in m_targetEnemies)
        {
            currentEnemy.m_isDead = false;
            currentEnemy.ResetHealth();
        }
    }

}
