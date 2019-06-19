using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff Types/Slow Motion")]
public class PlayerBuff_SlowMotion : PlayerBuff_Base
{
	public float m_slowAmount;
	public float m_slowLength;

	public override void UseBuff(PlayerController p_playerRefrence, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(SlowMotion(p_playerRefrence));
	}

	private IEnumerator SlowMotion(PlayerController p_playerRefrence)
	{
		Time.timeScale = m_slowAmount;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		yield return new WaitForSecondsRealtime(m_slowLength);
		Time.fixedDeltaTime = 0.02f;
		Time.timeScale = 1f;
	}
}
