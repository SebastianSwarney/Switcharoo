using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
    public float m_moveSpeed = 10;

    private void Update() 
    {
        transform.Translate(transform.right * m_moveSpeed * Time.deltaTime, Space.World);
    }
}
