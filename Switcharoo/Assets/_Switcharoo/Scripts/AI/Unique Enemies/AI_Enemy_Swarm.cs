using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Enemy_Swarm : MonoBehaviour
{
    public enum AiState { Idle, Attack }
    public AiState m_currentAiState = AiState.Idle;
    public List<AI_Enemy_Swarm_Entity> m_swarmEntities;

    public GameObject m_entityPrefab;
    public int m_entityAmount;
    public float m_entityRespawnTime;
    bool m_respawnEntity;

    public Transform m_target;
    

    [Header("Detection Variables")]
    public CircleCollider2D m_detectionCollider;
    public float m_detectionRadius;
    public string m_playerTag = "Player";

    public float m_maxDistanceAway;
    public float m_maxEntityDistance;
    public float m_entitySpeed, m_entityTurnSpeed;
    public float m_entityDamage;

    WaitForSeconds m_entitySpawnDelay;
    Coroutine m_entitySpawnCoroutine;

    bool m_swarmReset = false;

    private void Start()
    {
        m_entitySpawnDelay = new WaitForSeconds(m_entityRespawnTime);
        m_swarmEntities = new List<AI_Enemy_Swarm_Entity>();
        m_detectionCollider.radius = m_detectionRadius;
        m_swarmReset = true;


    }
    /// <summary>
    /// Spawns drones, until the max amount is reached
    /// </summary>
    void SpawnNewEntities()
    {
        int amountToSpawn = m_entityAmount - m_swarmEntities.Count;
        for (int i = 0; i < amountToSpawn; i++)
        {

            SpawnNewEntity();
        }
    }

    private void OnEnable()
    {
        
        m_swarmReset = true;
        foreach (AI_Enemy_Swarm_Entity drone in m_swarmEntities)
        {
            drone.gameObject.SetActive(false);
            ObjectPooler.instance.ReturnToPool(drone.gameObject);
        }

        m_swarmEntities.Clear();

        m_respawnEntity = false;
        

    }
    private void Update()
    {
        if (m_swarmReset)
        {
            SpawnNewEntities();
            m_swarmReset = false;
        }
        CheckState();

        //the loop for respawning more drones
        if (!m_respawnEntity)
        {
            if (m_swarmEntities.Count < m_entityAmount)
            {
                m_respawnEntity = true;
                m_entitySpawnCoroutine = StartCoroutine(SpawnMoreEntities());
            }
        }
        else
        {
            if (m_swarmEntities.Count == m_entityAmount)
            {
                m_respawnEntity = false;
            }
        }
    }

    void CheckState()
    {
        switch (m_currentAiState)
        {
            case AiState.Attack:
                bool allReachedPoint = true;
                foreach (AI_Enemy_Swarm_Entity currentEntity in m_swarmEntities)
                {
                    if (!currentEntity.m_targetReached)
                    {
                        allReachedPoint = false;
                    }
                }
                if (allReachedPoint)
                {
                    if (IsPlayerInRadius())
                    {
                        NewTarget(m_target.position);
                    }
                    else
                    {
                        SwitchEntityState(AiState.Idle);
                    }
                }
                break;
            case AiState.Idle:
                NewTarget(transform.position);
                break;
        }
    }

    void SpawnNewEntity()
    {
        AI_Enemy_Swarm_Entity newEntity = ObjectPooler.instance.NewObject(m_entityPrefab, transform).GetComponent<AI_Enemy_Swarm_Entity>();
        newEntity.transform.position = new Vector3(transform.position.x, transform.position.y + .2f, 0f);
        newEntity.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, Random.Range(0, 360));
        newEntity.transform.parent = transform.parent;

        newEntity.InitializeEntity(this, m_entitySpeed, m_entityTurnSpeed, m_maxEntityDistance, m_entityDamage, m_playerTag);
        newEntity.m_entityState = m_currentAiState;
        if (m_currentAiState == AiState.Attack)
        {
            newEntity.m_currentTarget = m_target.position;
        }
        m_swarmEntities.Add(newEntity);
    }

    bool IsPlayerInRadius()
    {
        if (Vector3.Distance(transform.position, m_target.position) < m_maxDistanceAway)
        {
            return true;
        }
        return false;
    }
    void SwitchEntityState(AiState p_newState)
    {
        foreach (AI_Enemy_Swarm_Entity currentEntity in m_swarmEntities)
        {
            currentEntity.m_entityState = p_newState;
        }
        m_currentAiState = p_newState;
    }



    void NewTarget(Vector3 p_newTarget)
    {
        foreach (AI_Enemy_Swarm_Entity currentEntity in m_swarmEntities)
        {
            currentEntity.m_currentTarget = p_newTarget;
            currentEntity.m_targetReached = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == m_playerTag && m_currentAiState != AiState.Attack)
        {
            m_target = collision.gameObject.transform;
            SwitchEntityState(AiState.Attack);
            NewTarget(m_target.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
    }

    IEnumerator SpawnMoreEntities()
    {
        yield return m_entitySpawnDelay;
        SpawnNewEntity();
        m_respawnEntity = false;
    }

    private void OnDisable()
    {
        foreach (AI_Enemy_Swarm_Entity drone in m_swarmEntities)
        {
            drone.m_queenDied = true;
        }
    }
}
