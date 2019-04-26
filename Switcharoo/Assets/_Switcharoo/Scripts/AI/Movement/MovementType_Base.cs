using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementType_Base : ScriptableObject {
    public float m_speed;
    public abstract void MoveToPosition(Rigidbody2D p_rb, Vector3 p_startPos,Vector3 p_targetPosition);
}
