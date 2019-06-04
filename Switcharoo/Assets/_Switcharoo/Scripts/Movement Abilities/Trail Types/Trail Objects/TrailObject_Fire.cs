using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Fire : TrailObject_Base
{
	[HideInInspector]
	public LayerMask m_damageTargetMask;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckCollisionLayer(m_damageTargetMask, collision.collider))
		{
			collision.gameObject.GetComponent<Health>().SetFireState();
		}

		ObjectPooler.instance.ReturnToPool(gameObject);
	}
}
