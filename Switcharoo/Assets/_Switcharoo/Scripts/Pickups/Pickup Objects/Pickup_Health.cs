using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Health : MonoBehaviour
{
	public float m_healthIncrease;

	public LayerMask m_playerLayer;

    private void Start()
    {
        ObjectPooler pooler = ObjectPooler.instance;
        pooler.AddObjectToDespawn(this.gameObject);
        pooler.AddObjectToPauser(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckCollisionLayer(m_playerLayer, collision.collider))
		{
			collision.gameObject.GetComponent<Health>().HealDamage(m_healthIncrease);
			ObjectPooler.instance.ReturnToPool(gameObject);
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
