using System.Collections;
using System.Collections.Generic;
using UnityEngine;



///<Summary>
///The behviour that performs the melee enemies
///<Summary>

[CreateAssetMenu(fileName = "AI_AttackType_Collide", menuName = "AI/AttackType/Collide", order = 0)]
public class AI_AttackType_Collide : AI_AttackType_Base
{
    [Header("Melee-Only Variables")]
    public float m_targetOvershootPlayerDistance;   //Used for if the target overshoots the player, and the player jumps over them
    public bool m_jumpAtPlayer;
    public float m_jumpHeight;
    public float m_jumpDistanceFromPlayer;
    public float m_gravityValue;
    ///<Summary>
    ///Where all the attack logic is
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun)
    {
        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:

                //Perform the visual tell
                VisualTell(p_aiController, p_rb);
                p_rb.velocity = new Vector3(0f, p_rb.velocity.y, 0f);
                p_aiController.PlayerSpotted(true);
                break;

            case AttackState.Perform:

                //If the player is in range, set a position that is in their direction
                if (PlayerInRange(p_player, p_enemyObject, p_aiController.m_enemyType.m_detectionRadius))
                {
                    m_attackMovement.MoveToPosition(p_aiController,p_aiController.m_attackSpeed, p_rb,p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos,p_aiController.m_isGrounded);

                    if (m_jumpAtPlayer)
                    {
                        if (Vector3.Distance(p_enemyObject.transform.position, m_attackMovement.ConvertRelativePosition(p_aiController.m_agent, p_enemyObject, p_player.transform.position)) < m_jumpDistanceFromPlayer)
                        {
                            
                            if (p_aiController.m_isGrounded)
                            {
                                
                                Jump(p_aiController,p_rb);
                            }
                        }
                    }
                    

                    //If the enemy reaches that position, end the current attack
                    if (m_attackMovement.PostionReached(p_aiController.m_agent,p_enemyObject, p_targetPos, m_targetStoppingDistance))
                    {
                        p_aiController.m_currentAttackState = AttackState.Finished;
                    }
                }

                //If the player is not in range, end the attack
                else
                {
                    p_aiController.m_target = null;
                    p_aiController.PlayerSpotted(false);
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
    public override Vector3 SetAttackTargetPosition(AiController p_aiCont,GameObject p_enemyObject, GameObject p_player)
    {
        Vector3 playerPos = m_attackMovement.ConvertRelativePosition(p_aiCont.m_agent,p_enemyObject, p_player.transform.position);
        Vector3 enemyPos = m_attackMovement.ConvertRelativePosition(p_aiCont.m_agent,p_enemyObject, p_enemyObject.transform.position);
        Vector3 dir = (playerPos - enemyPos).normalized;
        dir = playerPos + dir * m_targetOvershootPlayerDistance;

        return dir;
    }

    ///<Summary>
    //Start the attack
    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun)
    {
        p_aiController.m_currentAttackState = AttackState.Start;
    }


    void Jump(AiController aiCont, Rigidbody2D p_rb)
    {
        float jumpForce = Mathf.Sqrt(2f * m_gravityValue * m_jumpHeight);
        p_rb.velocity = new Vector3(p_rb.velocity.x, jumpForce, 0);
        aiCont.EnemyGrounded(false);
        aiCont.EnemyJump();
    }


}
