using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///Movement for grounded enemies
///<Summary>

[CreateAssetMenu(fileName = "AI_MovementType_Grounded", menuName = "AI/Movement/Grounded", order = 0)]
public class AI_MovementType_Grounded : AI_MovementType_Base
{
    [Header("Ground Type Variables")]
    public bool m_canJump;
    public float m_gravityValue, m_jumpHeight;

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



    public override bool WallInFront(Rigidbody2D p_rb, Transform p_enemyObject, float p_raycastLength, int p_forwardDir, LayerMask p_wallLayer)
    {
        if (!m_canJump) return false;

        if (Physics2D.Raycast(p_enemyObject.position, p_enemyObject.right * p_forwardDir, p_raycastLength, p_wallLayer))
        {
            float jumpForce = Mathf.Sqrt(2f * m_gravityValue * m_jumpHeight);
            p_rb.velocity = new Vector3(p_rb.velocity.x, jumpForce, 0);
            return true;
        }
        return false;

    }

    public override bool EnemyInFront(Rigidbody2D p_rb, Transform p_enemyObject, float p_raycastLength, int p_forwardDir, LayerMask p_enemyLayer)
    {
        if (Physics2D.Raycast(p_enemyObject.position, p_enemyObject.right * p_forwardDir, p_raycastLength, p_enemyLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool IsGrounded(Rigidbody2D p_rb, Transform p_enemyObject, float p_raycastLength, LayerMask p_wallLayer)
    {
        if (Physics2D.Raycast(p_enemyObject.position, -p_enemyObject.up, p_raycastLength, p_wallLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
