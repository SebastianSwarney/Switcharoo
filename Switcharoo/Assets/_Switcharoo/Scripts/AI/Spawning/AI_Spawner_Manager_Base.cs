using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///Acts as the base spawner manager
///This script does not start the enemies, unless another script tells it to start
///Inherited classes will be able to start the spawning
public abstract class AI_Spawner_Manager_Base : MonoBehaviour
{
    public GameObject m_player;
    public List<AI_Spawner> m_spawnersInRoom;

    public List<GameObject> m_placedEnemies;
    public int m_maxAiInRoom;
    public int m_currentAiCount;

    public List<AiController> m_currentEnemiesInRoom;

    void Awake()
    {
        foreach (AI_Spawner spawner in m_spawnersInRoom)
        {
            spawner.m_spawnManager = this;
        }

    }

    
    ///This object is enabled when the room is enabled
    void OnEnable()
    {
        InitializeAllSpawners();
    }

    //Sets up all the spawners, when the object is enabled
    public virtual void InitializeAllSpawners()
    {
        foreach(AiController placedEnemy in m_currentEnemiesInRoom){
            placedEnemy.gameObject.SetActive(true);
        }
        foreach (AI_Spawner spawner in m_spawnersInRoom)
        {
            spawner.gameObject.SetActive(true);
        }
        foreach (GameObject enemy in m_placedEnemies)
        {
            enemy.SetActive(true);
        }
        m_currentAiCount = m_placedEnemies.Count;
    }

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

    
    //When the room is unloaded, unload all entities attached to this room
    void OnDisable()
    {
        foreach(AI_Spawner spawner in m_spawnersInRoom){
            spawner.ChangeSpawning(false);
            spawner.gameObject.SetActive(false);

        }

        foreach(AiController enemy in m_currentEnemiesInRoom){
            enemy.gameObject.SetActive(false);
        }
    }
}
