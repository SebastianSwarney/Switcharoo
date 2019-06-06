using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AI_AttackType_Collide - Jump", menuName = "AI/AttackType/Collide - Jump", order = 0)]
public class AI_AttackType_Collide_Jump : AI_AttackType_Base
{
    [Header("Jump Melee only variables")]
    public AI_MovementType_Grounded m_jumpMovement;
    public float m_jumpAttackDistance;


    ///<Summary>
    ///Where all the attack logic is
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun)
    {
        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:

                //Perform the visual tell
                VisualTell(p_aiController, p_rb);
                break;

            case AttackState.Perform:

                if (p_player.transform.position.x - p_aiController.transform.position.x  <= m_jumpAttackDistance && p_aiController.m_isGrounded)
                {
                    m_jumpMovement.Jump(p_rb);
                }
                else
                {
                    bool flipEntity = m_attackMovement.WallInFront(p_aiController, p_rb, new Vector2(p_aiController.transform.position.x - p_aiController.m_spriteOffset.x, p_aiController.transform.position.y - p_aiController.m_spriteOffset.y), p_aiController.m_circleCastRad, p_aiController.m_currentForward, p_aiController.m_wallLayer, p_aiController.m_isGrounded);
                }
                //If the player is in range, set a position that is in their direction
                if (PlayerInRange(p_player, p_enemyObject))
                {
                    m_attackMovement.MoveToPosition(p_aiController, p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos, p_aiController.m_isGrounded);



                    if (m_attackMovement.PostionReached(p_aiController.m_agent, p_enemyObject, p_targetPos, m_targetStoppingDistance))
                    {
                        p_aiController.m_currentAttackState = AttackState.Finished;
                    }
                }

                //If the player is not in range, end the attack
                else
                {
                    p_aiController.m_target = null;
                    p_aiController.m_currentAttackState = AttackState.Finished;
                }
                break;

            case AttackState.Finished:
                return true;
        }
        return false;
    }

    ///<Summary>
    ///Generates a target position that is a little behind the player, allowing for that overshooting
    public override Vector3 SetAttackTargetPosition(AiController p_aiCont, GameObject p_enemyObject, GameObject p_player)
    {
        Vector3 playerPos = m_attackMovement.ConvertRelativePosition(p_aiCont.m_agent, p_enemyObject, p_player.transform.position);

        return playerPos;
    }

    ///<Summary>
    //Start the attack
    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun)
    {
        p_aiController.m_currentAttackState = AttackState.Start;
    }
}
