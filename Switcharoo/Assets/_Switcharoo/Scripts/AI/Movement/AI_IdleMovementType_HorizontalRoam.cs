using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_IdleMovement_HorizontalRoam", menuName = "AI/IdleMovement/HorizontalRoam", order = 0)]
public class AI_IdleMovementType_HorizontalRoam : AI_IdleMovementType_Base
{
    public override void PerformIdleMovement(Rigidbody2D p_rb, Transform p_enemyObject, int p_forwardDir)
    {
        p_rb.velocity = p_enemyObject.right * p_forwardDir * m_movementType.m_speed;
    }
}
