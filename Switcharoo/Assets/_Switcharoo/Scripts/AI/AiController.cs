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

    #region Components on the Enemy

    Rigidbody2D m_rb;
    ShootController m_gun;

    public Transform m_bulletOrigin;
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

    public float m_stuckMoveTime = 3;
    float m_stuckTimer;
    public bool m_isStuck;




    public bool m_isGrounded;

    #endregion

    #region Managing Variables
    [HideInInspector]
    public AI_Spawner_Manager_Base m_spawnerManager;
    #endregion

    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_gun = GetComponent<ShootController>();
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
    }
    void CheckState()
    {
        if (m_isStuck)
        {
            m_stuckTimer += Time.deltaTime;
            if (m_stuckTimer > m_stuckMoveTime)
            {
                FlipEnemy(m_currentForward * -1);
                m_isStuck = false;
            }
        }
        else
        {
            m_stuckTimer = 0;
        }
        if (target != null)
        {
            //If the attack is finished, and no longer running
            if (m_currentAttackState == AI_AttackType_Base.AttackState.Finished)
            {
                //Restart the attack
                m_enemyType.m_attackType.StartAttack(this, m_rb, target, gameObject, m_gun);
                m_attackTargetPos = m_enemyType.m_attackType.SetAttackTargetPosition(gameObject, target);
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
                        target = null;
                    }
                }
                else
                {
                    //If the player has moved a set amount of distance (set in the attack type), recalculate the attackTarget Position
                    if (PlayerMoved())
                    {
                        m_attackTargetPos = m_enemyType.m_attackType.SetAttackTargetPosition(gameObject, target);
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
        return false;
    }

    #endregion


    #region Move Functions
    void PerformIdleMovement()
    {
        m_enemyType.m_idleMovementType.PerformIdleMovement(m_rb, transform, m_currentForward);
        Vector2 wallBoxcastPos = new Vector2(transform.position.x + (m_enemyType.m_enemyDimensions.x / 2) * m_currentForward, transform.position.y);
        Vector2 floorBoxcastPos = transform.position;//new Vector2(transform.position.x, transform.position.y - m_enemyDimensions.y/2);
        m_isGrounded = m_enemyType.m_idleMovementType.m_movementType.IsGrounded(m_rb, floorBoxcastPos, m_enemyType.m_groundCheckDimensions, m_wallLayer);
        if (m_enemyType.m_idleMovementType.m_movementType.WallInFront(this, m_rb, wallBoxcastPos, m_enemyType.m_enemyDimensions, m_currentForward, m_movementFlipLayer, m_isGrounded))
        {

            FlipEnemy(m_currentForward * -1);
            m_isStuck = false;
        }

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
            m_spawnerManager.m_currentEnemiesInRoom.Remove(this);
        }

    }
}
