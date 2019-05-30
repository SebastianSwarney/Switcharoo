using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Traversal_DestroySpawns : RoomManager_Base
{
    
    [Header("Traversal - Destroy Spawns Properties")]
    public List<GameObject> m_enemySpawners;

    public override void CheckRoomObjective()
    {
        bool roomComplete = true;
        foreach(GameObject spawn in m_enemySpawners)
        {
            if (spawn.activeSelf) roomComplete = false;
        }
        if (roomComplete)
        {
            m_roomTaskComplete = true;
            if (m_stopEnemySpawnsOnComplete)
            {
                m_roomAiManager[0].StopAllSpawners();
            }
        }
    }

    public override void EnemyKilled(AiController p_enemy)
    {
    }

}
