using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_DeathParticle : MonoBehaviour
{
    ObjectPooler m_pooler;
    public float m_lifespan;
    WaitForSeconds m_delay;
    ParticleSystem m_particles;
    Coroutine m_lifeCor;
    private void Start()
    {
        ObjectPooler.instance.AddObjectToDespawn(this.gameObject);
    }

    private void OnEnable()
    {
        
        if (m_particles == null)
        {
            m_particles = GetComponent<ParticleSystem>();
            m_delay = new WaitForSeconds(m_lifespan);
        }

        if (m_lifeCor != null)
        {
            StopCoroutine(m_lifeCor);
        }
        m_particles.Play();
        m_lifeCor = StartCoroutine(LifeSpan());


    }
    

    IEnumerator LifeSpan()
    {
        yield return m_delay;
        if(m_pooler == null)
        {
            m_pooler = ObjectPooler.instance;
        }
        m_pooler.ReturnToPool(gameObject);

    }

    private void OnDisable()
    {
        if (m_lifeCor != null)
        {
            StopCoroutine(m_lifeCor);
        }
    }

}
