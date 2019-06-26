using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///Movement for grounded enemies
///<Summary>

[CreateAssetMenu(fileName = "AI_MovementType_Grounded", menuName = "AI/Movement/Grounded", order = 0)]
public class AI_MovementType_Grounded : AI_MovementType_Base
{
    [Header("Ground Type Variables")]
    public bool m_canJump;
    public float m_jumpTurnDistance;
    public float m_gravityValue, m_jumpHeight;

    ///<Summary>
    ///Converts the target position so that the enemy can reach it
    public override Vector3 ConvertRelativePosition(Ai_Pathfinding_Agent p_agent, GameObject p_enemyObject, Vector3 p_convertPos)
    {
        ///Flattens the position, so that its on the same y level as the enemy
        return new Vector3(p_convertPos.x, p_enemyObject.transform.position.y, 0f);
    }




    ///<Summary>
    ///The movement of the enemy
    public override void MoveToPosition(AiController p_aiCont, float p_speed, Rigidbody2D p_rb, Ai_Pathfinding_Agent p_agent, Vector3 p_startPos, Vector3 p_targetPos, bool p_isGrounded)
    {

        if (!p_aiCont.m_jumpAnim)
        {
            ///Sets gravity to be active
            p_rb.gravityScale = 1;
            Vector3 dir = p_targetPos - p_startPos;
            dir = new Vector3(Mathf.Sign(dir.x) * p_speed, p_rb.velocity.y, 0f);
            p_rb.velocity = dir;
            if (p_aiCont.m_aiBounds != null)
            {
                if (!p_aiCont.m_aiBounds.TargetInBounds(p_aiCont.transform.position + (p_targetPos - p_startPos).normalized * p_aiCont.m_checkWallDistance/2))
                {
                    p_rb.velocity = new Vector3(0f, p_rb.velocity.y, 0f);

                }
            }

            ///Only travels on the x axis
            if (Mathf.Sign(dir.x) != Mathf.Sign(p_aiCont.m_currentForward))
            {
                p_aiCont.FlipEnemy((int)Mathf.Sign(dir.x));
            }
        }
        else
        {

            p_rb.velocity = Vector3.zero;
            if (p_aiCont.m_beginJump)
            {
                PerformJump(p_rb);
                p_aiCont.m_jumpAnim = false;
                p_aiCont.m_beginJump = false;
            }
        }
        

    }

    ///<Summary>
    ///Determines if the position has been reached
    public override bool PostionReached(Ai_Pathfinding_Agent p_agent, GameObject p_enemyObject, Vector3 p_targetPos, float p_stoppingDistance)
    {
        if (Mathf.Abs(p_targetPos.x - p_enemyObject.transform.position.x) <= p_stoppingDistance)
        {
            return true;
        }
        return false;
    }

    

    public override bool WallInFront(AiController p_aiCont, Rigidbody2D p_rb, Vector2 p_castPos, float p_circleCastRad, int p_forwardDir, LayerMask p_hitLayer, bool p_isGrounded)
    {
        if (p_aiCont.m_aiBounds != null)
        {
            
            Vector3 checkDistance = p_aiCont.m_aiBounds.PositionInBounds((Vector2)p_aiCont.transform.position + (Vector2.right * p_forwardDir) * ((p_aiCont.m_checkWallDistance)*2 - (p_circleCastRad / 2)));


            if (Vector3.Distance(p_aiCont.transform.position, checkDistance) < p_aiCont.m_checkWallDistance)
            {
                if (m_canJump)
                {
                    if (p_isGrounded)
                    {
                        JumpAnim(p_aiCont, p_rb);
                        return false;
                    }
                    else
                    {
                        float distToObj = Vector3.Distance(p_rb.transform.position, checkDistance);

                        if (distToObj < m_jumpTurnDistance)
                        {
                            if (p_aiCont.m_debugPhysicsChecks)
                            {
                                Debug.DrawLine(p_rb.gameObject.transform.position + p_aiCont.m_spriteOffset, checkDistance, Color.green, .5f);
                            }

                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        RaycastHit2D[] hit = Physics2D.CircleCastAll(p_castPos, p_circleCastRad / 2, Vector2.right * p_forwardDir, p_aiCont.m_checkWallDistance - (p_circleCastRad / 2), p_hitLayer);



        foreach (RaycastHit2D rayHit in hit)
        {
            if (rayHit.transform.gameObject == p_rb.gameObject) continue;
            if (rayHit)
            {
                if (m_canJump)
                {
                    if (p_isGrounded)
                    {
                        JumpAnim(p_aiCont,p_rb);
                        return false;
                    }
                    else
                    {
                        float distToObj = Vector3.Distance(p_rb.transform.position, rayHit.point);

                        if (distToObj < m_jumpTurnDistance)
                        {
                            if (p_aiCont.m_debugPhysicsChecks)
                            {
                                Debug.DrawLine(p_rb.gameObject.transform.position + p_aiCont.m_spriteOffset, rayHit.point, Color.green, .5f);
                            }
                            
                            return true;
                        }
                        return false;
                    }
                }
                
                return true;
            }
        }

        

        return false;

    }

    public override bool IsGrounded(AiController p_aiCont, LayerMask p_hitLayer)
    {


        RaycastHit2D hit = Physics2D.BoxCast(p_aiCont.m_groundCheckPos + p_aiCont.transform.position, p_aiCont.m_groundCheckDimensions, 0, -Vector3.up, 0f, p_hitLayer);
        if (hit)
        {
            p_aiCont.EnemyGrounded(true);
        }
        return hit;
    }



    public override void VisualTellMovement(Rigidbody2D p_rb)
    {
        //p_rb.velocity = new Vector3(p_rb.velocity.x/2,p_rb.velocity.y,0f);
    }

    public override void StopMoving(Rigidbody2D p_rb)
    {
        p_rb.velocity = new Vector3(0f, p_rb.velocity.y,0f);
    }

    public void JumpAnim(AiController p_aiController, Rigidbody2D p_rb)
    {
        if (!p_aiController.m_isJumping)
        {
            p_aiController.EnemyJump();
            p_aiController.EnemyGrounded(false);
            p_aiController.m_jumpAnim = true;
        }
        
        
    }

    void PerformJump(Rigidbody2D p_rb)
    {
        float jumpForce = Mathf.Sqrt(2f * m_gravityValue * m_jumpHeight);
        p_rb.velocity = new Vector3(p_rb.velocity.x, jumpForce, 0);
    }
}
