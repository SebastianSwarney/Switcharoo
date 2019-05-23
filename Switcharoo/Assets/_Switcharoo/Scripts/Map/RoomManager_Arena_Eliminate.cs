using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager_Arena_Eliminate : RoomManager_Base
{
    public List<GameObject> m_targetEnemies;
    public override void CheckRoomObjective()
    {
        if (m_roomTaskComplete) return;
        List<GameObject> removeFromList = new List<GameObject>();
        foreach(GameObject currentTarget in m_targetEnemies)
        {
            if (!currentTarget.activeSelf)
            {
                removeFromList.Add(currentTarget);
            }
        }
        foreach (GameObject remove in removeFromList)
        {
            m_targetEnemies.Remove(remove);
        }

        if (m_targetEnemies.Count == 0)
        {
            m_roomTaskComplete = true;
        }
    }

    public override void EnemyKilled(AiController p_enemy)
    {
        throw new System.NotImplementedException();
    }
}
