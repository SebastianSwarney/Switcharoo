using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotType_Base : ScriptableObject
{
    public float m_fireRate;

    public GameObject m_bulletPrefab;

    public abstract void Shoot(Transform p_bulletOrigin);
}
