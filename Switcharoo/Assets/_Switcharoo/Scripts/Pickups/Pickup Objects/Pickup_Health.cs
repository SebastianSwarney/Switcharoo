using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickup_Health : MonoBehaviour, IActivatable
{
    public OnActivationEvent m_playerCollided = new OnActivationEvent();
    [Space (10)]
    public bool m_isPlatform = false;
    public float m_healthIncrease;
    public float m_regenTime = 30f;
    Coroutine m_regenCoroutine;
	public LayerMask m_playerLayer;

    private GameObject m_sprite;
    private Collider2D m_collisionBox;

    [Header("Activate Variables")]
    public bool m_startActive = true;


    #region Drop Variables
    ObjectPooler m_pooler;
    #endregion
    private void Start()
    {
        if (m_isPlatform)
        {
            m_collisionBox = GetComponent<Collider2D>();
            m_sprite = transform.GetChild(0).gameObject;

            ResetMe();
        }
        else
        {
            m_pooler = ObjectPooler.instance;
            m_pooler.AddObjectToDespawn(this.gameObject);
            m_pooler.AddObjectToPauser(this.gameObject);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckCollisionLayer(m_playerLayer, collision.collider))
		{
            m_playerCollided.Invoke();
			collision.gameObject.GetComponent<Health>().HealDamage(m_healthIncrease);
            if (m_isPlatform)
            {
                m_sprite.SetActive(false);
                m_collisionBox.enabled = false;
                print("Collide");
                m_regenCoroutine = StartCoroutine(RegenerateObject());
            }
            else
            {
                m_pooler.ReturnToPool(this.gameObject);
            }
            
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

    public void ActiveState(bool p_active)
    {
        m_sprite.SetActive(p_active);
        m_collisionBox.enabled = p_active;
    }

    public void ResetMe()
    {
        if(m_regenCoroutine != null)
        {
            StopCoroutine(m_regenCoroutine);
        }
        m_sprite.SetActive(m_startActive);
        m_collisionBox.enabled = m_startActive;
    }

    IEnumerator RegenerateObject()
    {
        yield return new WaitForSeconds(m_regenTime);
        ActiveState(true);
    }
}
