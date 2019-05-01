using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_IdleMovementType_Base : ScriptableObject
{
    public AI_MovementType_Base m_movementType;

    public abstract void PerformIdleMovement(Rigidbody2D p_rb, Transform p_enemyObject, int p_forwardDir);
}
