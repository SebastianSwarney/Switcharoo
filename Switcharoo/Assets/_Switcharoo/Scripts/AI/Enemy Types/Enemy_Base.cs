using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AI_Enemy", menuName = "AI/Enemy Types", order = 0)]
public class Enemy_Base : ScriptableObject
{

    public Vector3 m_enemyDimensions;
    public Vector3 m_groundCheckDimensions;
    public float m_collisionDamage;
    public AI_AttackType_Base m_attackType;
    public AI_IdleMovementType_Base m_idleMovementType;
}
