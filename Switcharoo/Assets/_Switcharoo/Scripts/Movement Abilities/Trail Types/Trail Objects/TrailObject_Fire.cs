using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Fire : MonoBehaviour
{
	[HideInInspector]
	public LayerMask m_damageTargetMask;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//collision.gameObject.GetComponent<Health>().SetFireState();
		ObjectPooler.instance.ReturnToPool(gameObject);
	}
}
