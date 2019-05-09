using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Spawner : MonoBehaviour
{
    [HideInInspector]
    public AI_Spawner_Manager_Base m_spawnManager;
    public float m_spawnPerMinute;
    float m_timeToSpawn;
    public List<EnemySpawns> m_allEnemies;
    public List<EnemySpawns> m_adjustedAllEnemies;
    WaitForSeconds m_spawnDelay;
    Coroutine m_spawnEnemies;



    void InitateSpawning()
    {
        CheckSpawnChances();
        m_timeToSpawn = 60 / m_spawnPerMinute;
        m_spawnDelay = new WaitForSeconds(m_timeToSpawn);
        m_spawnEnemies = StartCoroutine(SpawnRoutine());
    }


    ///<Summary> 
    ///Adjusts the ratios if they do not all add up to 100%
    ///<Summary>
    void CheckSpawnChances()
    {
        float totalPercent = 0;
        foreach (EnemySpawns enemy in m_allEnemies)
        {
            totalPercent += enemy.m_spawnChance;
        }

        if (totalPercent == 1)
        {
            m_adjustedAllEnemies = m_allEnemies;
            return;
        }else if (totalPercent == 0)
        {
            float percent = 1f / m_allEnemies.Count;
            for (int x = 0; x < m_allEnemies.Count; x++)
            {
                m_adjustedAllEnemies.Add(new EnemySpawns(m_allEnemies[x].m_enemyPrefab, percent, m_allEnemies[x].m_spawnDir, m_allEnemies[x].m_enemyPatrolPoint));
            }
            return;
        }

        float changePercent = totalPercent / 1;

        for (int x = 0; x < m_allEnemies.Count; x++)
        {
            m_adjustedAllEnemies.Add(new EnemySpawns(m_allEnemies[x].m_enemyPrefab, m_allEnemies[x].m_spawnChance / changePercent, m_allEnemies[x].m_spawnDir, m_allEnemies[x].m_enemyPatrolPoint));
        }
    }


    void SpawnNewEnemy()
    {

        float enemyChance = Random.Range(0, 1f);
        float currentChange = 0;
        foreach (EnemySpawns enemy in m_adjustedAllEnemies)
        {
            currentChange += enemy.m_spawnChance;
            if (enemyChance < currentChange)
            {
                AiController aiCont = ObjectPooler.instance.NewObject(enemy.m_enemyPrefab, transform, true, false, false).GetComponent<AiController>();
                aiCont.m_currentForward = (enemy.m_spawnDir == EnemySpawns.SpawnDir.Left) ? -1 : 1;
                aiCont.m_spawnerManager = m_spawnManager;
                aiCont.m_patrolPoints = enemy.m_enemyPatrolPoint;
                aiCont.m_agent.m_navGrid = m_spawnManager.m_currentNavGrid;
                aiCont.gameObject.SetActive(true);
                aiCont.transform.position = this.transform.position;
                aiCont.m_isPooled = true;
                aiCont.InitiateAi();


                m_spawnManager.m_currentEnemiesInRoom.Add(aiCont);
                return;
            }

        }

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
            StopCoroutine(m_spawnEnemies);
        }
    }


    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (m_spawnManager.m_currentAiCount < m_spawnManager.m_maxAiInRoom)
            {
                m_spawnManager.m_currentAiCount++;
                SpawnNewEnemy();
            }
            yield return m_spawnDelay;

        }
    }



    [System.Serializable]
    public struct EnemySpawns
    {
        public enum SpawnDir { Left, Right }
        public GameObject m_enemyPrefab;
        [Range(0, 1)]
        public float m_spawnChance;
        public SpawnDir m_spawnDir;

        public List<Transform> m_enemyPatrolPoint;

        public void AdjustSpawnChance(float p_newSpawnChance)
        {
            m_spawnChance = p_newSpawnChance;
        }

        public EnemySpawns(GameObject p_enemy, float p_newChance, SpawnDir p_spawnDir, List<Transform> p_patrolPoints)
        {
            m_enemyPrefab = p_enemy;
            m_spawnChance = p_newChance;
            m_spawnDir = p_spawnDir;
            m_enemyPatrolPoint = p_patrolPoints;
        }
    }
}
