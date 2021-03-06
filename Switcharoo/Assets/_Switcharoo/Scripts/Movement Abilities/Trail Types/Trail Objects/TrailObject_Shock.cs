﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Shock : TrailObject_Base
{
	[HideInInspector]
	public TrailType_Shock m_trailType;
	[HideInInspector]
	public LayerMask m_damageTargetMask;
    private void OnEnable()
    {
        ObjectPooler.instance.AddObjectToDespawn(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
	{
		m_trailType.ShockBlast(transform.position, m_damageTargetMask, m_type);
		ObjectPooler.instance.ReturnToPool(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {


            m_trailType.ShockBlast(transform.position, m_damageTargetMask, m_type);
            ObjectPooler.instance.ReturnToPool(gameObject);
        }
    }
}
