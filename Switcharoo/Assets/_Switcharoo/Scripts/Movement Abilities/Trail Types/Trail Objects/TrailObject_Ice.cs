using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject_Ice : TrailObject_Base
{
    public OnActivationEvent m_iceLandSound;
    public GameObject m_iceParticleBurst;
	[HideInInspector]
	public TrailType_Ice m_trailType;
	[HideInInspector]
	public LayerMask m_damageTargetMask;
    ObjectPooler m_pooler;
    
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
        m_pooler.AddObjectToDespawn(this.gameObject);

    }
    private void OnCollisionEnter2D(Collision2D collision)
	{
        m_iceLandSound.Invoke();
        m_pooler.NewObject(m_iceParticleBurst, transform.position, Quaternion.identity);
		m_trailType.IceBlast(transform.position, m_damageTargetMask, m_type);
		ObjectPooler.instance.ReturnToPool(gameObject);
	}
}
