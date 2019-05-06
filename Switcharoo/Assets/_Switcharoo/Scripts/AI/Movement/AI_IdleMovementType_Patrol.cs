using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AI_IdleMovement_Patrol", menuName = "AI/IdleMovement/Patrol", order = 0)]
public class AI_IdleMovementType_Patrol : AI_IdleMovementType_Base
{
    public override void PerformIdleMovement(Ai_Pathfinding_Agent p_agent,Rigidbody2D p_rb, Transform p_enemyObject, int p_forwardDir, Vector3 m_targetPos, bool p_isGrounded)
    {
        m_movementType.MoveToPosition(p_rb,p_agent,p_enemyObject.position,m_targetPos, p_isGrounded);
        
    }
}
