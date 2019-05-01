using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///The scriptable object class used to createh the different movement behaviours
///<Summary>
public abstract class AI_MovementType_Base : ScriptableObject {
    public float m_speed;
    public abstract void MoveToPosition(Rigidbody2D p_rb, Vector3 p_startPos,Vector3 p_targetPosition);

    public abstract bool PostionReached(GameObject p_enemyObject,Vector3 p_targetPos, float p_stoppingDistance);

    public abstract Vector3 ConvertRelativePosition(GameObject p_enemyObject,Vector3 p_convertPos);

    public abstract bool WallInFront(AiController p_aiCont,Rigidbody2D p_rb,  Vector2 p_boxcastPos,  Vector2 p_raycastDimensions, int p_forwardDir, LayerMask p_wallLayer, bool p_isGrounded);

    public abstract bool IsGrounded(Rigidbody2D p_rb,  Vector2 p_boxcastPos,  Vector2 p_raycastDimensions, LayerMask p_wallLayer);
}
