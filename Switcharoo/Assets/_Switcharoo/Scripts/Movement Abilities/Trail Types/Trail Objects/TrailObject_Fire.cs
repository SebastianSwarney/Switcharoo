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
        ObjectPooler.instance.AddObjectToDespawn(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckCollisionLayer(m_damageTargetMask, collision.collider))
		{
			if (collision.gameObject.tag == "Enemy")
			{
				if (collision.gameObject.GetComponent<AiController>().m_entityType == m_type)
				{
					collision.gameObject.GetComponent<Health>().SetFireState();
				}
			}
			else if (collision.gameObject.tag == "EnemySpawner")
			{
				if (collision.gameObject.GetComponent<AI_Spawner>().m_spawnerType == m_type)
				{
					collision.gameObject.GetComponent<Health>().SetFireState();
				}
			}
		}

		ObjectPooler.instance.ReturnToPool(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (CheckCollisionLayer(m_damageTargetMask, collision.GetComponent<Collider2D>()))
            {
                if (collision.gameObject.tag == "Enemy")
                {
                    if (collision.gameObject.GetComponent<AiController>().m_entityType == m_type)
                    {
                        collision.gameObject.GetComponent<Health>().SetFireState();
                    }
                }
                else if (collision.gameObject.tag == "EnemySpawner")
                {
                    if (collision.gameObject.GetComponent<AI_Spawner>().m_spawnerType == m_type)
                    {
                        collision.gameObject.GetComponent<Health>().SetFireState();
                    }
                }
            }

            ObjectPooler.instance.ReturnToPool(gameObject);
        }
    }
}
