using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnEnemyJump : UnityEvent { }

[System.Serializable]
public class OnEnemyDied : UnityEvent<bool> { }

[System.Serializable]
public class OnPlayerSpotted : UnityEvent<bool> { }

[System.Serializable]
public class OnEnemyAttack : UnityEvent<bool> { }

[System.Serializable]
public class OnEnemyShoot : UnityEvent<bool> { }

[System.Serializable]
public class OnEnemyShootBreak : UnityEvent { }

[System.Serializable]
public class OnEnemyShootAlt : UnityEvent { }
[System.Serializable]
public class OnEnemyShootAltBreak : UnityEvent { }

[System.Serializable]
public class OnEnemyGrounded : UnityEvent<bool> { }

[System.Serializable]
public class OnPaused : UnityEvent<bool> { }

///<Summary>
///The brain of the AI. This is essentially an empty shell that requires components to function
public class AiController : MonoBehaviour, IPauseable
{



    public Enemy_Base m_enemyType;
    public bool m_spawnedOnSpawnerDestroy = false;
    [HideInInspector]
    public GameObject m_target;
    [Space(10)]
    public string m_playerTag = "Player";

    #region Components on the Enemy

    Rigidbody2D m_rb;
    ShootController m_gun;
    public SpriteRenderer m_sRend;
    public bool m_flipTransform;

    public Transform m_bulletOrigin;


    [HideInInspector]
    public bool m_isPooled = false;


    public CircleCollider2D m_detectionRange;
    Health m_enemyHealth;
    #endregion

    #region Attack Variables
    [Header("Attack State")]
    public AI_AttackType_Base.AttackState m_currentAttackState = AI_AttackType_Base.AttackState.Finished;

    Vector3 m_attackTargetPos;

    [HideInInspector]
    public Vector3 m_delayedPlayerPosition;

    [HideInInspector]
    public float m_visualTellTimer;
    float m_currentShootTimer, m_currentShootDelay;




    #endregion

    #region Move Variables
    public float m_idleSpeed, m_attackSpeed;
    public int m_currentForward = 1;


    public List<Transform> m_patrolPoints;
    Queue<Transform> m_patrolPointOrder;

    [HideInInspector]
    public Transform currentPatrolPoint;

    //[HideInInspector]
    public bool m_isGrounded;

    #endregion

    #region PAthfinding Variables


    [HideInInspector]
    public Ai_Pathfinding_Agent m_agent;
    public float m_patrolStoppingDistance;

    #endregion

    #region Managing Variables
    [HideInInspector]
    public AI_Spawner_Manager_Base m_spawnerManager;
    [HideInInspector]
    public AI_Spawner m_parentSpawn;
    #endregion

    #region Heavy Exclusive Variables
    [Header("Heavy Specific Variables")]

    public Transform m_originPoint;    
    public Transform m_shootAltOrigin;
    //[HideInInspector]
    public bool m_fireAlt;
    #endregion

    #region respawn Variables
    Vector3 m_respawnPos;
    int m_startingForward;
    bool m_died = false;
    #endregion

    #region Events
    public OnEnemyDied m_enemyDied = new OnEnemyDied();
    public OnPlayerSpotted m_playerSpotted = new OnPlayerSpotted();
    public OnEnemyAttack m_enemyAttack = new OnEnemyAttack();
    public OnEnemyJump m_enemyJump = new OnEnemyJump();
    public OnEnemyShoot m_enemyShoot = new OnEnemyShoot();
    public OnEnemyShootBreak m_enemyShootBreak = new OnEnemyShootBreak();
    public OnEnemyShootAlt m_enemyShootAlt = new OnEnemyShootAlt();
    public OnEnemyShootAltBreak m_enemyShootAltBreak = new OnEnemyShootAltBreak();
    public OnEnemyGrounded m_enemyGrounded = new OnEnemyGrounded();
    public OnPaused m_enemyPaused = new OnPaused();
    #endregion


    #region Physics Settings
    [Header("Physics Settings")]
    public bool m_debugPhysicsChecks;
    public float m_circleCastRad;
    public float m_checkWallDistance;
    public Vector3 m_spriteOffset;
    public Vector3 m_groundCheckPos;
    public Vector3 m_groundCheckDimensions;
    public LayerMask m_wallLayer;
    public LayerMask m_movementFlipLayer;

    #endregion


    #region Animation Delay Events
    //[HideInInspector]
    public bool m_jumpAnim, m_beginJump, m_isJumping, m_shootingMovement;

    [HideInInspector]
    public bool m_startShootAnim, m_inShootingAnim;
    [HideInInspector]
    public int m_bulletsPerPattern;
    public int m_currentBulletAmount;

    [HideInInspector]
    public float m_shootBreakTime, m_shootTriggerTime;
    float m_currentShootBreakTimer, m_currentShootBreakTime;


    #endregion


    #region Pause Settings
    Vector3 m_pausedVelocity;
    bool m_isPaused;
    #endregion

