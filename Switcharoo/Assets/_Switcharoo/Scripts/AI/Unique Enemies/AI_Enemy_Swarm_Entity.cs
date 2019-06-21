using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Enemy_Swarm_Entity : MonoBehaviour
{
    public AI_Enemy_Swarm.AiState m_entityState;
    public bool m_targetReached;

    public bool m_queenDied;
    [HideInInspector]
    public float m_speed, m_turnSpeed, m_maxDistanceAway, m_entityDamage;

    public float m_targetStoppingDistance = 1f;
    AI_Enemy_Swarm m_swarmBase;
    Health m_health;
    Rigidbody2D m_rb;
    [HideInInspector]
    public string m_playerTag;

    ObjectPooler m_pooler;


    public Vector3 m_currentTarget;
    private void Start()
    {
        if (m_pooler == null)
        {
            m_pooler = ObjectPooler.instance;
        }
        m_pooler.AddObjectToDespawn(this.gameObject);
        m_health = GetComponent<Health>();
        m_rb = GetComponent<Rigidbody2D>();
    }
    public void InitializeEntity(AI_Enemy_Swarm p_swarmBase, float p_speed, float p_turnSpeed, float p_maxDistanceAway, float p_entityDamage, string p_playerTag)
    {
        m_speed = p_speed;
        m_turnSpeed = p_turnSpeed;
        m_swarmBase = p_swarmBase;
        m_maxDistanceAway = p_maxDistanceAway;
        m_entityDamage = p_entityDamage;
        m_playerTag = p_playerTag;
        m_queenDied = false;
    }

    private void Update()
    {
        m_rb.velocity = transform.right * m_speed;
        if (m_queenDied)
        {
            m_currentTarget = DungeonManager.instance.m_playerGameObject.transform.position;
            m_entityState = AI_Enemy_Swarm.AiState.Attack;
        }
        if (m_currentTarget != null)
        {
            RotateTowards();
        }


        switch (m_entityState)
        {
            case (AI_Enemy_Swarm.AiState.Attack):
                if (Vector3.Distance(transform.position, m_currentTarget) < m_targetStoppingDistance)
                {
                    m_targetReached = true;
                }
                break;
            case (AI_Enemy_Swarm.AiState.Idle):
                if (Vector3.Distance(transform.position, m_swarmBase.transform.position) > m_maxDistanceAway)
                {
                    Vector3 newSwarmPos = new Vector3(Random.Range(-m_swarmBase.m_maxDistanceAway, m_swarmBase.m_maxDistanceAway), Random.Range(-m_swarmBase.m_maxDistanceAway, m_swarmBase.m_maxDistanceAway), 0);
                    Vector2 newDir = (m_swarmBase.transform.position - (transform.position + newSwarmPos)).normalized;
                    transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg);
                }
                break;
        }


        if (m_health.m_isDead)
        {
            Die();
        }

    }

    void RotateTowards()
    {
        Vector2 dir = (Vector2)m_currentTarget - m_rb.position;
        dir.Normalize();
        float rotateAmount = Vector3.Cross(dir, -transform.right).z;
        m_rb.angularVelocity = rotateAmount * m_turnSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == m_playerTag)
        {
            collision.transform.parent.gameObject.GetComponent<Health>().TakeDamage(m_entityDamage);

            m_swarmBase.m_swarmEntities.Remove(this);
            ObjectPooler.instance.ReturnToPool(gameObject);
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (m_swarmBase != null)
        {
            if (m_swarmBase.m_swarmEntities.Contains(this))
            {
                m_swarmBase.m_swarmEntities.Remove(this);
            }
        }
        if (m_pooler == null)
        {
            m_pooler = ObjectPooler.instance;
        }

    }

}
