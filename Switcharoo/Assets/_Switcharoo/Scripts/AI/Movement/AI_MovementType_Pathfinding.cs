using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AI_MovementType_Pathfinder", menuName = "AI/Movement/Pathfinder", order = 0)]
public class AI_MovementType_Pathfinding : AI_MovementType_Base
{

    public float m_jumpHeight;
    #region Modular Methods
    public override Vector3 ConvertRelativePosition(Ai_Pathfinding_Agent p_agent, GameObject p_enemyObject, Vector3 p_convertPos)
    {

        return p_convertPos;
    }

    public override bool IsGrounded(Rigidbody2D p_rb, Vector2 p_boxcastPos, Vector2 p_raycastDimensions, LayerMask p_wallLayer)
    {
        RaycastHit2D hit = Physics2D.BoxCast(p_boxcastPos, p_raycastDimensions, 0, -Vector3.up, p_raycastDimensions.y / 2, p_wallLayer);
        return hit;
    }

    public override void MoveToPosition(Rigidbody2D p_rb, Ai_Pathfinding_Agent p_agent, Vector3 p_startPos, Vector3 p_targetPosition, bool p_isGrounded)
    {
        if (!p_agent.TargetPositionReached(p_targetPosition))
        {
            if (p_agent.m_currentNode == null)
            {

                p_agent.CreatePath(p_startPos, p_targetPosition, m_jumpHeight);
            }

            if (p_agent.TargetMoved(p_targetPosition))
            {

                p_agent.CreatePath(p_startPos, p_targetPosition, m_jumpHeight);
            }
            else
            {
                if (p_agent.m_currentNode.m_worldPos.x - p_rb.position.x > p_agent.m_stoppingDistance)
                {

                }
                p_agent.MoveToNode(m_speed, m_jumpHeight, p_isGrounded);


            }
        }
        else
        {
            p_rb.velocity = Vector3.zero;
        }
    }



    public override bool PostionReached(Ai_Pathfinding_Agent p_agent, GameObject p_enemyObject, Vector3 p_targetPos, float p_stoppingDistance)
    {
        bool oi = (p_agent.TargetPositionReached(p_targetPos));
        return oi;
    }

    public override bool WallInFront(AiController p_aiCont, Rigidbody2D p_rb, Vector2 p_boxcastPos, Vector2 p_raycastDimensions, int p_forwardDir, LayerMask p_wallLayer, bool p_isGrounded)
    {
        return false;
    }
    #endregion

}
