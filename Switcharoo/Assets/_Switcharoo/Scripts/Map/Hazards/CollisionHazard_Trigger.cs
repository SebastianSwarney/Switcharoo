using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Trigger : CollisionHazard_Base
{
	[Header("Trigger Hazard Properties")]
	public Bounds m_triggerArea;
	public float m_triggerDelay;

	private bool m_isTriggered;

	public override void Start()
	{
		base.Start();
		m_renderer.color = Color.clear;
	}

	private void Update()
	{
		if (!m_isTriggered)
		{
			CheckForTarget();
		}
	}

	void CheckForTarget()
	{
		Collider2D collider = Physics2D.OverlapBox(m_triggerArea.center, m_triggerArea.size, 0f, m_targetMask);

		if (collider)
		{
			m_isTriggered = true;
			StartCoroutine(PopOut());
		}
	}

	IEnumerator PopOut()
	{
		float t = 0;

		while (t < m_triggerDelay)
		{
			t += Time.deltaTime;
			yield return null;
		}

		m_canDamage = true;
		m_renderer.color = Color.white;
	}

	void OnDrawGizmos()
	{
		DebugExtension.DebugBounds(m_triggerArea, Color.red);
	}
}
