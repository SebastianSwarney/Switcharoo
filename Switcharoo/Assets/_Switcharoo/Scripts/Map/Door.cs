using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string m_playerTag = "Player";

    public Transform m_nextRoomSpawnPosition;
    public GameObject m_lockedDoor;

    [Space(10)]
    public RoomManager_Base m_nextRoom;

    public void ChangeLockOnDoor(bool p_lockDoor)
    {
        m_lockedDoor.SetActive(p_lockDoor);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            DungeonManager.instance.LoadNewMap( m_nextRoom, m_nextRoomSpawnPosition.position);
        }
    }
}
