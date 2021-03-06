﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///The scriptable object class used to createh the different movement behaviours
///<Summary>
public abstract class AI_MovementType_Base : ScriptableObject {
    public bool m_usesPathfinding;
    public abstract void MoveToPosition(AiController p_aiCont,float p_speed, Rigidbody2D p_rb,Ai_Pathfinding_Agent p_agent, Vector3 p_startPos,Vector3 p_targetPosition, bool p_isGrounded);

    public abstract bool PostionReached(Ai_Pathfinding_Agent p_agent,GameObject p_enemyObject,Vector3 p_targetPos, float p_stoppingDistance);

    public abstract Vector3 ConvertRelativePosition(Ai_Pathfinding_Agent p_agent,GameObject p_enemyObject,Vector3 p_convertPos);

    public abstract bool WallInFront(AiController p_aiCont, Rigidbody2D p_rb, Vector2 p_castPos, float p_circleCastRad, int p_forwardDir, LayerMask p_hitLayer, bool p_isGrounded);

    public abstract bool IsGrounded(AiController p_aiCont, LayerMask p_wallLayer);

    public abstract void VisualTellMovement(Rigidbody2D p_rb);

    public abstract void StopMoving(Rigidbody2D p_rb);
}
