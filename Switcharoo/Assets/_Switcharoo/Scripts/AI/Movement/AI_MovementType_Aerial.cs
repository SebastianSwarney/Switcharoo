﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///The aerial movement pattern, used for flying enemeis
///<Summary>

[CreateAssetMenu(fileName = "AI_MovementType_Aerial", menuName = "AI/Movement/Aerial", order = 0)]
public class AI_MovementType_Aerial : AI_MovementType_Base
{

    ///<Summary>
    ///Returns the target position in a positon that the enemy can reach
    public override Vector3 ConvertRelativePosition(Ai_Pathfinding_Agent p_agent,GameObject p_enemyObject, Vector3 p_convertPos)
    {
        return p_convertPos;
    }



    ///<Summary>
    ///Moves the enemy to a given position
    public override void MoveToPosition(Rigidbody2D p_rb, Ai_Pathfinding_Agent p_agent,Vector3 p_startPos, Vector3 p_targetPosition, bool p_isGrounded)
    {
        ///Removes gravity, to allow for flight
        p_rb.gravityScale = 0;
        Vector3 dir = (p_targetPosition - p_startPos).normalized;
        p_rb.velocity = dir * m_speed;
    }

    ///<Summary>
    ///Determines if the postion has been reached
    public override bool PostionReached(Ai_Pathfinding_Agent p_agent,GameObject p_enemyObject, Vector3 p_targetPos, float p_stoppingDistance)
    {
        if (Vector3.Distance(p_targetPos, p_enemyObject.transform.position) <= p_stoppingDistance)
        {
            return true;
        }
        return false;
    }



    public override bool WallInFront(AiController p_aiCont, Rigidbody2D p_rb, Vector2 p_castPos, float p_circleCastRad, int p_forwardDir, LayerMask p_hitLayer, bool p_isGrounded)
    {
        
        return false;
    }

    public override bool IsGrounded(Rigidbody2D p_rb, Vector2 p_boxcastPos,  Vector2 p_raycastDimensions, LayerMask p_wallLayer)
    {
        return false;
    }

    public override void VisualTellMovement(Rigidbody2D p_rb)
    {
        p_rb.velocity = Vector3.zero;
    }
}
