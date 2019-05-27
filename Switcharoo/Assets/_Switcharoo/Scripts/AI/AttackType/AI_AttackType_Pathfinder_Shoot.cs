using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AI_AttackType_Shoot_Pathfinder - Instant Aim", menuName = "AI/AttackType/Shoot_Pathfinder/InstantAim", order = 0)]
public class AI_AttackType_Pathfinder_Shoot : AI_AttackType_Base
{
    [Header("Pathfinder Shoot-Only variables")]
    public float m_distanceFromPlayer;
    public float m_shootIntervalTime, m_shootBreakTime;
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
                VisualTell(p_aiController, p_rb);
                break;

            case AttackState.Perform:
                //If the player is in range, perform the attack
                if (PlayerInRange(p_player, p_enemyObject))
                {

                    AimAtTarget(p_bulletOrigin, p_player.transform.position);

                    ///Creates bursts between the bullets
                    if (CanFireWeapon(p_aiController))
                    {
                        p_gun.Shoot(p_bulletOrigin);
                        p_aiController.SwapShooting(true, m_shootIntervalTime);


                        //If the enemy gets close enough to the player, they will stop advancing
                        if (Mathf.Abs(p_player.transform.position.x - p_enemyObject.transform.position.x) > m_distanceFromPlayer)
                        {
                            m_shootingMovement.MoveToPosition(p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos, p_aiController.m_isGrounded);
                        }
                        else
                        {
                            p_rb.velocity = new Vector3(0f, p_rb.velocity.y, 0f);
                        }

                    }
                    else
                    {
                        p_aiController.SwapShooting(false, m_shootBreakTime);

                        if (Mathf.Abs(p_player.transform.position.x - p_enemyObject.transform.position.x) > m_distanceFromPlayer)
                        {
                            m_shootingMovement.MoveToPosition(p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_targetPos, p_aiController.m_isGrounded);
                        }
                        else
                        {
                            p_rb.velocity = new Vector3(0f, p_rb.velocity.y, 0f);
                        }

                    }


                }

                ///If the player gets out of range, end the attack
                else
                {
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

    //Determines if the weapon can be fired
    bool CanFireWeapon(AiController p_aiCont)
    {
        return p_aiCont.CanFireGun();
    }

}
