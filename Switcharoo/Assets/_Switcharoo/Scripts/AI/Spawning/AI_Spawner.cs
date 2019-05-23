using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Spawner : MonoBehaviour
{
    public enum SpawnDir { Left, Right };
    public SpawnDir m_currentSpawnDir;
    [HideInInspector]
    public AI_Spawner_Manager_Base m_spawnManager;
    public float m_spawnPerMinute;
    float m_timeToSpawn;
    public GameObject m_enemyToSpawn;
    WaitForSeconds m_spawnDelay;
    Coroutine m_spawnEnemies;

    public List<Transform> m_spawnedEnemyPatrolPoints;

    public int m_maxEnemyFromThis;
    //[HideInInspector]
    public int m_currentEnemyNumber;


    void InitateSpawning()
    {
        m_timeToSpawn = 60 / m_spawnPerMinute;
        m_spawnDelay = new WaitForSeconds(m_timeToSpawn);
        m_spawnEnemies = StartCoroutine(SpawnRoutine());
    }


    void SpawnEnemy()
    {
        AiController aiCont = ObjectPooler.instance.NewObject(m_enemyToSpawn, transform, true, false, false).GetComponent<AiController>();
        aiCont.m_currentForward = (m_currentSpawnDir == SpawnDir.Left) ? -1 : 1;
        aiCont.m_spawnerManager = m_spawnManager;
        aiCont.m_patrolPoints = m_spawnedEnemyPatrolPoints;
        aiCont.m_agent.m_navGrid = m_spawnManager.m_currentNavGrid;
        aiCont.gameObject.SetActive(true);
        aiCont.transform.position = this.transform.position;
        aiCont.m_isPooled = true;

        aiCont.m_parentSpawn = this;

        aiCont.InitiateAi();
        m_currentEnemyNumber++;


        m_spawnManager.m_currentEnemiesInRoom.Add(aiCont);
        return;
    }






    ///<Summary>
    /// Used for forcing an enemy to spawn here.
    /// the forced spawn does not have to be in the list of enemies that can spawn here
    public void ForceSpawnEnemy(GameObject m_forcedSpawn)
    {
        Debug.Log("Forcing a spawn will result in an enemy without patrol points");
        AiController aiCont = ObjectPooler.instance.NewObject(m_forcedSpawn, transform, true, false, false).GetComponent<AiController>();
        aiCont.m_currentForward = (m_spawnManager.m_player.transform.position.x > transform.position.x) ? -1 : 1;
        aiCont.gameObject.SetActive(true);
    }

    ///<Summary>
    ///Start or stop spawning
    public void ChangeSpawning(bool p_spawnEnemies)
    {
        if (p_spawnEnemies)
        {
            InitateSpawning();
        }
        else
        {
            if (m_spawnEnemies == null) return;
            StopCoroutine(m_spawnEnemies);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (m_spawnManager.m_currentAiCount < m_spawnManager.m_maxAiInRoom)
            {
                if (m_currentEnemyNumber < m_maxEnemyFromThis)
                {
                    m_spawnManager.m_currentAiCount++;
                    SpawnEnemy();
                }

            }
            yield return m_spawnDelay;

        }
    }

}
