using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class OnSpawnerDeath : UnityEvent { }

public class AI_Spawner : MonoBehaviour, IPauseable
{
    public enum SpawnDir { Left, Right };
    public bool m_showGizmos = true;



    [Header("Trigger Properties")]
    public bool m_triggerStart;
    public LayerMask m_playerLayers;
    public Vector2 m_triggerOrigin;
    public Vector2 m_triggerDimensions;
    Coroutine m_triggerCheckCoroutine;

    [HideInInspector]
    public AI_Spawner_Manager_Base m_spawnManager;


    [Header("Spawner Properties")]

    public SpawnDir m_currentSpawnDir;
    public float m_spawnerRadius;
    public float m_spawnPerMinute;
    public int m_maxEnemyFromThis;
    float m_timeToSpawn;

    [Header("Enemy Properties")]
    public GameObject m_enemyToSpawn;
    WaitForSeconds m_spawnDelay;
    Coroutine m_spawnEnemies;



    public List<Transform> m_spawnedEnemyPatrolPoints;


    [HideInInspector]
    public int m_currentEnemyNumber;

    public List<GameObject> m_disableObjectsOnDisable;

    public enum ObjectSpawnType { Heavy, Enemy, Object, ExistingObject }
    public List<ObjectSpawnsOnDeath> m_spawnOnDestroyed;


    Health m_health;



    bool m_isPaused;

    [Header("Events")]
    public OnSpawnerDeath m_spawnerDestroyed = new OnSpawnerDeath();

    private void Start()
    {
        ObjectPooler.instance.AddObjectToPooler(this.gameObject);
    }
    void InitateSpawning()
    {


        m_timeToSpawn = 60 / m_spawnPerMinute;
        m_spawnDelay = new WaitForSeconds(m_timeToSpawn);

        if (m_spawnEnemies != null)
        {
            StopCoroutine(m_spawnEnemies);
        }
        if (!gameObject.activeSelf || m_triggerStart) return;
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

        Vector3 randomPos = Random.insideUnitCircle * m_spawnerRadius + (Vector2)transform.position;
        aiCont.transform.position = randomPos;

        aiCont.InitiateAi();
        m_currentEnemyNumber++;


        m_spawnManager.m_currentEnemiesInRoom.Add(aiCont);
        return;
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

    private void Update()
    {
        if (m_health.m_isDead)
        {
            Die();
        }
    }

    void Die()
    {
        
        m_spawnerDestroyed.Invoke();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (m_health == null)
        {
            m_health = GetComponent<Health>();
        }

        foreach (GameObject currentObj in m_disableObjectsOnDisable)
        {
            currentObj.gameObject.SetActive(true);
        }

        if (m_triggerCheckCoroutine != null)
        {
            StopCoroutine(m_triggerCheckCoroutine);
        }
        if (m_triggerStart)
        {
            m_triggerCheckCoroutine = StartCoroutine(TriggerRoutine());
        }
    }
    private void OnDisable()
    {

        foreach (GameObject currentObj in m_disableObjectsOnDisable)
        {
            currentObj.gameObject.SetActive(false);
        }

        if (m_health.m_isDead)
        {
            foreach (ObjectSpawnsOnDeath spawning in m_spawnOnDestroyed)
            {
                if (spawning.m_spawnObject == null) continue;
                if (spawning.m_objectSpawnType == ObjectSpawnType.Enemy)
                {
                    AiController aiCont = ObjectPooler.instance.NewObject(spawning.m_spawnObject, this.transform).GetComponent<AiController>();
                    aiCont.m_patrolPoints = spawning.m_patrolPoints;
                    m_spawnManager.m_currentEnemiesInRoom.Add(aiCont);
                }

                else if (spawning.m_objectSpawnType == ObjectSpawnType.Heavy)
                {
                    AiController aiCont = ObjectPooler.instance.NewObject(spawning.m_spawnObject, this.transform).GetComponent<AiController>();
                    aiCont.m_originPoint = spawning.m_patrolPoints[0];
                    m_spawnManager.m_currentEnemiesInRoom.Add(aiCont);
                }

                else if (spawning.m_objectSpawnType == ObjectSpawnType.Object)
                {
                    ObjectPooler.instance.NewObject(spawning.m_spawnObject, this.transform);
                }

                else if (spawning.m_objectSpawnType == ObjectSpawnType.ExistingObject)
                {
                    spawning.m_spawnObject.SetActive(true);
                    AiController aiCont = spawning.m_spawnObject.GetComponent<AiController>();
                    if (aiCont != null)
                    {
                        aiCont.m_originPoint = spawning.m_patrolPoints[0];
                        aiCont.m_patrolPoints = spawning.m_patrolPoints;
                        if (!m_spawnManager.m_currentEnemiesInRoom.Contains(aiCont))
                        {
                            m_spawnManager.m_currentEnemiesInRoom.Add(aiCont);
                        }

                    }
                }
            }
        }
    }

    public void Respawn()
    {

        m_health.m_isDead = false;
        m_health.ResetHealth();
        gameObject.SetActive(true);

    }

    IEnumerator SpawnRoutine()
    {

        while (true)
        {
            if (m_spawnManager.m_currentAiCount < m_spawnManager.m_maxAiInRoom)
            {
                if (!m_isPaused)
                {


                    if (m_currentEnemyNumber < m_maxEnemyFromThis)
                    {
                        m_spawnManager.m_currentAiCount++;
                        SpawnEnemy();
                    }
                }
            }
            yield return m_spawnDelay;

        }
    }

    IEnumerator TriggerRoutine()
    {
        bool m_playerFound = false;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!m_playerFound)
        {
            yield return delay;
            if (Physics2D.OverlapBox(m_triggerOrigin + (Vector2)transform.position, m_triggerDimensions, 0, m_playerLayers) != null)
            {
                m_playerFound = true;
            }
        }
        InitateSpawning();
        m_spawnEnemies = StartCoroutine(SpawnRoutine());

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_triggerOrigin + (Vector2)transform.position, m_triggerDimensions);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_spawnerRadius);
    }

    public void SetPauseState(bool p_isPaused)
    {
        m_isPaused = p_isPaused;
    }

    [System.Serializable]
    public struct ObjectSpawnsOnDeath
    {
        public ObjectSpawnType m_objectSpawnType;
        public Vector2 m_spawnPosition;
        public float m_spawnRadius;
        public GameObject m_spawnObject;
        public List<Transform> m_patrolPoints;
    }

}
