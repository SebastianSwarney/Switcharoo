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

    public abstract bool WallInFront(Rigidbody2D p_rb,  Transform p_enemyObject, float p_raycastLength, int p_forwardDir, LayerMask p_wallLayer);

    public abstract bool EnemyInFront(Rigidbody2D p_rb,  Transform p_enemyObject, float p_raycastLength, int p_forwardDir, LayerMask p_enemyLayer);

    public abstract bool IsGrounded(Rigidbody2D p_rb,  Transform p_enemyObject, float p_raycastLength, LayerMask p_wallLayer);
}
