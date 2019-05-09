using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomManager_Base : MonoBehaviour
{
    public List<AI_Spawner_Manager_Base> m_roomAiManager;
    public bool m_roomTaskComplete = false;
    public bool m_stopEnemySpawnsOnComplete = true;
    public GameObject m_roomTilemap;
    public GameObject m_tempLockedDoor;


    [HideInInspector]
    public GameObject m_playerObject;

    public PlatformerNavigation m_navGrid;

    void Awake()
    {
        m_navGrid = GetComponent<PlatformerNavigation>();
        if (m_roomAiManager == null)
        {
            Debug.Log(gameObject.name + " does not have a room AI Manager");
        }
        if (m_roomTilemap == null)
        {
            Debug.Log(gameObject.name + " does not have a room tilemap object");
        }
    }
    private void Start()
    {
        m_playerObject = DungeonManager.instance.m_playerGameObject;
        foreach (AI_Spawner_Manager_Base spawn in m_roomAiManager)
        {
            spawn.m_currentNavGrid = m_navGrid;
        }
    }
    private void OnEnable()
    {

        
        m_roomTilemap.SetActive(true);
        foreach (AI_Spawner_Manager_Base spawns in m_roomAiManager)
        {
            spawns.gameObject.SetActive(!m_roomTaskComplete);
        }

    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        DeloadRoom();
    }
    public void DeloadRoom()
    {
        foreach (AI_Spawner_Manager_Base spawns in m_roomAiManager)
        {
            spawns.gameObject.SetActive(false);
        }
        m_roomTilemap.SetActive(false);
    }

    private void Update()
    {
        CheckRoomObjective();
        m_tempLockedDoor.SetActive(!m_roomTaskComplete);
    }

    public abstract void CheckRoomObjective();
}
