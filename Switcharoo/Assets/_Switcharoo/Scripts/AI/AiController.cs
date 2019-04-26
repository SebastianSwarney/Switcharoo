using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///The brain of the AI. This is essentially an empty shell that requires components to function
public class AiController : MonoBehaviour
{

    public GameObject target;

    #region Components on the Enemy
    public AttackType_Base m_attackType;
    public MovementType_Base m_movementType;
    Rigidbody2D m_rb;
    ShootController m_gun;

    public Transform m_bulletOrigin;
    #endregion

    #region Attack Variables
    [Header("Attack State")]
    public AttackType_Base.AttackState m_currentAttackState = AttackType_Base.AttackState.Finished;

    Vector3 m_attackTargetPos;

    [HideInInspector]
    public Vector3 m_delayedPlayerPosition;

    [HideInInspector]
    public float m_visualTellTimer;
    float m_currentShootTimer, m_currentShootDelay;

    bool m_isShooting;
    #endregion
    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_gun = GetComponent<ShootController>();
    }
    private void Update()
    {
        CheckAttackState();
    }

    #region Attacking Functions
    void CheckAttackState()
    {
        if (target != null)
        {
            //If the attack is finished, and no longer running
            if (m_currentAttackState == AttackType_Base.AttackState.Finished)
            {
                //Restart the attack
                m_attackType.StartAttack(this, m_rb, target, gameObject, m_gun);
                m_attackTargetPos = m_attackType.SetAttackTargetPosition(gameObject, target);
            }

            //If the attack is running
            else
            {

                //If the attack has ceased
                if (m_attackType.AttackFinished(this, m_rb, m_attackTargetPos, target, gameObject, m_bulletOrigin, m_gun))
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
                        m_attackTargetPos = m_attackType.SetAttackTargetPosition(gameObject, target);
                    }
                }
            }
        }
    }

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
        if (Vector3.Distance(target.transform.position, m_delayedPlayerPosition) > m_attackType.m_playerMoveDistanceReaction)
        {
            print("Player Moved");
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
}
