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
    public float m_jumpTurnDistance;
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



    public override bool WallInFront(AiController p_aiCont,Rigidbody2D p_rb, Vector2 p_boxcastPos, Vector2 p_raycastDimensions, int p_forwardDir, LayerMask p_hitLayer, bool p_isGrounded)
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(p_boxcastPos, p_raycastDimensions, 0, Vector2.right * p_forwardDir, p_raycastDimensions.x / 2, p_hitLayer);
        Debug.DrawLine(p_rb.gameObject.transform.position, p_boxcastPos);
        foreach (RaycastHit2D rayHit in hit)
        {
            if (rayHit.transform.gameObject == p_rb.gameObject) continue;
            if (rayHit)
            {
                if (m_canJump)
                {
                    if (p_isGrounded)
                    {
                        float jumpForce = Mathf.Sqrt(2f * m_gravityValue * m_jumpHeight);
                        p_rb.velocity = new Vector3(p_rb.velocity.x, jumpForce, 0);
                        return false;
                    }else{
                        float distToObj = Vector3.Distance(p_rb.transform.position, rayHit.point);
                        
                        if(distToObj < m_jumpTurnDistance){
                            return true;
                        }
                        return false;
                    }
                }

                return true;
            }
        }
        return false;

    }

    public override bool IsGrounded(Rigidbody2D p_rb, Vector2 p_boxcastPos, Vector2 p_raycastDimensions, LayerMask p_hitLayer)
    {
        RaycastHit2D hit = Physics2D.BoxCast(p_boxcastPos, p_raycastDimensions, 0, -Vector3.up, p_raycastDimensions.y/2, p_hitLayer);

        return hit;
    }
}
