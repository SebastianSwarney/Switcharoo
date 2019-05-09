using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance { get; private set; }
    public GameObject m_playerGameObject;

    public List<RoomManager_Base> m_allRooms;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        foreach (RoomManager_Base currentRoom in m_allRooms)
        {
            if (!currentRoom.gameObject.activeSelf)
            {
                currentRoom.DeloadRoom();
            }
        }
    }

    public void LoadNewMap(RoomManager_Base p_deloadMap, RoomManager_Base p_loadMap, Vector3 p_playerSpawnPos)
    {
        p_loadMap.gameObject.SetActive(true);
        Debug.Log("Do fancy camera pan, through a coroutine, here");
        m_playerGameObject.transform.position = p_playerSpawnPos;
        p_deloadMap.gameObject.SetActive(false);


    }
}
