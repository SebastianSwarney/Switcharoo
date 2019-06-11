using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string m_playerTag = "Player";

    public Transform m_nextRoomSpawnPosition;

    Animator m_doorAnimation;

    [Space(10)]
    public RoomManager_Base m_nextRoom;

    public bool m_drawGizmos;

    private void Awake()
    {
        m_doorAnimation = GetComponent<Animator>();
    }
    public void ChangeLockOnDoor(bool p_lockDoor)
    {
        
        m_doorAnimation.SetBool("DoorUnlocked", !p_lockDoor);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            DungeonManager.instance.LoadNewMap( m_nextRoom, m_nextRoomSpawnPosition.position);
        }
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawArrow(transform.position,transform.up * 2 , Color.yellow);
    }
}
