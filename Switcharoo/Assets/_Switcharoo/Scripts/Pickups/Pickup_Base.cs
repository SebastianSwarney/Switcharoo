using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup_Base : MonoBehaviour
{
	public LayerMask m_playerLayer;

	public abstract void SetPickup(PlayerController p_playerRefrence);

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CheckCollisionLayer(m_playerLayer, collision))
		{
			SetPickup(collision.gameObject.GetComponent<PlayerController>());
		}
	}

	public bool CheckCollisionLayer(LayerMask p_layerMask, Collider2D p_collision)
	{
		if (p_layerMask == (p_layerMask | (1 << p_collision.gameObject.layer)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
