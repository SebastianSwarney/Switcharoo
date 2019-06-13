using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Trigger : CollisionHazard_Base, IActivatable
{
	[Header("Trigger Hazard Properties")]
	public Bounds m_triggerArea;
	public float m_triggerDelay;
	public bool m_drawBoundsInWorldSpace;

	private bool m_isTriggered;

    Coroutine m_hazardStartCoroutine;
	public override void Start()
	{
		base.Start();
		m_renderer.color = Color.clear;

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
			m_isTriggered = true;
			m_hazardStartCoroutine = StartCoroutine(PopOut());
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


    #region IActivatable Methods
    public void ActiveState(bool p_active)
    {
        if (p_active)
        {
            m_isTriggered = true;
            StartCoroutine(PopOut());
        }
    }

    public void ResetMe()
    {
        m_isTriggered = false;
        if(m_hazardStartCoroutine != null)
        {
            StopCoroutine(m_hazardStartCoroutine);
        }
        m_canDamage = false;
        m_renderer.color = Color.clear;
        
        
    }

    #endregion
}
