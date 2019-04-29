using System.Collections;
using System.Collections.Generic;
using UnityEngine;



///<Summary>
///The behviour that performs the melee enemies
///<Summary>

[CreateAssetMenu(fileName = "Collide", menuName = "Scriptable Objects/AttackType/Collide", order = 0)]
public class AttackType_Collide : AttackType_Base
{
    [Header("Melee-Only Variables")]
    public float m_targetOvershootPlayerDistance;   //Used for if the target overshoots the player, and the player jumps over them


    ///<Summary>
    ///Where all the attack logic is
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun)
    {
        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:

                //Perform the visual tell
                VisualTell(p_aiController);
                break;

            case AttackState.Perform:

                //If the player is in range, set a position that is in their direction
                if (PlayerInRange(p_player, p_enemyObject))
                {
                    m_attackMovement.MoveToPosition(p_rb, p_enemyObject.transform.position, p_targetPos);

                    //If the enemy reaches that position, end the current attack
                    if (m_attackMovement.PostionReached(p_enemyObject, p_targetPos, m_targetStoppingDistance))
                    {
                        p_aiController.m_currentAttackState = AttackState.Finished;
                    }
                }

                //If the player is not in range, end the attack
                else
                {
                    p_aiController.target = null;
                    p_aiController.m_currentAttackState = AttackState.Finished;
                }
                break;

            case AttackState.Finished:
                return true;
        }
        return false;
    }

    ///<Summary>
    ///Generates a target position that is a little behind the player, allowing for that overshooting
    public override Vector3 SetAttackTargetPosition(GameObject p_enemyObject, GameObject p_player)
    {
        Vector3 playerPos = m_attackMovement.ConvertRelativePosition(p_enemyObject, p_player.transform.position);
        Vector3 enemyPos = m_attackMovement.ConvertRelativePosition(p_enemyObject, p_enemyObject.transform.position);
        Vector3 dir = (playerPos - enemyPos).normalized;
        dir = playerPos + dir * m_targetOvershootPlayerDistance;

        return dir;
    }

    ///<Summary>
    //Start the attack
    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun)
    {
        p_aiController.m_currentAttackState = AttackState.Start;
    }





}
