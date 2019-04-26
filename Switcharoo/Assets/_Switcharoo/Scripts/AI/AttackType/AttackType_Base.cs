using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackType_Base : ScriptableObject
{
    public enum AttackState { Start, Perform, Finished }
    public MovementType_Base m_attackMovement;
    public float m_attackRadius;
    public float m_tellTime;

    public abstract void StartAttack(AiController p_aiController, Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject);

    public abstract bool AttackFinished(AiController p_aiController, Rigidbody2D p_rb, Vector3 p_targetPos,GameObject p_player, GameObject p_enemyObject, Transform p_bulletOrigin);

    public void VisualTell(AiController p_aiController, float p_visualTellTimer)
    {
        float percent = (Time.time - p_visualTellTimer) / m_tellTime;

        if (percent >= 1)
        {
            p_aiController.m_currentAttackState = AttackState.Perform;
            Debug.Log("Visual tell done");
        }
    }

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

    public abstract Vector3 SetAttackTargetPosition(GameObject p_enemyObject, GameObject p_player);
}

