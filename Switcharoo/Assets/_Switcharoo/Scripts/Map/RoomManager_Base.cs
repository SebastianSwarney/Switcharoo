using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomManager_Base : MonoBehaviour
{
    public List<AI_Spawner_Manager_Base> m_roomAiManager;
    public List<GameObject> m_roomAiVariants;
    public List<RoomManager_Base> m_roomsToAlterOnCompletion;
    public List<GameObject> m_roomTilemapVariants;



    [Header("Preset variables")]
    public bool m_roomTaskComplete = false;
    bool m_roomAlreadyComplete = false;
    public GameObject m_doorsParent;
    public GameObject m_roomObjectsParent;
    List<IActivatable> m_resetableObjects;



    int m_roomVariantIndex = 0;
    public bool m_stopEnemySpawnsOnComplete = true;



    List<Door> m_LockedDoors;

    [HideInInspector]
    public bool m_increaseRoomIndex = false;

    [HideInInspector]
    public GameObject m_currentLoadedTilemap;

    [HideInInspector]
    public GameObject m_playerObject;

    [HideInInspector]
    public PlatformerNavigation m_navGrid;




    void Awake()
    {
        m_navGrid = GetComponent<PlatformerNavigation>();
        if (m_roomAiManager == null)
        {
            Debug.Log(gameObject.name + " does not have a room AI Manager");
        }

        foreach (AI_Spawner_Manager_Base spawn in m_roomAiManager)
        {
            spawn.m_roomBase = this;
        }
        m_LockedDoors = new List<Door>();
        for (int i = 0; i < m_doorsParent.transform.childCount; i++)
        {
            m_LockedDoors.Add(m_doorsParent.transform.GetChild(i).GetComponent<Door>());
        }
        m_resetableObjects = new List<IActivatable>();
        for (int i = 0; i < m_roomObjectsParent.transform.childCount; i++)
        {
            IActivatable activateMe = m_roomObjectsParent.transform.GetChild(i).GetComponent<IActivatable>();
            if (activateMe != null)
            {
                m_resetableObjects.Add(activateMe);
            }
        }




    }

    private void Start()
    {
        foreach (GameObject currentRoom in m_roomAiVariants)
        {
            currentRoom.SetActive(true);
        }

        foreach (AI_Spawner_Manager_Base spawns in m_roomAiManager)
        {
            spawns.gameObject.SetActive(!m_roomTaskComplete);
            spawns.InitializeSpawnerManager();

        }

        for (int i = 1; i < m_roomAiVariants.Count; i++)
        {
            m_roomAiVariants[i].SetActive(false);
            if (i < m_roomTilemapVariants.Count)
            {
                if (m_roomTilemapVariants[i] == m_roomTilemapVariants[0]) continue;
                m_roomTilemapVariants[i].SetActive(false);
            }

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

        m_roomAiVariants[m_roomVariantIndex].SetActive(false);
        if (m_roomVariantIndex < m_roomAiManager.Count)
        {
            m_roomAiManager[m_roomVariantIndex].gameObject.SetActive(false);

        }
        if (m_roomVariantIndex < m_roomTilemapVariants.Count)
        {
            m_roomTilemapVariants[m_roomVariantIndex].SetActive(false);
        }
        else
        {
            m_roomTilemapVariants[m_roomTilemapVariants.Count - 1].SetActive(false);
        }
    }
    private void OnEnable()
    {
        if (m_increaseRoomIndex)
        {
            m_increaseRoomIndex = false;
            if (m_roomVariantIndex < m_roomAiVariants.Count - 1)
            {
                m_roomVariantIndex++;
            }

        }
        m_roomAiVariants[m_roomVariantIndex].SetActive(true);
        if (m_roomVariantIndex < m_roomAiManager.Count)
        {
            m_roomAiManager[m_roomVariantIndex].gameObject.SetActive(true);
        }
        if (m_roomVariantIndex < m_roomTilemapVariants.Count)
        {
            m_roomTilemapVariants[m_roomVariantIndex].SetActive(true);
            m_currentLoadedTilemap = m_roomTilemapVariants[m_roomVariantIndex];
        }
        else
        {
            m_roomTilemapVariants[m_roomTilemapVariants.Count - 1].SetActive(true);
            m_currentLoadedTilemap = m_roomTilemapVariants[m_roomTilemapVariants.Count - 1];
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


        if (m_roomTaskComplete && !m_roomAlreadyComplete)
        {
            foreach (Door currentDoor in m_LockedDoors)
            {
                currentDoor.ChangeLockOnDoor(false);
            }


            m_roomAlreadyComplete = true;
            m_increaseRoomIndex = true;
            foreach (RoomManager_Base room in m_roomsToAlterOnCompletion)
            {
                room.m_increaseRoomIndex = true;
            }
        }
    }

    public abstract void CheckRoomObjective();

    public abstract void EnemyKilled(AiController p_enemy);

    public virtual void ResetRoom()
    {
        if (m_roomVariantIndex == 0)
        {
            m_roomTaskComplete = false;
            m_roomAlreadyComplete = false;
            foreach (Door currentDoor in m_LockedDoors)
            {
                currentDoor.ChangeLockOnDoor(true);
            }
        }

        foreach (IActivatable resetMe in m_resetableObjects)
        {
            resetMe.ResetMe();
        }

        m_increaseRoomIndex = false;


    }
}
