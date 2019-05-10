using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Ice : MonoBehaviour
{
	[HideInInspector]
	public TrailType_Ice m_trailType;
	[HideInInspector]
	public LayerMask m_damageTargetMask;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		m_trailType.IceBlast(transform.position, m_damageTargetMask);
		ObjectPooler.instance.ReturnToPool(gameObject);
	}
}
