using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackType_Collide : AttackType_Base
{
    public float m_tellTime;
    public float m_stopAttackDistance;
    float m_visualTellTimer;
    public override bool AttackFinished(Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject)
    {
        switch (m_currentAttackState)
        {
            case AttackState.Start:
                VisualTell();
                break;

            case AttackState.Perform:

                if (PlayerInRange(p_player, p_enemyObject))
                {
                    m_attackMovement.PerformMovement(p_rb, p_enemyObject.transform, p_player.transform.position);
                }
                else
                {
                    m_currentAttackState = AttackState.Finished;
                }
                break;

            case AttackState.Finished:
                Debug.Log("Attack Finished");
                return true;
        }
        return false;
    }

    public override bool CurrentlyAttacking()
    {
        if (m_currentAttackState != AttackState.Finished)
        {
            return false;
        }
        return true;
    }

    public override void StartAttack(Rigidbody2D p_rb, GameObject p_player, GameObject p_enemyObject)
    {
        m_currentAttackState = AttackState.Start;
        m_visualTellTimer = Time.time;
    }

    bool PlayerInRange(GameObject p_player, GameObject p_enemyObject)
    {
        if (p_player != null)
        {
            if (Vector3.Distance(p_player.transform.position, p_enemyObject.transform.position) < m_stopAttackDistance)
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

    void VisualTell()
    {
        float percent = (Time.time - m_visualTellTimer) / m_tellTime;

        if (percent >= 1)
        {
            m_currentAttackState = AttackState.Perform;
        }
    }
}
