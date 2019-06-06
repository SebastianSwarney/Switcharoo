using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
    /*  Idle movement behaviours
        As some of these behaviours can be share throughout the different movement types
        idle behaviours have become their own scriptable objects
        They do NOT have access to the player
    */
///<Summary>
public abstract class AI_IdleMovementType_Base : ScriptableObject
{
    public AI_MovementType_Base m_movementType;

    public abstract void PerformIdleMovement(AiController p_aiController, Ai_Pathfinding_Agent p_agent,Rigidbody2D p_rb, Transform p_enemyObject, int p_forwardDir, Vector3 m_targetPos, bool p_isGrounded);
}
