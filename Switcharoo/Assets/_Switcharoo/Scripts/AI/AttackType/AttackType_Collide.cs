﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Collide", menuName = "Scriptable Objects/AttackType/Collide", order = 0)]
public class AttackType_Collide : AttackType_Base
{
    [Header("Melee-Only Variables")]
    public float m_targetStoppingDistance;
    public float m_targetOvershootPlayerDistance;
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin)
    {
        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:
                VisualTell(p_aiController, m_tellTime);
                break;

            case AttackState.Perform:

                if (PlayerInRange(p_player, p_enemyObject))
                {
                    m_attackMovement.MoveToPosition(p_rb, p_enemyObject.transform.position, p_targetPos);
                    if(m_attackMovement.PostionReached(p_enemyObject, p_targetPos,m_targetStoppingDistance)){
                        p_aiController.m_currentAttackState = AttackState.Finished;
                    }
                }
                else
                {
                    p_aiController.target = null;
                    p_aiController.m_currentAttackState = AttackState.Finished;
                }
                break;

            case AttackState.Finished:
                return true;
        }
        return false;
    }

    public override Vector3 SetAttackTargetPosition(GameObject p_enemyObject, GameObject p_player)
    {
        Vector3 playerPos = m_attackMovement.ConvertRelativePosition(p_enemyObject,p_player.transform.position);
        Vector3 enemyPos = m_attackMovement.ConvertRelativePosition(p_enemyObject,p_enemyObject.transform.position);
        Vector3 dir = (playerPos - enemyPos).normalized;
        dir = playerPos + dir*m_targetOvershootPlayerDistance;
        
        return dir;
    }

    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject)
    {
        p_aiController.m_currentAttackState = AttackState.Start;
    }





}
