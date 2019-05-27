using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_AttackType_HeavyShoot", menuName = "AI/AttackType/HeavyShoot", order = 0)]
public class AI_AttackType_HeavyShoot : AI_AttackType_Base
{
    public enum HeavyAttackState { FarAttack, CloseAttack }

    [Header("Heavy-Only variables")]
    public float m_closeDistanceMax;
    public float m_closeShootIntervalTime, m_closeShootBreakTime, m_farShootIntervalTime, m_farShootBreakTime;
    public int m_closeFireAmount, m_farFireAmount;

    public float m_farShootAimUp;

    public AI_MovementType_Base m_closeShootingMovement, m_farShootingMovement;

    public ShootController.WeaponComposition m_closeShootComp, m_farShootComp;

    [Header("Stop moving at this distance")]
    public float m_maxDistanceFromOrigin;

    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun)
    {
        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:

                HeavyAttackPattern(p_enemyObject, p_player, p_gun);
                //Starts the visual tell
                VisualTell(p_aiController, p_rb);

                break;

            case AttackState.Perform:

                //If the player is in range, perform the attack
                if (PlayerInRange(p_player, p_enemyObject))
                {

                    AimAtTarget(p_aiController,p_bulletOrigin, p_player.transform.position,p_gun);

                    ///Creates bursts between the bullets
                    if (CanFireWeapon(p_aiController))
                    {
                        p_gun.Shoot(p_bulletOrigin);
                        if (p_aiController.CheckPatternType(IsCloseBehaviour(p_gun) ? m_closeFireAmount : m_farFireAmount, true))
                        {
                            p_aiController.m_currentAttackState = AttackState.Finished;
                            return true;
                        }
                        p_aiController.SwapShooting(true, (IsCloseBehaviour(p_gun)) ? m_closeShootIntervalTime : m_farShootIntervalTime);

                        //Different movement for when they are shooting

                        if (IsCloseBehaviour(p_gun))
                        {
                            if (!m_closeShootingMovement.PostionReached(p_aiController.m_agent, p_enemyObject, p_targetPos, m_targetStoppingDistance))
                            {
                                Vector3 dir = (p_targetPos - p_enemyObject.transform.position).normalized;
                                if (Vector3.Distance(p_enemyObject.transform.position + dir, p_aiController.m_originPoint.position) < m_maxDistanceFromOrigin)
                                {
                                    m_closeShootingMovement.ConvertRelativePosition(p_aiController.m_agent, p_enemyObject, p_targetPos);
                                    m_closeShootingMovement.MoveToPosition(p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos, p_aiController.m_isGrounded);
                                }
                                else
                                {
                                    m_closeShootingMovement.StopMoving(p_rb);
                                }

                            }
                            else
                            {
                                m_closeShootingMovement.StopMoving(p_rb);
                            }
                        }
                        else
                        {
                            if (!m_farShootingMovement.PostionReached(p_aiController.m_agent, p_enemyObject, p_targetPos, m_targetStoppingDistance))
                            {
                                Vector3 dir = (p_targetPos - p_enemyObject.transform.position).normalized;
                                if (Vector3.Distance(p_enemyObject.transform.position + dir, p_aiController.m_originPoint.position) < m_maxDistanceFromOrigin)
                                {
                                    m_farShootingMovement.ConvertRelativePosition(p_aiController.m_agent, p_enemyObject, p_targetPos);
                                    m_farShootingMovement.MoveToPosition(p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos, p_aiController.m_isGrounded);
                                }
                                else
                                {
                                    m_farShootingMovement.StopMoving(p_rb);
                                }

                            }
                            else
                            {
                                m_farShootingMovement.StopMoving(p_rb);
                            }
                        }

                    }
                    else
                    {
                        p_aiController.SwapShooting(false, IsCloseBehaviour(p_gun) ? m_closeShootBreakTime : m_farShootBreakTime);
                        ///If they've havent reached the position, move to it still
                        if (!m_attackMovement.PostionReached(p_aiController.m_agent, p_enemyObject, p_targetPos, m_targetStoppingDistance))
                        {
                            Vector3 dir = (p_targetPos - p_enemyObject.transform.position).normalized;
                            if (Vector3.Distance(p_enemyObject.transform.position + dir, p_aiController.m_originPoint.position) < m_maxDistanceFromOrigin)
                            {
                                p_targetPos = m_attackMovement.ConvertRelativePosition(p_aiController.m_agent, p_enemyObject, p_targetPos);
                                m_attackMovement.MoveToPosition(p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos, p_aiController.m_isGrounded);
                            }
                            else
                            {
                                m_attackMovement.StopMoving(p_rb);
                            }
                        }
                        else
                        {
                            m_attackMovement.StopMoving(p_rb);
                        }
                    }


                }

                ///If the player gets out of range, end the attack
                else
                {
                    p_aiController.m_currentAttackState = AttackState.Finished;
                }

                break;

            case AttackState.Finished:
                p_aiController.m_target = null;
                return true;
        }
        return false;
    }


    public override Vector3 SetAttackTargetPosition(AiController p_aiCont, GameObject p_enemyObject, GameObject p_player)
    {
        return m_attackMovement.ConvertRelativePosition(p_aiCont.m_agent, p_enemyObject, p_player.transform.position);
    }

    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun)
    {
        p_aiController.m_currentAttackState = AttackState.Start;


    }

    public void AimAtTarget(AiController p_aiController,Transform p_bulletOrigin, Vector3 p_targetPos, ShootController p_gun)
    {
        Vector3 dir = p_targetPos - p_bulletOrigin.transform.position;

        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        p_bulletOrigin.rotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);
        if (!IsCloseBehaviour(p_gun))
        {

            p_bulletOrigin.rotation = Quaternion.AngleAxis(lookAngle + Mathf.Sign(dir.x) * m_farShootAimUp, Vector3.forward);

        }

        Debug.DrawLine(p_bulletOrigin.position, (p_bulletOrigin.right * 9 + p_bulletOrigin.position), Color.red);

    }

    bool CanFireWeapon(AiController p_aiCont)
    {
        return p_aiCont.CanFireGun();
    }


    void HeavyAttackPattern(GameObject p_enemyObject, GameObject p_targetObject, ShootController p_gun)
    {

        if (Vector3.Distance(p_enemyObject.transform.position, p_targetObject.transform.position) > m_closeDistanceMax)
        {
            //Farther than the close behaviour
            SwitchShootPattern(m_farShootComp, p_gun);

        }
        else
        {
            //Is the close behaviour
            SwitchShootPattern(m_closeShootComp, p_gun);
        }
    }
    void SwitchShootPattern(ShootController.WeaponComposition p_newComp, ShootController p_gun)
    {
        p_gun.m_currentWeaponComposition = p_newComp;
        Debug.Log("New Behave: " + p_newComp.m_shotPattern);
    }

    bool IsCloseBehaviour(ShootController p_gun)
    {
        return p_gun.m_currentWeaponComposition.m_shotPattern == m_closeShootComp.m_shotPattern;
    }


}
