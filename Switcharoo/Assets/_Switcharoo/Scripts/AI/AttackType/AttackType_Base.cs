using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackType_Base: ScriptableObject
{
    public enum AttackState{Start, Perform, Finished}
    public AttackState m_currentAttackState = AttackState.Finished;
    public AttackMovement_Base m_attackMovement;
    
    public abstract void StartAttack(Rigidbody2D p_rb,GameObject p_player, GameObject p_enemyObject);

    public abstract bool AttackFinished(Rigidbody2D p_rb,GameObject p_player, GameObject p_enemyObject);

    public abstract bool CurrentlyAttacking();
}
