using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Fire : TrailObject_Base
{
	[HideInInspector]
	public LayerMask m_damageTargetMask;
    public float m_lifespan = 3f;
    private WaitForSeconds m_delay;

    private void Start()
    {
        m_delay = new WaitForSeconds(m_lifespan);
    }
    private IEnumerator SpawnDelete()
    {
        yield return m_delay;
        ObjectPooler.instance.ReturnToPool(gameObject);
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnDelete());
    }
    private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckCollisionLayer(m_damageTargetMask, collision.collider))
		{
			collision.gameObject.GetComponent<Health>().SetFireState();
		}

		ObjectPooler.instance.ReturnToPool(gameObject);
	}
}
