using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Fall : CollisionHazard_Base, IActivatable
{

    public string m_bulletTag = "Bullet";

	[Header("Fall Hazard Properties")]
	public Bounds m_triggerArea;
	public bool m_drawBoundsInWorldSpace;

	private bool m_isTriggered;

    public float m_lifespan;


    public float m_respawnTime = 3f;
    private Coroutine m_respawnCoroutine;
    private WaitForSeconds m_repsawnDelay;
    private Vector3 m_startScale;
    private Vector3 m_startRotation;
    private bool m_canTrigger;

    [Header("Visuals")]
    public AI_Spawner.RaceType m_hazardType;
    public Sprite m_alienSprite;
    public Sprite m_robotSprite;
    public float m_fadeTimeStart;
    private float m_currentTimer;
    private SpriteRenderer m_sRend;
    public Color m_endColor;
    private Color m_startColor;

    //IActivatable value
    private Vector3 m_StartPos;

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
        m_startScale = transform.localScale;
        m_startRotation = transform.eulerAngles;
        m_repsawnDelay = new WaitForSeconds(m_respawnTime);
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

	private void CheckForTarget()
	{
		Collider2D collider = Physics2D.OverlapBox(m_triggerArea.center, m_triggerArea.size, 0f, m_targetMask);

		if (collider)
		{
			m_rigidbody.isKinematic = false;
			m_isTriggered = true;
            m_currentTimer = 0;
		}
	}

    private  void FadeAway()
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
                m_sRend.color = m_endColor;
                m_respawnCoroutine = StartCoroutine(RespawnHazard());
            }
            
            
        }
        m_currentTimer += Time.deltaTime;

    }

	private void OnDrawGizmos()
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


    private IEnumerator RespawnHazard()
    {
        m_rigidbody.isKinematic = true;
        
        transform.position = m_StartPos;
        transform.localScale = Vector3.zero;
        transform.eulerAngles = m_startRotation;
        
        yield return m_repsawnDelay;
        
        float currentTimer = 0, percent = 0;
        while (currentTimer < 1)
        {
            percent = currentTimer / 1f;
            transform.localScale = Vector3.Lerp(Vector3.zero, m_startScale, percent);
            m_sRend.color = Color.Lerp(m_endColor, m_startColor, percent);
            currentTimer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = m_startScale;
        m_sRend.color = m_startColor;
        m_isTriggered = false;

        
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
        if (m_respawnCoroutine != null)
        {
            StopCoroutine(m_respawnCoroutine);
        }
        transform.localScale = m_startScale;
        transform.eulerAngles = m_startRotation;
        this.gameObject.SetActive(true);
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == m_bulletTag)
        {
            print("Triggered");
            m_rigidbody.isKinematic = false;
            m_isTriggered = true;
            m_currentTimer = 0;
        }
    }

}
