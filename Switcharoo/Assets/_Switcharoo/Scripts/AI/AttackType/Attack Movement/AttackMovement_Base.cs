using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackMovement_Base : ScriptableObject
{
    public abstract void PerformMovement(Rigidbody2D p_rb, Transform p_enemyObject, Vector3 p_targetPos);
}
