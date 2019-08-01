using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsDoor : MonoBehaviour
{
    public List<RoomManager_Base> m_mainRooms;
    public List<Door> m_door;
    public List<Hazard_ForceFieldWall> m_activatables;




    public void CheckDoor()
    {
        bool complete = true;
        foreach (RoomManager_Base room in m_mainRooms)
        {
            if (!room.m_roomTaskComplete)
            {
                complete = false;
            }
        }
        if (complete)
        {
            foreach (Door door in m_door)
            {
                door.ChangeLockOnDoor(false);

            }
            foreach (Hazard_ForceFieldWall activate in m_activatables)
            {
                activate.ActiveState(false);
            }
        }
    }
}
