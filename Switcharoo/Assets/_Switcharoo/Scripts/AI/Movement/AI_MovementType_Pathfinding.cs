using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AI_MovementType_Pathfinder", menuName = "AI/Movement/Pathfinder", order = 0)]
public class AI_MovementType_Pathfinding : AI_MovementType_Base
{
    [Header("Pathfinding only variables")]
    public float m_maxJumpHeight;
    public float m_recalculateDistance;
    #region Modular Methods
    public override Vector3 ConvertRelativePosition(Ai_Pathfinding_Agent p_agent, GameObject p_enemyObject, Vector3 p_convertPos)
    {


        return p_convertPos;
    }

    public override bool IsGrounded(AiController p_aiCont, LayerMask p_wallLayer)
    {
        RaycastHit2D hit = Physics2D.BoxCast(p_aiCont.m_groundCheckPos + p_aiCont.transform.position, p_aiCont.m_groundCheckDimensions, 0, -Vector3.up, 0f, p_wallLayer);
        if (!p_aiCont.m_isGrounded && hit)
        {
            p_aiCont.EnemyGrounded(true);

            p_aiCont.m_isJumping = false;

        }
        

        return hit;
    }

    public override void MoveToPosition(AiController p_aiCont,float p_speed, Rigidbody2D p_rb, Ai_Pathfinding_Agent p_agent, Vector3 p_startPos, Vector3 p_targetPosition, bool p_isGrounded)
    {
        if (!p_agent.TargetPositionReached(p_targetPosition))
        {

            if (p_agent.m_currentNode == null)
            {
                //Create a path to the current node
                p_agent.CreatePath(p_startPos, p_targetPosition, m_maxJumpHeight);
            }
            else if (Vector3.Distance((Vector2)p_rb.position, (Vector2)p_agent.m_currentNode.m_worldPos) > m_recalculateDistance)
            {
                Node occupiedNode = p_agent.m_navGrid.NodeFromWorldPoint(p_rb.position);
                if (occupiedNode.m_worldPos.y < p_agent.m_currentNode.m_worldPos.y)
                {
                    p_agent.CreatePath(p_startPos, p_targetPosition, m_maxJumpHeight);
                }
                else
                {
                    bool containsCurrentNode = false;
                    foreach (Node.NodeConnection connect in p_agent.m_currentNode.m_connectedTo)
                    {
                        if (connect.m_connectedTo == occupiedNode)
                        {
                            containsCurrentNode = true;
                        }
                    }
                    if (!containsCurrentNode)
                    {
                        p_agent.CreatePath(p_startPos, p_targetPosition, m_maxJumpHeight);
                    }
                }

            }

            if (p_agent.TargetMoved(p_targetPosition))
            {
                //If the target has moved a signifcant distance away, recreate path
                p_agent.CreatePath(p_startPos, p_targetPosition, m_maxJumpHeight);
            }
            else
            {
                p_agent.MoveToNode(p_speed, m_maxJumpHeight, p_isGrounded);


            }
        }
        else
        {

            p_rb.velocity = new Vector3(0, p_rb.velocity.y, 0f);
        }
    }



    public override bool PostionReached(Ai_Pathfinding_Agent p_agent, GameObject p_enemyObject, Vector3 p_targetPos, float p_stoppingDistance)
    {
        bool oi = (p_agent.TargetPositionReached(p_targetPos));
        return oi;
    }

    public override void VisualTellMovement(Rigidbody2D p_rb)
    {

    }

    public override bool WallInFront(AiController p_aiCont, Rigidbody2D p_rb, Vector2 p_castPos, float p_circleCastRad, int p_forwardDir, LayerMask p_hitLayer, bool p_isGrounded)
    {
        return false;
    }
    public override void StopMoving(Rigidbody2D p_rb)
    {
        p_rb.velocity = new Vector3(0f, p_rb.velocity.y, 0f);
    }

    #endregion

}
