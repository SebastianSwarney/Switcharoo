using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Base : MonoBehaviour
{
	public PlayerController.PlayerType m_type;

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
