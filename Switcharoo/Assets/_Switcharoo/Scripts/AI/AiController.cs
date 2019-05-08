using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///The brain of the AI. This is essentially an empty shell that requires components to function
public class AiController : MonoBehaviour
{
    public Enemy_Base m_enemyType;
    [Space(10)]
    public GameObject target;
    public string playerTag = "Player";

    #region Components on the Enemy

    Rigidbody2D m_rb;
    ShootController m_gun;

    public Transform m_bulletOrigin;

    [HideInInspector]
    public bool m_isPooled = false;
    Vector3 m_hardSetPos;

    public CircleCollider2D m_detectionRange;
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

    bool m_isShooting;
    #endregion

    #region Move Variables
    public int m_currentForward = 1;
    public LayerMask m_wallLayer;
    public LayerMask m_movementFlipLayer;
    public float m_circleCastRad;
    public Vector2 m_circleCastOffset;
    public float m_stuckMoveTime = 3;
    float m_stuckTimer;
    public bool m_isStuck;

    public List<Transform> m_patrolPoints;
    Queue<Transform> m_patrolPointOrder;
    public Transform currentPatrolPoint;

    [HideInInspector]
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
    #endregion


    void Awake()
    {
        m_patrolPointOrder = new Queue<Transform>();
        m_rb = GetComponent<Rigidbody2D>();
        m_gun = GetComponent<ShootController>();
        m_agent = GetComponent<Ai_Pathfinding_Agent>();

        if (!m_isPooled)
        {
            m_hardSetPos = transform.position;
            //gameObject.SetActive(false);
        }
    }

    private void Update()
    {

        CheckState();

    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (m_enemyType == null)
        {
            Debug.Log("Error: " + gameObject.name + " has no assigned enemy type");
            Debug.Break();
        }
        m_detectionRange.radius = m_enemyType.m_attackType.m_attackRadius;
        if (!m_isPooled)
        {
            transform.position = m_hardSetPos;
        }
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
        if (m_isStuck)
        {
            m_stuckTimer += Time.deltaTime;
            if (m_stuckTimer > m_stuckMoveTime)
            {
                FlipEnemy(m_currentForward * -1);
                print("Fliped : stcuk?");
                m_isStuck = false;
            }
        }
        else
        {
            m_stuckTimer = 0;
        }
        
        
        if (target != null)
        {

            //Check if the enemy is grounded
            Vector2 wallBoxcastPos = new Vector2(transform.position.x + (m_enemyType.m_enemyDimensions.x / 2) * m_currentForward, transform.position.y);
            Vector2 floorBoxcastPos = transform.position;
            m_isGrounded = m_enemyType.m_attackType.m_attackMovement.IsGrounded(m_rb, floorBoxcastPos, m_enemyType.m_groundCheckDimensions, m_wallLayer);


            //If the attack is finished, and no longer running
            if (m_currentAttackState == AI_AttackType_Base.AttackState.Finished)
            {
                if (!PlayerInRadius())
                {
                    Debug.Log("Player Gone");
                    target = null;
                }
                else
                {
                    //Restart the attack
                    m_enemyType.m_attackType.StartAttack(this, m_rb, target, gameObject, m_gun);
                    m_attackTargetPos = m_enemyType.m_attackType.SetAttackTargetPosition(this, gameObject, target);
                }
            }

            //If the attack is running
            else
            {

                //If the attack has ceased
                if (m_enemyType.m_attackType.AttackFinished(this, m_rb, m_attackTargetPos, target, gameObject, m_bulletOrigin, m_gun))
                {
                    //If the player is no lnger in the radius, set the target to null
                    if (!PlayerInRadius())
                    {
                        Debug.Log("Player Gone 2");
                        target = null;
                    }
                }
                else
                {
                    //If the player has moved a set amount of distance (set in the attack type), recalculate the attackTarget Position
                    if (PlayerMoved())
                    {
                        m_attackTargetPos = m_enemyType.m_attackType.SetAttackTargetPosition(this, gameObject, target);
                    }
                }
            }
        }

        else
        {
            PerformIdleMovement();
        }
    }


    #region Attacking Functions


    ///<Summary>
    ///Checks the shooting pattern, to see if the ai can currently shoot
    public bool CanFireGun()
    {
        if (m_currentShootTimer <= m_currentShootDelay)
        {
            m_currentShootTimer += Time.deltaTime;
            return m_isShooting;
        }
        else
        {
            return !m_isShooting;
        }
    }

    ///<Summary>
    ///Allows the AI to stop shooting, allowing for patterns
    public void SwapShooting(bool p_isShooting, float p_newDelay)
    {
        if (p_isShooting != m_isShooting)
        {
            m_isShooting = p_isShooting;
            m_currentShootDelay = p_newDelay;
            m_currentShootTimer = 0;
        }

    }

    ///<Summary>
    ///If the player has moved a set distance from a spot
    bool PlayerMoved()
    {
        if (Vector3.Distance(target.transform.position, m_delayedPlayerPosition) > m_enemyType.m_attackType.m_playerMoveDistanceReaction)
        {
            m_delayedPlayerPosition = target.transform.position;
            return true;
        }
        return false;
    }

    ///<Summary>
    ///Checks if the player is in a set radius
    ///TODO: Create the following function
    bool PlayerInRadius()
    {
        if(Vector3.Distance(transform.position,target.transform.position) < m_enemyType.m_attackType.m_attackRadius){
            return true;
        }else{
            return false;
        }
    }

    #endregion


    #region Move Functions
    void PerformIdleMovement()
    {

        //If there exists patrol points
        if (m_patrolPoints.Count > 0)
        {
            if (CloseToPoint(currentPatrolPoint.position))
            {
                currentPatrolPoint = NewPatrolPoint();
            }

            m_enemyType.m_idleMovementType.PerformIdleMovement(m_agent, m_rb, transform, m_currentForward, currentPatrolPoint.position, m_isGrounded);

        }
        else
        {
            m_enemyType.m_idleMovementType.PerformIdleMovement(m_agent, m_rb, transform, m_currentForward, transform.position, m_isGrounded);
        }



        //Check for walls infront, and check if gronded
        Vector2 circleCastPos = new Vector2(transform.position.x - m_circleCastOffset.x, transform.position.y - m_circleCastOffset.y);
        
        Vector2 floorBoxcastPos = transform.position;
        m_isGrounded = m_enemyType.m_idleMovementType.m_movementType.IsGrounded(m_rb, floorBoxcastPos, m_enemyType.m_groundCheckDimensions, m_wallLayer);

        if (m_enemyType.m_idleMovementType.m_movementType.WallInFront(this, m_rb, circleCastPos,m_circleCastRad, m_currentForward, m_movementFlipLayer, m_isGrounded))
        {

            FlipEnemy(m_currentForward * -1);
            m_isStuck = false;
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
    }

    void OnDisable()
    {
        if (m_spawnerManager != null)
        {
            if (m_isPooled)
            {
                m_spawnerManager.m_currentEnemiesInRoom.Remove(this);
                m_patrolPoints.Clear();
            }
        }

    }


    void OnTriggerStay2D(Collider2D other)
    {
        if(target != null){
            return;
        }
        if (other.gameObject.tag == playerTag)
        {
            
            float dis = Vector3.Distance(transform.position, other.gameObject.transform.position);
            print("Player Detected | Dis: " + dis);
            if ( dis< m_enemyType.m_attackType.m_attackRadius)
            {
                print("Player in range");
                target = other.gameObject;
            }
        }
    }
}
