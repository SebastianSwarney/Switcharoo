using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Rotate : CollisionHazard_Base, IActivatable
{
    [Header("Rotate Hazard Properties")]
    [Range(0, 180)]
    public float m_rotateDegrees;
    public float m_rotateSpeed;
    public Transform m_rotateTarget;
    public LayerMask m_layerObstacleMask;

    private LineRenderer m_lineRenderer;
    private float m_rotateTimer = 0;
    private BoxCollider2D m_collider;

    GameObject m_particleSystemObject;
    public GameObject m_particleSystem;
    [Header("Active Properties")]
    public bool m_startActive;
    bool m_isActive;
    float m_startingRotation;
    

    public override void Start()
    {
        base.Start();


        m_particleSystemObject = ObjectPooler.instance.NewObject(m_particleSystem,transform.position, Quaternion.identity);
        m_particleSystem.SetActive(m_startActive);
        
        m_lineRenderer = GetComponent<LineRenderer>();
        m_collider = GetComponentInChildren<BoxCollider2D>();
        m_rigidbody = GetComponentInChildren<Rigidbody2D>();

        m_lineRenderer.useWorldSpace = true;


        m_startingRotation = transform.eulerAngles.z;


        m_isActive = m_startActive;
        ActiveState(m_isActive);
    }

    private void Update()
    {
        if (m_paused) return;
        if (m_isActive)
        {
            Rotate();
        }
        
    }

    private void Rotate()
    {
        m_rotateTimer += Time.deltaTime;
        float angle = Mathf.Sin(m_rotateTimer * m_rotateSpeed) * m_rotateDegrees;
        m_rotateTarget.localRotation = Quaternion.AngleAxis(angle, transform.forward);

        RaycastHit2D hit = Physics2D.Raycast(m_rotateTarget.position, m_rotateTarget.right, Mathf.Infinity, m_layerObstacleMask);

        if (hit)
        {
            m_lineRenderer.SetPosition(0, transform.position);
            m_lineRenderer.SetPosition(1, hit.point);

            float dst = Vector3.Distance(transform.position, hit.point);
            m_collider.size = new Vector2(dst, m_collider.size.y);
            m_collider.offset = new Vector2(m_collider.size.x / 2, m_collider.offset.y);

            m_particleSystemObject.transform.position = hit.point;
            angle = Mathf.Atan2(transform.position.y - m_particleSystemObject.transform.position.y, transform.position.x - m_particleSystemObject.transform.position.x) * Mathf.Rad2Deg;
            m_particleSystemObject.transform.eulerAngles = new Vector3(0f,0f,angle);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 lineEnd1 = Quaternion.Euler(0, 0, m_rotateDegrees) * transform.right;
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, lineEnd1, Mathf.Infinity, m_layerObstacleMask);

        if (hit1)
        {
            Debug.DrawLine(m_rotateTarget.position, hit1.point, Color.red);
        }

        Vector3 lineEnd2 = Quaternion.Euler(0, 0, -m_rotateDegrees) * transform.right;
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, lineEnd2, Mathf.Infinity, m_layerObstacleMask);

        if (hit2)
        {
            Debug.DrawLine(m_rotateTarget.position, hit2.point, Color.red);
        }
    }


    #region IActivatable Methods
    public void ActiveState(bool p_active)
    {
        m_isActive = p_active;
        m_lineRenderer.enabled = p_active;
        m_particleSystemObject.SetActive(p_active);
        this.enabled = p_active;
    }

    public void ResetMe()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, m_startingRotation);
        m_isActive = m_startActive;
        m_particleSystemObject.SetActive(m_isActive);
    }
    #endregion
}
