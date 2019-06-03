using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup_Base : MonoBehaviour
{
	public LayerMask m_playerLayer;

	public abstract void SetPickup(PlayerController p_playerRefrence);

	public int RandomIndex(int p_amountOfTypes)
	{
		 return Random.Range(0, p_amountOfTypes);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CheckCollisionLayer(m_playerLayer, collision))
		{
			SetPickup(collision.gameObject.GetComponent<PlayerController>());

			gameObject.SetActive(false);
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
