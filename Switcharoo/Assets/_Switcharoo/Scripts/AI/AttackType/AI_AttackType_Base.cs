using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///The scriptable object that is used for the different attack types
///New attack types inherit from this script, allowing for new different attack types
public abstract class AI_AttackType_Base : ScriptableObject
{
    public enum AttackState { Start, Perform, Finished }
    public AI_MovementType_Base m_attackMovement;
    public float m_tellTime;
    public float m_targetStoppingDistance;
    public float m_playerMoveDistanceReaction;

    #region Abstract Methods

    ///Initiates the attacks, setting variables
    public abstract void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun);


    ///Where the attack state machine exists. All attack logic is done here
    public abstract bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun);


    ///Used to get a postion for the movement that occurs during an attack
    public abstract Vector3 SetAttackTargetPosition(AiController p_aiCont, GameObject p_enemyObject, GameObject p_player);

    #endregion

    #region Methods

    ///The moment before an attack, the visual tell
    ///After this runs, the attack will commence
    ///Visual Tell animations may be done here
    public virtual void VisualTell(AiController p_aiController, Rigidbody2D p_rb)
    {
        float percent = p_aiController.m_visualTellTimer / m_tellTime;

        if (percent >= 1)
        {
            p_aiController.m_visualTellTimer = 0;
            p_aiController.m_currentAttackState = AttackState.Perform;
        }
        else
        {
            p_aiController.m_visualTellTimer += Time.deltaTime;
            m_attackMovement.VisualTellMovement(p_rb);
        }
    }

    ///Determines whether the player is in range
    public virtual bool PlayerInRange(AiController p_aiCont, GameObject p_player, GameObject p_enemyObject, Vector2 p_detectionRange)
    {
        
        if (p_player != null)
        {

            
            bool checkPlayer = true;
            if (p_aiCont.m_aiBounds != null)
            {
                if (p_aiCont.m_aiBounds.TargetInBounds(p_player.transform.position))
                {

                    checkPlayer = true;
                }
                else
                {
                    Debug.Log("Player Not Spotted");
                    checkPlayer = false;
                    p_aiCont.PlayerSpotted(false);
                    p_aiCont.ChangeAnimation(false);
                }
            }

            if (!checkPlayer) return false;
            if (p_player.transform.position.x > p_enemyObject.transform.position.x + p_aiCont.m_enemyType.m_detectionOffset.x + p_detectionRange.x / 2 ||
                p_player.transform.position.x < p_enemyObject.transform.position.x + p_aiCont.m_enemyType.m_detectionOffset.x - p_detectionRange.x / 2 ||
                p_player.transform.position.y > p_enemyObject.transform.position.y + p_aiCont.m_enemyType.m_detectionOffset.y + p_detectionRange.y / 2 ||
                p_player.transform.position.y < p_enemyObject.transform.position.y + p_aiCont.m_enemyType.m_detectionOffset.y - p_detectionRange.y / 2)
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
        return false;
    }


    public virtual void CheckForPlayer(AiController p_aiCont)
    {
        if (p_aiCont.m_target != null)
        {

            if (!PlayerInRange(p_aiCont, p_aiCont.m_target, p_aiCont.gameObject, p_aiCont.m_enemyType.m_detectionRadius))
            {
                Debug.Log("Target : null : Base");
                p_aiCont.m_target = null;

            }
        }
        else
        {
            Collider2D playerCol = Physics2D.OverlapBox((Vector2)p_aiCont.transform.position + p_aiCont.m_enemyType.m_detectionOffset, p_aiCont.m_enemyType.m_detectionRadius, 0, p_aiCont.m_playerLayer);
            if (playerCol != null)
            {
                if (p_aiCont.m_aiBounds != null)
                {
                    if (p_aiCont.m_aiBounds.TargetInBounds(playerCol.transform.position))
                    {
                        p_aiCont.m_target = playerCol.gameObject;
                    }
                }
                else
                {
                    p_aiCont.m_target = playerCol.gameObject;
                }

            }
        }
    }
    #endregion
}

