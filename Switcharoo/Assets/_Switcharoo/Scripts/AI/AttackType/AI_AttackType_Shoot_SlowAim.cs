﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_AttackType_Shoot - Slow Aim", menuName = "AI/AttackType/Shoot/Slow Aim", order = 0)]
public class AI_AttackType_Shoot_SlowAim : AI_AttackType_Shoot
{
    [Header("Slow Aim variables")]
    public float m_aimSpeed;
    public override void AimAtTarget(Transform p_bulletOrigin, Vector3 p_targetPos)
    {
        Vector3 dir = p_targetPos - p_bulletOrigin.transform.position;

        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float slowedAngle = Mathf.Lerp(p_bulletOrigin.eulerAngles.z, lookAngle, Time.deltaTime * m_aimSpeed);

        p_bulletOrigin.rotation = Quaternion.AngleAxis(slowedAngle, Vector3.forward);


    }
}
