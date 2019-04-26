using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///Movement for grounded enemies
///<Summary>

[CreateAssetMenu(fileName = "Grounded", menuName = "Scriptable Objects/Movement/Grounded", order = 0)]
public class MovementType_Grounded : MovementType_Base
{

    ///<Summary>
    ///Converts the target position so that the enemy can reach it
    public override Vector3 ConvertRelativePosition(GameObject p_enemyObject, Vector3 p_convertPos)
    {
        ///Flattens the position, so that its on the same y level as the enemy
        return new Vector3(p_convertPos.x, p_enemyObject.transform.position.y, 0f);
    }


    ///<Summary>
    ///The movement of the enemy
    public override void MoveToPosition(Rigidbody2D p_rb, Vector3 p_startPos, Vector3 p_targetPos)
    {
        ///Sets gravit to be active
        p_rb.gravityScale = 1;

        ///Only travels on the x axis
        Vector3 dir = p_targetPos - p_startPos;
        dir = new Vector3(Mathf.Sign(dir.x) * m_speed, p_rb.velocity.y, 0f);
        p_rb.velocity = dir;
    }

    ///<Summary>
    ///Determines if the position has been reached
    public override bool PostionReached(GameObject p_enemyObject, Vector3 p_targetPos, float p_stoppingDistance)
    {
        if (Mathf.Abs(p_targetPos.x - p_enemyObject.transform.position.x) <= p_stoppingDistance)
        {
            return true;
        }
        return false;
    }


}
