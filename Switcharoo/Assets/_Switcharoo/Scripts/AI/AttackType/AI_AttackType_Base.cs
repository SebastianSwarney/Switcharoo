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
    public float m_attackRadius;
    public float m_tellTime;
    public float m_targetStoppingDistance;
    public float m_playerMoveDistanceReaction;

    #region Abstract Methods
    
    ///Initiates the attacks, setting variables
    public abstract void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject, ShootController p_gun);

    
    ///Where the attack state machine exists. All attack logic is done here
    public abstract bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos, GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin, ShootController p_gun);
    
    
    ///Used to get a postion for the movement that occurs during an attack
    public abstract Vector3 SetAttackTargetPosition(GameObject p_enemyObject, GameObject p_player);

    #endregion
    
    #region Methods
    
    ///The moment before an attack, the visual tell
    ///After this runs, the attack will commence
    ///Visual Tell animations may be done here
    public void VisualTell(AiController p_aiController)
    {
        float percent = p_aiController.m_visualTellTimer / m_tellTime;

        if (percent >= 1)
        {
            p_aiController.m_visualTellTimer = 0;
            p_aiController.m_currentAttackState = AttackState.Perform;
            Debug.Log("Visual tell done");
        }else{
            p_aiController.m_visualTellTimer += Time.deltaTime;
        }
    }

    ///Determines whether the player is in range
    public bool PlayerInRange(GameObject p_player, GameObject p_enemyObject)
    {
        if (p_player != null)
        {
            if (Vector3.Distance(p_player.transform.position, p_enemyObject.transform.position) < m_attackRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }


    #endregion
}

