using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_AttackType_HeavyShoot", menuName = "AI/AttackType/HeavyShoot", order = 0)]
public class AI_AttackType_HeavyShoot : AI_AttackType_Base
{
    public enum HeavyAttackState { FarAttack, CloseAttack }

    [Header("Heavy-Only variables")]
    public float m_closeDistanceMax;
    public int m_closeShootBulletCount, m_farShootBulletCount;
    public float m_closeShootTriggerTime, m_farShootTriggerTime;

    public float m_closeShootBreakTime, m_farShootBreakTime;



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

                if (p_aiController.m_canSwitchToAlt)
                {
                    HeavyAttackPattern(p_enemyObject, p_player, p_gun);
                    Debug.Log("Change Fire");
                    p_aiController.m_shootBreakTime = (IsCloseBehaviour(p_gun) ? m_closeShootBreakTime : m_farShootBreakTime);
                    p_aiController.m_shootTriggerTime = (IsCloseBehaviour(p_gun) ? m_closeShootTriggerTime : m_farShootTriggerTime);
                    p_aiController.m_bulletsPerPattern = (IsCloseBehaviour(p_gun) ? m_closeShootBulletCount : m_farShootBulletCount);
                    p_aiController.m_fireAlt = IsCloseBehaviour(p_gun) ? false : true;
                }
                //Starts the visual tell
                VisualTell(p_aiController, p_rb);

                break;

            case AttackState.Perform:





                AimAtTarget(p_aiController, p_aiController.m_shootAltOrigin, p_player.transform.position, p_gun);
                AimAtTarget(p_aiController, p_aiController.m_bulletOrigin, p_player.transform.position, p_gun);
                //If the player is in range, perform the attack
                if (PlayerInRange(p_aiController, p_player, p_enemyObject, p_aiController.m_enemyType.m_detectionRadius))
                {



                    //Stop Moving after a certain point
                    if (!m_closeShootingMovement.PostionReached(p_aiController.m_agent, p_enemyObject, p_targetPos, m_targetStoppingDistance))
                    {
                        Vector3 dir = (p_targetPos - p_enemyObject.transform.position).normalized;
                        if (Vector3.Distance(p_enemyObject.transform.position + dir, p_aiController.m_aiBounds.transform.position) < p_aiController.m_aiBounds.m_boundsDimensions.x/2)
                        {
                            m_closeShootingMovement.ConvertRelativePosition(p_aiController.m_agent, p_enemyObject, p_targetPos);
                            m_closeShootingMovement.MoveToPosition(p_aiController, p_aiController.m_attackSpeed, p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos, p_aiController.m_isGrounded);


                            //Debug.Log("Fire");
                            p_aiController.m_canSwitchToAlt = false;
                            p_aiController.ChangeAnimation(false);
                        }
                        else
                        {

                            p_aiController.ChangeAnimation(true);
                            m_closeShootingMovement.StopMoving(p_rb);
                        }

                    }
                    else
                    {

                        p_aiController.ChangeAnimation(true);
                        m_closeShootingMovement.StopMoving(p_rb);
                    }

                }

                ///If the player gets out of range, end the attack
                else
                {
                    //Debug.Log("Heavy attack end");
                    p_aiController.m_currentAttackState = AttackState.Finished;
                    p_aiController.ChangeAnimation(false);
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

    public void AimAtTarget(AiController p_aiController, Transform p_bulletOrigin, Vector3 p_targetPos, ShootController p_gun)
    {
        Vector3 dir = p_targetPos - p_bulletOrigin.transform.position;

        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        p_bulletOrigin.rotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);
        if (!IsCloseBehaviour(p_gun))
        {

            p_bulletOrigin.rotation = Quaternion.AngleAxis(lookAngle + Mathf.Sign(dir.x) * m_farShootAimUp, Vector3.forward);

        }



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

    }

    bool IsCloseBehaviour(ShootController p_gun)
    {
        return p_gun.m_currentWeaponComposition.m_shotPattern == m_closeShootComp.m_shotPattern;
    }

    public override bool PlayerInRange(AiController p_aiCont, GameObject p_player, GameObject p_enemyObject, Vector2 p_detectionRange)
    {

        //return base.PlayerInRange(p_aiCont, p_player, p_enemyObject, p_detectionRange);
        if (p_player.transform.position.x > p_enemyObject.transform.position.x + p_detectionRange.x / 2 ||
                p_player.transform.position.x < p_enemyObject.transform.position.x - p_detectionRange.x / 2 ||
                p_player.transform.position.y > p_enemyObject.transform.position.y + p_detectionRange.y / 2 ||
                p_player.transform.position.y < p_enemyObject.transform.position.y - p_detectionRange.y / 2)
        {
            p_aiCont.PlayerSpotted(false);
            p_aiCont.ChangeAnimation(false);

            return false;
        }
        else
        {
            p_aiCont.PlayerSpotted(true);
            p_aiCont.ChangeAnimation(true);

            return true;
        }
    }

    public override void CheckForPlayer(AiController p_aiCont)
    {
        if (p_aiCont.m_target == null)
        {
            Collider2D playerCol = Physics2D.OverlapBox(p_aiCont.transform.position, p_aiCont.m_enemyType.m_detectionRadius, 0, p_aiCont.m_playerLayer);
            if (playerCol != null)
            {

                p_aiCont.m_target = playerCol.gameObject;
            }


        }
    }
}
