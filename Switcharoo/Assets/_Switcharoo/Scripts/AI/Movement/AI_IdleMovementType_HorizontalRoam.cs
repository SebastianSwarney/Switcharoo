using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_IdleMovement_HorizontalRoam", menuName = "AI/IdleMovement/HorizontalRoam", order = 0)]
public class AI_IdleMovementType_HorizontalRoam : AI_IdleMovementType_Base
{
    public override void PerformIdleMovement(AiController p_aiController, Ai_Pathfinding_Agent p_agent,Rigidbody2D p_rb, Transform p_enemyObject, int p_forwardDir, Vector3 p_targetPos, bool p_isGrounded)
    {
        m_movementType.MoveToPosition(p_aiController, p_rb, p_agent, p_enemyObject.position, (p_enemyObject.right * p_forwardDir) + p_enemyObject.position,p_isGrounded);
    }
}
