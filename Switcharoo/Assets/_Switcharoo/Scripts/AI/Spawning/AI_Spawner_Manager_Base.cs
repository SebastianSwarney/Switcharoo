using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///Acts as the base spawner manager
///This script does not start the enemies, unless another script tells it to start
///Inherited classes will be able to start the spawning
public abstract class AI_Spawner_Manager_Base : MonoBehaviour
{
    [HideInInspector]
    public GameObject m_player;
    [HideInInspector]
    public RoomManager_Base m_roomBase;

    public GameObject m_enemySpawnersParent;
    public GameObject m_placedEnemiesParent;

    List<AI_Spawner> m_spawnersInRoom;
    List<GameObject> m_placedEnemies;



    public int m_maxAiInRoom;

    [HideInInspector]
    public int m_currentAiCount;

    [HideInInspector]
    public bool m_roomDeloaded = false;



    [HideInInspector]
    public List<AiController> m_currentEnemiesInRoom;

    [HideInInspector]
    public PlatformerNavigation m_currentNavGrid;
    private void Awake()
    {
        m_placedEnemies = new List<GameObject>();
        m_spawnersInRoom = new List<AI_Spawner>();
        m_currentEnemiesInRoom = new List<AiController>();
    }

    ///This object is enabled when the room is enabled


    public void InitializeSpawnerManager()
    {
        m_player = DungeonManager.instance.m_playerGameObject;
        m_currentNavGrid = m_roomBase.m_navGrid;
        m_currentAiCount = 0;
        m_placedEnemies.Clear();

        m_spawnersInRoom.Clear();
        m_currentEnemiesInRoom.Clear();

        for (int i = 0; i < m_placedEnemiesParent.transform.childCount; i++)
        {
            m_placedEnemies.Add(m_placedEnemiesParent.transform.GetChild(i).gameObject);
            m_currentAiCount++;
            m_currentEnemiesInRoom.Add(m_placedEnemiesParent.transform.GetChild(i).GetComponent<AiController>());
        }


        for (int i = 0; i < m_enemySpawnersParent.transform.childCount; i++)
        {

            m_spawnersInRoom.Add(m_enemySpawnersParent.transform.GetChild(i).GetComponent<AI_Spawner>());
        }


        InitializeAllSpawners();

    }

    //Sets up all the spawners, when the object is enabled
    public virtual void InitializeAllSpawners()
    {
        foreach (AiController placedEnemy in m_currentEnemiesInRoom)
        {
            placedEnemy.gameObject.SetActive(true);
            placedEnemy.m_agent.m_navGrid = m_currentNavGrid;
            if (placedEnemy.m_spawnedOnSpawnerDestroy)
            {
                placedEnemy.gameObject.SetActive(false);
            }
        }
        foreach (AI_Spawner spawner in m_spawnersInRoom)
        {
            spawner.gameObject.SetActive(true);
            spawner.m_spawnManager = this;

        }
        foreach (GameObject enemy in m_placedEnemies)
        {
            if (!enemy.GetComponent<AiController>().m_spawnedOnSpawnerDestroy)
            {
                enemy.SetActive(true);
            }
        }
        m_currentAiCount = m_placedEnemies.Count;
    }
    public abstract void ResetSpawners();

    public abstract void DeintializeAllSpawners();

    //The method that is called to enable spawning in this room
    public void StartAllSpawners()
    {
        foreach (AI_Spawner spawner in m_spawnersInRoom)
        {
            spawner.ChangeSpawning(true);
        }
    }

    //The method that is called to stop all spawning in this room
    public void StopAllSpawners()
    {
        foreach (AI_Spawner spawner in m_spawnersInRoom)
        {
            spawner.ChangeSpawning(false);

        }
    }

    public void EnemyKilled(AiController p_enemyKilled)
    {
        if (p_enemyKilled.m_isPooled)
        {
            m_currentEnemiesInRoom.Remove(p_enemyKilled);
        }
        m_currentAiCount -= 1;
        m_roomBase.EnemyKilled(p_enemyKilled);
    }
    //When the room is unloaded, unload all entities attached to this room
    void OnDisable()
    {
        m_roomDeloaded = true;
        DeintializeAllSpawners();
        foreach (AI_Spawner spawner in m_spawnersInRoom)
        {
            spawner.ChangeSpawning(false);

        }
        List<AiController> destroyEnemies = new List<AiController>();
        foreach (AiController enemy in m_currentEnemiesInRoom)
        {
            destroyEnemies.Add(enemy);
        }
        foreach (AiController enem in destroyEnemies)
        {

            enem.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        m_roomDeloaded = false;
        InitializeAllSpawners();
        foreach (AiController enem in m_currentEnemiesInRoom)
        {
            if (!enem.m_spawnedOnSpawnerDestroy)
            {
                enem.gameObject.SetActive(true);
            }

            enem.Respawn();

        }
        foreach (AI_Spawner spawner in m_spawnersInRoom)
        {
            spawner.Respawn();
        }
        ResetSpawners();


    }
}
