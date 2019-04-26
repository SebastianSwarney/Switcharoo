using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot", menuName = "Scriptable Objects/AttackType/Shoot", order = 0)]
public class AttackType_Shoot : AttackType_Base
{
    [Header("Shoot-Only variables")]
    public float m_distanceFromPlayer;

    //public ShotType_Base m_shotType;
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin)
    {
        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:
                Debug.Log("Start Attack");
                VisualTell(p_aiController, m_tellTime);
                break;

            case AttackState.Perform:

                if (PlayerInRange(p_player, p_enemyObject))
                {
                    m_attackMovement.MoveToPosition(p_rb, p_enemyObject.transform.position, p_targetPos);
                }
                break;

            case AttackState.Finished:
                Debug.Log("Attack Finished");
                p_aiController.target = null;
                return true;
        }
        return false;
    }

    public override Vector3 SetAttackTargetPosition(GameObject p_enemyObject, GameObject p_player)
    {
        Vector3 dir = (p_player.transform.position - p_enemyObject.transform.position).normalized;

        Vector3 targetPos = p_player.transform.position - dir * m_distanceFromPlayer;
        return m_attackMovement.ConvertRelativePosition(p_enemyObject,targetPos);
    }

    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject)
    {
        p_aiController.m_currentAttackState = AttackState.Start;
    }
}
