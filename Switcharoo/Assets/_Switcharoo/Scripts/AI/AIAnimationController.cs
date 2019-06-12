using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationController : MonoBehaviour
{
    Animator m_animCont;
    AiController m_aiCont;

    private void Awake()
    {
        m_animCont = GetComponent<Animator>();
        m_aiCont = transform.parent.GetComponent<AiController>();
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
    }
    #endregion

    #region Animation Paused
    public void PauseAnimation(bool p_pause)
    {
        m_animCont.enabled = !p_pause;
    }
    #endregion

}
