using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public MovementType_Base m_movementType;
    public Transform target;

    Rigidbody2D m_rb;
    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if(target != null){
            m_movementType.MoveToPosition(m_rb,transform.position,target.transform.position);
        }
    }
}
