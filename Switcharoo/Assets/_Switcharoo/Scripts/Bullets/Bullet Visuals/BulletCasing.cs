using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasing : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		ObjectPooler.instance.ReturnToPool(gameObject);
	}
}
