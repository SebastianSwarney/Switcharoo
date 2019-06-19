using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Fall : CollisionHazard_Base
{
	[Header("Fall Hazard Properties")]
	public Bounds m_triggerArea;
	public bool m_drawBoundsInWorldSpace;

	private bool m_isTriggered;

	public override void Start()
	{
		base.Start();

		if (!m_drawBoundsInWorldSpace)
		{
			m_triggerArea.center = m_triggerArea.center + transform.position;
		}
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
			m_rigidbody.isKinematic = false;
			m_isTriggered = true;
		}
	}

	void OnDrawGizmos()
	{
		Bounds drawBounds = new Bounds();

		if (!Application.isPlaying)
		{
			drawBounds.extents = m_triggerArea.extents;

			if (!m_drawBoundsInWorldSpace)
			{
				drawBounds.center = m_triggerArea.center + transform.position;
			}
			else
			{
				drawBounds.center = m_triggerArea.center;
			}
		}
		else
		{
			drawBounds = m_triggerArea;
		}

		DebugExtension.DebugBounds(drawBounds, Color.red);
	}
}
