using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasing : MonoBehaviour
{
    public float m_lifespan, m_longLifespan;
    
    float m_currentLife;

    SpriteRenderer m_spriteRender;
    public Color m_startColor, m_endColor;
    ObjectPooler m_pooler;
    bool m_startFade;

    Coroutine m_despawnCoroutine;
    WaitForSeconds m_delay;
    FMODUnity.StudioEventEmitter m_soundEmitter;
    private void Start()
    {
        m_spriteRender = GetComponent<SpriteRenderer>();
        m_pooler = ObjectPooler.instance;
        m_soundEmitter = GetComponent<FMODUnity.StudioEventEmitter>();

        m_pooler.AddObjectToDespawn(this.gameObject);
    }
    private void OnEnable()
    {
        m_startFade = false;
        m_currentLife = 0;
        if(m_spriteRender == null)
        {
            m_spriteRender = GetComponent<SpriteRenderer>();
        }
        m_spriteRender.color = Color.Lerp(m_startColor, m_endColor, 0);

        if(m_delay == null)
        {
            m_delay = new WaitForSeconds(m_longLifespan);
        }
        m_despawnCoroutine = StartCoroutine(DelayDespawn());
    }
    private void Update()
    {
        if (m_startFade)
        {
            float percent = m_currentLife / (m_lifespan);
            m_spriteRender.color = Color.Lerp(m_startColor, m_endColor, percent);
            m_currentLife += Time.deltaTime;

            if (percent >= 1)
            {
                m_pooler.ReturnToPool(this.gameObject);
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
	{
        m_startFade = true;
        if (m_despawnCoroutine != null)
        {
            StopCoroutine(m_despawnCoroutine);
        }
        m_soundEmitter.Play();
	}

    IEnumerator DelayDespawn()
    {
        yield return m_delay;
        m_startFade = true;
    }
}
