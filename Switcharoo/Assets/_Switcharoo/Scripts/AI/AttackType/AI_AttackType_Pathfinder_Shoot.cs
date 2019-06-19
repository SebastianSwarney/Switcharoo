using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AI_AttackType_Shoot_Pathfinder - Instant Aim", menuName = "AI/AttackType/Shoot_Pathfinder/InstantAim", order = 0)]
public class AI_AttackType_Pathfinder_Shoot : AI_AttackType_Base
{
    [Header("Pathfinder Shoot-Only variables")]
    public float m_distanceFromPlayer;
    public int m_bulletsPerPattern;
    public float m_shootBreakTime, m_shootTriggerTime;

    public AI_MovementType_Base m_shootingMovement;
    public ShootController.WeaponComposition m_weaponComposition;

    ///<Summary>
    ///Where all the attack logic is
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun)
    {

        switch (p_aiController.m_currentAttackState)
        {
            case AttackState.Start:
                //Starts the visual tell
                p_aiController.m_bulletsPerPattern = m_bulletsPerPattern;
                p_aiController.m_shootBreakTime = m_shootBreakTime;
                p_aiController.m_shootTriggerTime = m_shootTriggerTime;
                
                p_aiController.m_currentAttackState = AttackState.Perform;
                break;

            case AttackState.Perform:
                //If the player is in range, perform the attack
                if (PlayerInRange(p_aiController, p_player, p_enemyObject, p_aiController.m_enemyType.m_detectionRadius))
                {

                    AimAtTarget(p_bulletOrigin, p_player.transform.position);

                    if ((int)Mathf.Sign(p_player.transform.position.x - p_enemyObject.transform.position.x) != (int)Mathf.Sign(p_aiController.m_currentForward))
                    {
                        p_aiController.FlipEnemy((int)Mathf.Sign(p_player.transform.position.x - p_enemyObject.transform.position.x));
                    }

                    //If the enemy gets close enough to the player, they will stop advancing
                    if (Mathf.Abs(p_player.transform.position.x - p_enemyObject.transform.position.x) > m_distanceFromPlayer)
                    {
                        p_aiController.ChangeAnimation(false);
                        m_shootingMovement.MoveToPosition(p_aiController, p_aiController.m_attackSpeed, p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_aiController.m_aiBounds.PositionInBounds(p_targetPos), p_aiController.m_isGrounded);

                        

                    }
                    else
                    {
                        p_aiController.ChangeAnimation(true);
                        p_rb.velocity = new Vector3(0f, p_rb.velocity.y, 0f);
                    }




                }

                ///If the player gets out of range, end the attack
                else
                {
                    p_aiController.ChangeAnimation(false);
                    p_aiController.m_currentAttackState = AttackState.Finished;
                    
                }

                break;

            case AttackState.Finished:
                return true;
        }
        return false;
    }


    //Generates the target to move to during the attack
    public override Vector3 SetAttackTargetPosition(AiController p_aiCont, GameObject p_enemyObject, GameObject p_player)
    {

        return p_player.transform.position;
    }


    //Initialize the attack
    public override void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun)
    {
        p_gun.m_currentWeaponComposition = m_weaponComposition;
        p_aiController.m_currentAttackState = AttackState.Start;
    }

    //Aim the gun
    public virtual void AimAtTarget(Transform p_bulletOrigin, Vector3 p_targetPos)
    {
        Vector3 dir = p_targetPos - p_bulletOrigin.transform.position;

        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        p_bulletOrigin.rotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);




    }



}
