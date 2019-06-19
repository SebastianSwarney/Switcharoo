using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Bounds : MonoBehaviour
{
    public Vector2 m_boundsDimensions;
    Bounds m_bounds;


    public bool m_drawBounds;
    public Color m_boundsColor;
    private void Start()
    {
        m_bounds.center = transform.position;
        m_bounds.extents = m_boundsDimensions/2;
    }

    public bool TargetInBounds(Vector3 p_target)
    {
        return m_bounds.Contains(p_target);
    }

    private void OnDrawGizmos()
    {
        if (!m_drawBounds) return;
        Gizmos.color = m_boundsColor;
        Gizmos.DrawWireCube(transform.position, m_boundsDimensions);
        
    }
    public Vector3 PositionInBounds(Vector3 p_targetPos)
    {
        return m_bounds.ClosestPoint(p_targetPos);
    }
}
