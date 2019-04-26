using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Aerial", menuName = "Scriptable Objects/Movement/Aerial", order = 0)]
public class MovementType_Aerial : MovementType_Base
{
    public override Vector3 ConvertRelativePosition(GameObject p_enemyObject, Vector3 p_convertPos)
    {
        return p_convertPos;
    }

    public override void MoveToPosition(Rigidbody2D p_rb, Vector3 p_startPos,Vector3 p_targetPosition)
    {
        p_rb.gravityScale = 0;
        Vector3 dir = (p_targetPosition - p_startPos).normalized;
        p_rb.velocity = dir * m_speed;
    }

    public override bool PostionReached(GameObject p_enemyObject, Vector3 p_targetPos, float p_stoppingDistance)
    {
        if(Vector3.Distance(p_targetPos, p_enemyObject.transform.position) <= p_stoppingDistance){
            return true;
        }
        return false;
    }
}
