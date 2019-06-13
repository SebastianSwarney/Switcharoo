using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Move : CollisionHazard_Base, IActivatable
{
	[Header("Moving Properties")]
	public Vector3[] m_localWaypoints;
	Vector3[] m_globalWaypoints;

	public float m_moveSpeed;
	public bool m_isCyclic;
	public float m_waitTime;
	[Range(0, 2)]
	public float m_easeAmount;

	private int m_fromWaypointIndex;
	private float m_percentBetweenWaypoints;
	private float m_nextMoveTime;

    [Header("Active Properties")]
    public bool m_startActive;
    bool m_isActive;
    Vector3 m_startPos;
	public override void Start()
	{
		base.Start();

		m_globalWaypoints = new Vector3[m_localWaypoints.Length];
		for (int i = 0; i < m_localWaypoints.Length; i++)
		{
			m_globalWaypoints[i] = m_localWaypoints[i] + transform.position;
		}

        m_startPos = transform.position;
        m_isActive = m_startActive;
	}

	void Update()
	{
        if (m_isActive)
        {
            Vector3 velocity = CalculatePlatformMovement();
            transform.Translate(velocity);
        }

	}

	float Ease(float x)
	{
		float a = m_easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
	}

	Vector3 CalculatePlatformMovement()
	{

		if (Time.time < m_nextMoveTime)
		{
			return Vector3.zero;
		}

		m_fromWaypointIndex %= m_globalWaypoints.Length;
		int toWaypointIndex = (m_fromWaypointIndex + 1) % m_globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex]);
		m_percentBetweenWaypoints += Time.deltaTime * m_moveSpeed / distanceBetweenWaypoints;
		m_percentBetweenWaypoints = Mathf.Clamp01(m_percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease(m_percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

		if (m_percentBetweenWaypoints >= 1)
		{
			m_percentBetweenWaypoints = 0;
			m_fromWaypointIndex++;

			if (!m_isCyclic)
			{
				if (m_fromWaypointIndex >= m_globalWaypoints.Length - 1)
				{
					m_fromWaypointIndex = 0;
					System.Array.Reverse(m_globalWaypoints);
				}
			}
			m_nextMoveTime = Time.time + m_waitTime;
		}

		return newPos - transform.position;
	}

	void OnDrawGizmos()
	{
		if (m_localWaypoints != null)
		{
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i = 0; i < m_localWaypoints.Length; i++)
			{
				Vector3 globalWaypointPos = (Application.isPlaying) ? m_globalWaypoints[i] : m_localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}

    #region IActivatable Methods
    public void ActiveState(bool p_active)
    {
        m_isActive = p_active;
    }

    public void ResetMe()
    {
        m_isActive = m_startActive;
        transform.position = m_startPos;
    }
    #endregion
}
