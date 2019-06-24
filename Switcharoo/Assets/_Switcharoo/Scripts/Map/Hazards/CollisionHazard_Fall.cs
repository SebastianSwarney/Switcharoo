using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Fall : CollisionHazard_Base, IActivatable
{
	[Header("Fall Hazard Properties")]
	public Bounds m_triggerArea;
	public bool m_drawBoundsInWorldSpace;

	private bool m_isTriggered;

    public float m_lifespan;
    public float m_fadeTimeStart;
    float m_currentTimer;
    SpriteRenderer m_sRend;
    public Color m_endColor;
    Color m_startColor;


    [Header("Visuals")]
    public AI_Spawner.RaceType m_hazardType;
    public Sprite m_alienSprite;
    public Sprite m_robotSprite;
    //IActivatable value
    Vector3 m_StartPos;

	public override void Start()
	{
		base.Start();
        
		if (!m_drawBoundsInWorldSpace)
		{
			m_triggerArea.center = m_triggerArea.center + transform.position;
		}

        m_StartPos = transform.position;
        m_sRend = GetComponent<SpriteRenderer>();
        m_startColor = m_sRend.color;

        m_sRend.sprite = (m_hazardType == AI_Spawner.RaceType.Alien) ? m_alienSprite : m_robotSprite;
    }

	private void Update()
	{
		if (!m_isTriggered)
		{
			CheckForTarget();
        }
        else
        {
            FadeAway();
        }
	}

	void CheckForTarget()
	{
		Collider2D collider = Physics2D.OverlapBox(m_triggerArea.center, m_triggerArea.size, 0f, m_targetMask);

		if (collider)
		{
			m_rigidbody.isKinematic = false;
			m_isTriggered = true;
            m_currentTimer = 0;
		}
	}

    void FadeAway()
    {
        float percent = m_currentTimer / m_fadeTimeStart;
        if (percent >= 1)
        {
            percent = (m_currentTimer - m_fadeTimeStart) / (m_lifespan - m_fadeTimeStart);
            if (percent <= 1)
            {
                m_sRend.color = Color.Lerp(m_startColor, m_endColor, percent);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
            
            
        }
        m_currentTimer += Time.deltaTime;

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

    public override void PauseMe(bool p_paused)
    {
        m_rigidbody.simulated = !p_paused;
    }

    #region IActivatable Methods

    public void ActiveState(bool p_active)
    {
        if (p_active)
        {
            
            m_rigidbody.isKinematic = false;
            m_isTriggered = true;
        }
    }

    public void ResetMe()
    {
        m_sRend.color = m_startColor;
        m_rigidbody.isKinematic = true;
        m_isTriggered = false;
        transform.position = m_StartPos;
        this.gameObject.SetActive(true);
    }
    #endregion
}
