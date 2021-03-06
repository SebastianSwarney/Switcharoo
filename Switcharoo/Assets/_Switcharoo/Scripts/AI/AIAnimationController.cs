﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class OnEventActive : UnityEvent { }
public class AIAnimationController : MonoBehaviour
{
    Animator m_animCont;
    AiController m_aiCont;

    [Header("Enemy Hurt visuals")]
    public float m_displayHurtTime;
    public Color m_startColor, m_endColor;
    SpriteRenderer m_sRend;
    Coroutine m_displayHurtCoroutine;

    public Color m_frozenColor;
    Color m_enemyInitialColor;

    public OnEventActive m_fireBulletSound, m_heavyAltFireSound;
    private void Awake()
    {
        m_animCont = GetComponent<Animator>();
        m_aiCont = transform.parent.GetComponent<AiController>();
        m_sRend = GetComponent<SpriteRenderer>();
        m_enemyInitialColor = m_sRend.color;
    }

    #region Animation Start Events
    public void JumpAnimation()
    {
        m_animCont.SetTrigger("isJumping");
    }
    public void AttackAnimation(bool p_active)
    {
        m_animCont.SetBool("isAttacking", p_active);
    }
    public void PlayerInRangeAnimation(bool p_active)
    {
        m_animCont.SetBool("playerSpotted", p_active);
    }
    public void ShootAnimation(bool p_active)
    {
        m_animCont.SetBool("isShooting", p_active);
    }

    public void DieAnimation(bool p_active)
    {
        m_animCont.SetBool("die", p_active);
    }

    public void CheckGrounded(bool p_active)
    {
        m_animCont.SetBool("Grounded", p_active);
    }

    public void FireShootAnimation()
    {
        m_animCont.SetTrigger("FireBullet");
    }
    public void FireShootAltAnim()
    {
        m_animCont.SetTrigger("FireAlt");
    }

    public void Idle(bool p_active)
    {
        m_animCont.SetBool("Idle", !p_active);
    }
    #endregion

    #region Animation events
    public void BeginJump()
    {
        m_aiCont.m_beginJump = true;
        m_aiCont.m_isJumping = true;
    }
    public void CompleteJumpAnimation()
    {
        m_aiCont.m_isJumping = false;
    }

    public void FireBullet()
    {
        m_aiCont.ShootGun();
        if (!m_aiCont.m_fireAlt)
        {
            m_fireBulletSound.Invoke();
        }
        else
        {
            m_heavyAltFireSound.Invoke();
        }
    }

    public void SwitchToAltFire()
    {
        
        m_aiCont.m_canSwitchToAlt = true;
    }
    public void StopSwitchToAltFire()
    {
        m_aiCont.m_canSwitchToAlt = false;
    }



    #endregion

    #region Animation Paused
    public void PauseAnimation(bool p_pause)
    {
        if (m_animCont == null)
        {
            m_animCont = GetComponent<Animator>();
        }
        if (!p_pause)
        {
            if(m_aiCont == null)
            {
                m_aiCont = transform.parent.GetComponent<AiController>();

                
            }
            if (m_aiCont.m_isFrozen) return;
        }
        m_animCont.enabled = !p_pause;
    }
    #endregion


    public void RespawnEnemy()
    {
        if (m_sRend == null)
        {
            m_sRend = GetComponent<SpriteRenderer>();
        }
        if (m_animCont == null)
        {
            m_animCont = GetComponent<Animator>();
        }
        m_sRend.color = Color.white;
        m_animCont.enabled = true;
    }

    public void FreezeEnemy()
    {
        m_sRend.color = m_frozenColor;
        if (m_animCont == null)
        {
            m_animCont = GetComponent<Animator>();
        }

        m_animCont.enabled = false;
    }


    public void EnemyHurt()
    {
        m_sRend.color = m_startColor;
        if(m_displayHurtCoroutine != null)
        {
            StopCoroutine(m_displayHurtCoroutine);
        }
        m_displayHurtCoroutine = StartCoroutine(DisplayHurt());
    }

    IEnumerator DisplayHurt()
    {
        float percent = 0, currentTime = 0;
        while (currentTime <= m_displayHurtTime)
        {
            percent = currentTime / m_displayHurtTime;
            m_sRend.color = Color.Lerp(m_startColor, m_endColor, percent);
            currentTime += Time.deltaTime;
            yield return null;
        }
        m_sRend.color = Color.Lerp(m_startColor, m_endColor, 1);

    }
}
