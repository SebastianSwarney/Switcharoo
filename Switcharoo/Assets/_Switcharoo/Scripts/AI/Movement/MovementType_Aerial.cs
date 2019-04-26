using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Aerial", menuName = "Scriptable Objects/Movement/Aerial", order = 0)]
public class MovementType_Aerial : MovementType_Base
{
    public override void MoveToPosition(Rigidbody2D p_rb, Vector3 p_startPos,Vector3 p_targetPosition)
    {
        p_rb.gravityScale = 0;
        Vector3 dir = (p_targetPosition - p_startPos).normalized;
        p_rb.velocity = dir * m_speed;
    }
}
