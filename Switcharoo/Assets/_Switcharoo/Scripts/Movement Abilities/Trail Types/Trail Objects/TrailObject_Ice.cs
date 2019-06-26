using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Ice : TrailObject_Base
{
	[HideInInspector]
	public TrailType_Ice m_trailType;
	[HideInInspector]
	public LayerMask m_damageTargetMask;
    private void OnEnable()
    {
        ObjectPooler.instance.AddObjectToDespawn(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
	{
		m_trailType.IceBlast(transform.position, m_damageTargetMask);
		ObjectPooler.instance.ReturnToPool(gameObject);
	}
}
