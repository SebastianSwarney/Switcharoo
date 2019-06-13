using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_DeathParticle : MonoBehaviour
{
    ObjectPooler m_pooler;
    public float m_lifespan;
    WaitForSeconds m_delay;
    ParticleSystem m_particles;
    private void OnEnable()
    {
        if (m_particles == null)
        {
            m_particles = GetComponent<ParticleSystem>();
            m_pooler = ObjectPooler.instance;
            m_delay = new WaitForSeconds(m_lifespan);
        }
        else
        {
            m_particles.Play();
            StartCoroutine(LifeSpan());
        }

    }

    IEnumerator LifeSpan()
    {
        yield return m_delay;
        gameObject.SetActive(false);
        m_pooler.ReturnToPool(this.gameObject);
    }


}
