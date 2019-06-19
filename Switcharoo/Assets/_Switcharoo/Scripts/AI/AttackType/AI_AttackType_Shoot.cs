using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<Summary>
///The shooting behaviour used for enemies with guns
///<Summary>

[CreateAssetMenu(fileName = "AI_AttackType_Shoot - Instant Aim", menuName = "AI/AttackType/Shoot/InstantAim", order = 0)]
public class AI_AttackType_Shoot : AI_AttackType_Base
{
    [Header("Shoot-Only variables")]
    public float m_distanceFromPlayer;
    public int m_bulletsPerPattern;

    public float m_shootBreakTime, m_shootTriggerTime;

    public AI_MovementType_Base m_shootingMovement;
    public ShootController.WeaponComposition m_weaponComposition;

    ///<Summary>
    ///Where all the attack logic is
    public override bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun)
    {
        Debug.DrawLine(p_aiController.transform.position, p_targetPos, Color.cyan);
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


                    //Different movement for when they are shooting
                    if (!m_shootingMovement.PostionReached(p_aiController.m_agent, p_enemyObject, p_targetPos, m_targetStoppingDistance))
                    {
                        m_shootingMovement.ConvertRelativePosition(p_aiController.m_agent, p_enemyObject, p_aiController.m_aiBounds.PositionInBounds(p_targetPos));
                        m_shootingMovement.MoveToPosition(p_aiController, p_aiController.m_attackSpeed, p_rb, p_aiController.m_agent, p_enemyObject.transform.position, p_aiController.m_aiBounds.PositionInBounds(p_targetPos), p_aiController.m_isGrounded);

                        p_aiController.ChangeAnimation(false);
                        Debug.Log("Moving to position");
                    }
                    else
                    {
                        Debug.Log("Position reached");
                        p_aiController.ChangeAnimation(true);
                        m_shootingMovement.StopMoving(p_rb);
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
                p_aiController.m_target = null;
                return true;
        }
        return false;
    }


    //Generates the target to move to during the attack
    public override Vector3 SetAttackTargetPosition(AiController p_aiCont, GameObject p_enemyObject, GameObject p_player)
    {
        Vector3 dir = (p_player.transform.position - p_enemyObject.transform.position).normalized;

        Vector3 targetPos = p_player.transform.position - dir * m_distanceFromPlayer;

        
        return m_attackMovement.ConvertRelativePosition(p_aiCont.m_agent, p_enemyObject, p_aiCont.m_aiBounds.PositionInBounds(targetPos));
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

    public override bool PlayerInRange(AiController p_aiCont, GameObject p_player, GameObject p_enemyObject, Vector2 p_detectionRange)
    {
        
        //return base.PlayerInRange(p_aiCont, p_player, p_enemyObject, p_detectionRange);
        if (p_player.transform.position.x > p_enemyObject.transform.position.x + p_detectionRange.x / 2 ||
                p_player.transform.position.x < p_enemyObject.transform.position.x - p_detectionRange.x / 2 ||
                p_player.transform.position.y > p_enemyObject.transform.position.y + p_detectionRange.y / 2 ||
                p_player.transform.position.y < p_enemyObject.transform.position.y - p_detectionRange.y / 2)
        {
            p_aiCont.PlayerSpotted(false);
            p_aiCont.ChangeAnimation(false);

            return false;
        }
        else
        {
            p_aiCont.PlayerSpotted(true);
            p_aiCont.ChangeAnimation(true);

            return true;
        }
    }

    public override void CheckForPlayer(AiController p_aiCont)
    {
        if (p_aiCont.m_target == null)
        {
            Collider2D playerCol = Physics2D.OverlapBox(p_aiCont.transform.position, p_aiCont.m_enemyType.m_detectionRadius, 0, p_aiCont.m_playerLayer);
            if (playerCol != null)
            {

                p_aiCont.m_target = playerCol.gameObject;
            }


        }
    }

}
