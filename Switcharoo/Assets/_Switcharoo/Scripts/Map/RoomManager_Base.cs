using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomManager_Base : MonoBehaviour
{
    public List<GameObject> m_roomVariants;
    public List<AI_Spawner_Manager_Base> m_roomAiManager;
    public bool m_roomTaskComplete = false;
    bool m_roomAlreadyComplete = false;

    [HideInInspector]
    public bool m_increaseRoomIndex = false;

    int m_roomVariantIndex = 0;
    public bool m_stopEnemySpawnsOnComplete = true;
    public GameObject m_roomTilemap;
    public GameObject m_tempLockedDoor;


    [HideInInspector]
    public GameObject m_playerObject;

    [HideInInspector]
    public PlatformerNavigation m_navGrid;

    [Space(10)]
    public List<RoomManager_Base> m_roomsToAlterOnCompletion;


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

        foreach (AI_Spawner_Manager_Base spawn in m_roomAiManager)
        {
            spawn.m_roomBase = this;
        }

    }

    private void Start()
    {
        foreach (GameObject currentRoom in m_roomVariants)
        {
            currentRoom.SetActive(true);
        }
        m_roomTilemap.SetActive(true);
        foreach (AI_Spawner_Manager_Base spawns in m_roomAiManager)
        {
            spawns.gameObject.SetActive(!m_roomTaskComplete);
            spawns.InitializeSpawnerManager();
        }

        for (int i = 1; i < m_roomVariants.Count; i++)
        {
            m_roomVariants[i].SetActive(false);
        }

        m_playerObject = DungeonManager.instance.m_playerGameObject;
        foreach (AI_Spawner_Manager_Base spawn in m_roomAiManager)
        {
            spawn.m_roomBase = this;
        }

    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        DeloadRoom();

        m_roomVariants[m_roomVariantIndex].SetActive(false);
        if (m_roomVariantIndex < m_roomAiManager.Count)
        {
            m_roomAiManager[m_roomVariantIndex].gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        if (m_increaseRoomIndex)
        {
            m_increaseRoomIndex = false;
            if (m_roomVariantIndex < m_roomVariants.Count - 1)
            {
                m_roomVariantIndex++;
            }
            
        }
        m_roomVariants[m_roomVariantIndex].SetActive(true);
        if (m_roomVariantIndex < m_roomAiManager.Count)
        {
            m_roomAiManager[m_roomVariantIndex].gameObject.SetActive(true);
        }
    }
    public void DeloadRoom()
    {
        foreach (AI_Spawner_Manager_Base spawns in m_roomAiManager)
        {
            spawns.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        CheckRoomObjective();
        m_tempLockedDoor.SetActive(!m_roomTaskComplete);

        if (m_roomTaskComplete && !m_roomAlreadyComplete)
        {
            m_roomAlreadyComplete = true;
            m_increaseRoomIndex = true;
            foreach(RoomManager_Base room in m_roomsToAlterOnCompletion)
            {
                room.m_increaseRoomIndex = true;
            }
        }
    }

    public abstract void CheckRoomObjective();

    public abstract void EnemyKilled(AiController p_enemy);
}