    void Awake()
    {
        m_enemyHealth = GetComponent<Health>();
        m_patrolPointOrder = new Queue<Transform>();
        m_rb = GetComponent<Rigidbody2D>();
        m_gun = GetComponent<ShootController>();
        m_agent = GetComponent<Ai_Pathfinding_Agent>();
        FlipEnemy(m_currentForward);

        if (!m_isPooled)
        {
            m_respawnPos = transform.position;
            m_startingForward = m_currentForward;
            
        }
    }

    private void Start()
    {
        if (!m_isPooled)
        {
            ObjectPooler.instance.AddObjectToPooler(this.gameObject);
        }
    }

    public void Respawn()
    {
        m_died = false;
        transform.position = m_respawnPos;
        m_currentForward = m_startingForward;
        FlipEnemy(m_currentForward);
    }

    private void Update()
    {
        if (!m_isPaused)
        {
            if (m_enemyHealth.m_isDead && !m_died)
            {
                m_died = true;
                Die();
            }
            if (!m_died)
            {
                CheckState();
            }
        }

    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        m_enemyHealth.ResetHealth();
        if (m_enemyType == null)
        {
            Debug.Log("Error: " + gameObject.name + " has no assigned enemy type");
            Debug.Break();
        }
        m_detectionRange.radius = m_enemyType.m_attackType.m_attackRadius;

        InitiateAi();
    }

    public void InitiateAi()
    {
        m_patrolPointOrder.Clear();
        foreach (Transform patrolPoint in m_patrolPoints)
        {
            m_patrolPointOrder.Enqueue(patrolPoint);
        }
        if (m_patrolPointOrder.Count > 0)
        {
            currentPatrolPoint = m_patrolPointOrder.Dequeue();

        }
    }
    void CheckState()
    {



        if (m_target != null)
        {

            //Check if the enemy is grounded

            m_isGrounded = m_enemyType.m_attackType.m_attackMovement.IsGrounded(this, m_wallLayer);


            //If the attack is finished, and no longer running
            if (m_currentAttackState == AI_AttackType_Base.AttackState.Finished)
            {
                if (!PlayerInRadius())
                {
                    m_target = null;
                }
                else
                {
                    //Restart the attack
                    m_enemyType.m_attackType.StartAttack(this, m_rb, m_target, gameObject, m_gun);
                    m_attackTargetPos = m_enemyType.m_attackType.SetAttackTargetPosition(this, gameObject, m_target);
                }
            }

            //If the attack is running
            else
            {

                //If the attack has ceased
                if (m_enemyType.m_attackType.AttackFinished(this, m_rb, m_attackTargetPos, m_target, gameObject, m_bulletOrigin, m_gun))
                {
                    //If the player is no lnger in the radius, set the target to null
                    if (!PlayerInRadius())
                    {
                        m_target = null;
                    }
                }
                else
                {
                    //If the player has moved a set amount of distance (set in the attack type), recalculate the attackTarget Position
                    if (PlayerMoved())
                    {
                        m_attackTargetPos = m_enemyType.m_attackType.SetAttackTargetPosition(this, gameObject, m_target);
                    }
                    if (m_inShootingAnim)
                    {
                        CheckShootingTimer();
                    }
                }
            }
        }

        else
        {
            PerformIdleMovement();
        }
    }


    private void OnDrawGizmos()
    {
        if (!m_debugPhysicsChecks) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(m_groundCheckPos + transform.position, m_groundCheckDimensions);
        Gizmos.color = Color.blue;
        Vector3 spherePos = transform.position + m_spriteOffset;
        spherePos = new Vector3(spherePos.x + m_checkWallDistance * m_currentForward, spherePos.y, 0f);
        Gizmos.DrawWireSphere(spherePos, m_circleCastRad);
        Debug.DrawLine(transform.position + m_spriteOffset, spherePos);

    }

    #region Attacking Functions


    ///<Summary>
    ///If the player has moved a set distance from a spot
    bool PlayerMoved()
    {
        if (m_target == null) return false;
        if (Vector3.Distance(m_target.transform.position, m_delayedPlayerPosition) > m_enemyType.m_attackType.m_playerMoveDistanceReaction)
        {
            m_delayedPlayerPosition = m_target.transform.position;

            return true;
        }
        return false;
    }

    ///<Summary>
    ///Checks if the player is in a set radius
    ///TODO: Create the following function
    bool PlayerInRadius()
    {
        if (Vector3.Distance(transform.position, m_target.transform.position) < m_enemyType.m_attackType.m_attackRadius)
        {
            return true;
        }
        else
        {

            return false;
        }
    }

    #endregion

    #region Shooting Settings
    public void ShootGun()
    {

        m_gun.Shoot(m_fireAlt ? m_shootAltOrigin : m_bulletOrigin);
        m_currentBulletAmount++;
        m_currentShootBreakTimer = Time.time;
        if (m_currentBulletAmount >= m_bulletsPerPattern)
        {
            m_currentBulletAmount = 0;
            m_currentShootBreakTime = m_shootTriggerTime;
            if (m_originPoint != null)
            {
                m_currentAttackState = AI_AttackType_Base.AttackState.Start;
            }
            return;
        }
        m_currentShootBreakTime = m_shootBreakTime;
    }

