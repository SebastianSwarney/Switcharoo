﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///The shooting behaviour used for enemies with guns
///<Summary>

[CreateAssetMenu(fileName = "Shoot", menuName = "Scriptable Objects/AttackType/Shoot", order = 0)]
public class AttackType_Shoot : AttackType_Base
{
    [Header("Shoot-Only variables")]
    public float m_distanceFromPlayer;
    public ShotType_Base m_shotType;
    public float m_shootIntervalTime, m_shootBreakTime;

    ///<Summary>
    ///Where all the attack logic is
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun)
    {
        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:
                //Starts the visual tell
                VisualTell(p_aiController);
                break;

            case AttackState.Perform:

                //If the player is in range, perform the attack
                if (PlayerInRange(p_player, p_enemyObject))
                {

                    AimAtTarget(p_bulletOrigin, p_player.transform.position);

                    ///Creates bursts between the bullets
                    if (CanFireWeapon(p_aiController))
                    {
                        p_gun.Shoot(p_bulletOrigin);
                        p_aiController.SwapShooting(true, m_shootIntervalTime);
                    }
                    else
                    {
                        p_aiController.SwapShooting(false, m_shootBreakTime);
                    }

                    ///If they've havent reached the position, move to it still
                    if (!m_attackMovement.PostionReached(p_enemyObject, p_targetPos, m_targetStoppingDistance))
                    {
                        m_attackMovement.ConvertRelativePosition(p_enemyObject, p_targetPos);
                        m_attackMovement.MoveToPosition(p_rb, p_enemyObject.transform.position, p_targetPos);
                    }
                }

                ///If the player gets out of range, end the attack
                else
                {
                    p_aiController.m_currentAttackState = AttackState.Finished;
                }

                break;

            case AttackState.Finished:
                p_aiController.target = null;
                return true;
        }
        return false;
    }


    //Generates the target to move to during the attack
    public override Vector3 SetAttackTargetPosition(GameObject p_enemyObject, GameObject p_player)
    {
        Vector3 dir = (p_player.transform.position - p_enemyObject.transform.position).normalized;

        Vector3 targetPos = p_player.transform.position - dir * m_distanceFromPlayer;
        return m_attackMovement.ConvertRelativePosition(p_enemyObject, targetPos);
    }


    //Initialize the attack
    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun)
    {
        p_gun.m_shotType = m_shotType;
        p_aiController.m_currentAttackState = AttackState.Start;
    }

    //Aim the gun
    void AimAtTarget(Transform p_bulletOrigin, Vector3 p_targetPos)
    {
        Vector3 dir = p_targetPos - p_bulletOrigin.transform.position;

        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        p_bulletOrigin.rotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);


    }

    //Determines if the weapon can be fired
    bool CanFireWeapon(AiController p_aiCont)
    {
        return p_aiCont.CanFireGun();
    }


}
