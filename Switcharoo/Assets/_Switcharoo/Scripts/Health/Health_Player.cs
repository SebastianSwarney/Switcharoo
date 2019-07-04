using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Player : Health
{
    private PlayerController m_player;

    [SerializeField]
    private GameObject deathparticle;

    public override void Start()
    {
        base.Start();
        m_player = GetComponent<PlayerController>();
    }

    public override void TakeDamage(float p_damage)
    {
        base.TakeDamage(p_damage);

		if (m_canTakeDamage)
        {
            m_player.m_playerHurt.Invoke();
        }
    }

    public override void Die()
    {
		//Add delay here and particle effect
		if (!m_isDead)
		{
			m_player.m_playerDeath.Invoke();
		}

		m_isDead = true;


    }



    

    
}