    public void ChangeAnimation(bool p_active)
    {


        if (m_inShootingAnim != p_active)
        {
            m_enemyShoot.Invoke(p_active);

            m_inShootingAnim = p_active;
        }




    }

    void CheckShootingTimer()
    {
        if (Time.time - m_currentShootBreakTimer > m_currentShootBreakTime)
        {
            if (!m_fireAlt)
            {
                m_enemyShootBreak.Invoke();
            }
            else
            {
                m_enemyShootAlt.Invoke();
            }



        }
    }

    #endregion

    #region Move Functions
    void PerformIdleMovement()
    {

        //If there exists patrol points
        if (m_patrolPoints.Count > 0)
        {
            if (CloseToPoint(currentPatrolPoint.position) && m_isGrounded)
            {
                currentPatrolPoint = NewPatrolPoint();
            }

            m_enemyType.m_idleMovementType.PerformIdleMovement(this, m_agent, m_rb, transform, m_currentForward, currentPatrolPoint.position, m_isGrounded);

        }
        else
        {
            m_enemyType.m_idleMovementType.PerformIdleMovement(this, m_agent, m_rb, transform, m_currentForward, transform.position, m_isGrounded);
        }



        //Check for walls infront, and check if gronded
        Vector2 circleCastPos = new Vector2(transform.position.x + m_spriteOffset.x, transform.position.y + m_spriteOffset.y);

        Vector2 floorBoxcastPos = transform.position + m_spriteOffset;
        m_isGrounded = m_enemyType.m_idleMovementType.m_movementType.IsGrounded(this, m_wallLayer);

        if (m_enemyType.m_idleMovementType.m_movementType.WallInFront(this, m_rb, circleCastPos, m_circleCastRad, m_currentForward, m_movementFlipLayer, m_isGrounded))
        {

            FlipEnemy(m_currentForward * -1);
        }

    }



    bool CloseToPoint(Vector3 p_targetPoint)
    {
        if (Vector3.Distance(transform.position, p_targetPoint) < m_patrolStoppingDistance)
        {
            int newDir = (int)Mathf.Sign(p_targetPoint.x - transform.position.x);
            FlipEnemy(newDir);
            return true;
        }
        return false;
    }

    Transform NewPatrolPoint()
    {

        m_patrolPointOrder.Enqueue(currentPatrolPoint);
        return m_patrolPointOrder.Dequeue();

    }
    #endregion



    public void FlipEnemy(int p_newXDir)
    {
        m_currentForward = p_newXDir;

        if (!m_flipTransform)
        {
            m_sRend.flipX = (m_currentForward > 0) ? false : true;
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * p_newXDir, transform.localScale.y, transform.localScale.z);
        }

    }




    void OnTriggerStay2D(Collider2D other)
    {
        if (m_target != null)
        {
            return;
        }
        if (other.gameObject.tag == m_playerTag)
        {

            float dis = Vector3.Distance(transform.position, other.gameObject.transform.position);
            if (dis < m_enemyType.m_attackType.m_attackRadius)
            {
                m_target = other.gameObject;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == m_playerTag)
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(m_enemyType.m_collisionDamage);
        }
    }

    #region Enemy Death
    void Die()
    {
        EnemyDied(true);

    }

    public void DeathAnimComplete()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (m_spawnerManager != null)
        {
            if (m_isPooled)
            {
                m_patrolPoints.Clear();
                ObjectPooler.instance.ReturnToPool(this.gameObject);
            }
            m_spawnerManager.EnemyKilled(this);

        }
        if (m_parentSpawn != null)
        {
            m_parentSpawn.m_currentEnemyNumber--;
        }

    }

    #endregion
    
    #region Fire Events

    public void EnemyDied(bool p_active)
    {
        m_enemyDied.Invoke(p_active);
    }

    public void PlayerSpotted(bool p_active)
    {
        m_playerSpotted.Invoke(p_active);
    }

    public void EnemyJump()
    {
        m_enemyJump.Invoke();
    }

    public void EnemyShoot(bool p_active)
    {
        m_enemyShoot.Invoke(p_active);

    }



    public void EnemyGrounded(bool p_active)
    {
        m_enemyGrounded.Invoke(p_active);
    }


    #endregion

    public void SetPauseState(bool p_isPaused)
    {
        if (p_isPaused && !m_isPaused)
        {
            m_isPaused = true;
            m_pausedVelocity = m_rb.velocity;
            m_rb.velocity = Vector3.zero;
            m_rb.isKinematic = true;

        }
        else if (!p_isPaused && m_isPaused)
        {
            m_isPaused = false;
            m_rb.isKinematic = false;
            m_rb.velocity = m_pausedVelocity;

        }

        m_enemyPaused.Invoke(p_isPaused);

    }



}