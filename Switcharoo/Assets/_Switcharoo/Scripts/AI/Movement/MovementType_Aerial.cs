using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///The aerial movement pattern, used for flying enemeis
///<Summary>

[CreateAssetMenu(fileName = "Aerial", menuName = "Scriptable Objects/Movement/Aerial", order = 0)]
public class MovementType_Aerial : MovementType_Base
{

    ///<Summary>
    ///Returns the target position in a positon that the enemy can reach
    public override Vector3 ConvertRelativePosition(GameObject p_enemyObject, Vector3 p_convertPos)
    {
        return p_convertPos;
    }

    ///<Summary>
    ///Moves the enemy to a given position
    public override void MoveToPosition(Rigidbody2D p_rb, Vector3 p_startPos,Vector3 p_targetPosition)
    {
        ///Removes gravity, to allow for flight
        p_rb.gravityScale = 0;
        Vector3 dir = (p_targetPosition - p_startPos).normalized;
        p_rb.velocity = dir * m_speed;
    }

    ///<Summary>
    ///Determines if the postion has been reached
    public override bool PostionReached(GameObject p_enemyObject, Vector3 p_targetPos, float p_stoppingDistance)
    {
        if(Vector3.Distance(p_targetPos, p_enemyObject.transform.position) <= p_stoppingDistance){
            return true;
        }
        return false;
    }
}
