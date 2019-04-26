using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotType_Base : ScriptableObject
{
    public float m_fireRate;

    public abstract void Shoot(Transform p_bulletOrgin);
}
