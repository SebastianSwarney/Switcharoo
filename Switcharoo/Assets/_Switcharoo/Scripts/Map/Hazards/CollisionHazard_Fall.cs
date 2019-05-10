using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Fall : CollisionHazard_Base
{
	[Header("Fall Hazard Properties")]
	public Bounds m_triggerArea;

	private bool m_isTriggered;

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
			m_rigidbody.isKinematic = false;
			m_isTriggered = true;
		}
	}

	void OnDrawGizmos()
	{
		DebugExtension.DebugBounds(m_triggerArea, Color.red);
	}
}
