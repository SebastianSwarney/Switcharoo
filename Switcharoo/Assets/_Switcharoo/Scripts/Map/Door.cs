using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string m_playerTag = "Player";

    public RoomManager_Base m_currentRoom, m_nextRoom;
    public Transform m_nextRoomSpawn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            DungeonManager.instance.LoadNewMap(m_currentRoom, m_nextRoom, m_nextRoomSpawn.position);
        }
    }
}
