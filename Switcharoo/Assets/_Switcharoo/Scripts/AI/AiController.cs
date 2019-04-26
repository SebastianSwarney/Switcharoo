using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public AttackType_Base m_attackType;
    public MovementType_Base m_movementType;
    public GameObject target;

    Rigidbody2D m_rb;

    [Header ("Attack State")]
    public AttackType_Base.AttackState m_currentAttackState = AttackType_Base.AttackState.Finished;

    Vector3 m_attackTargetPos;
    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Debug.DrawLine(transform.position, m_attackTargetPos);
        if(target != null){
            //m_movementType.MoveToPosition(m_rb,transform.position,target.transform.position);
            if(m_currentAttackState == AttackType_Base.AttackState.Finished){
                print ("Enter Attack state");
                m_attackType.StartAttack(this,m_rb,target,gameObject);
                m_attackTargetPos = m_attackType.SetAttackTargetPosition(gameObject, target);
            }else{
                if(m_attackType.AttackFinished(this,m_rb,m_attackTargetPos,target,gameObject,gameObject.transform)){
                    print ("Currently Attacking");
                    target = null;
                }
            }
        }
    }

    /* private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackType.m_stopAttackDistance);
    }*/
}
