using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_ForceFieldWall : MonoBehaviour, IActivatable
{
    [Header("Set Up")]
    public bool m_displayGizmos = true;
    public Vector3Int m_wallDimensions = new Vector3Int(1, 1, 0);
    public bool m_rotateZ = false;

    [Header("Wall Properties")]

    public bool m_triggerActivated;
    bool m_activated;
    public Vector2 m_triggerOffset;
    public Vector2 m_triggerDimensions;

    [Header("Preset values")]
    public Color m_forceFieldGizmoColor;
    public GameObject m_forceFieldObject;
    public GameObject m_topEmitter, m_centerEmitter, m_bottomEmitter;
    public LayerMask m_playerLayers;
    BoxCollider2D m_collider;
    Transform m_forceFieldParent;

    [Header("Active Properties")]
    public bool m_startActive;

    private void OnDrawGizmos()
    {
        if (!m_displayGizmos) return;

        Vector3 newPos = OriginPosition() + transform.position;
        Gizmos.color = m_forceFieldGizmoColor;
        Gizmos.DrawCube(newPos, m_wallDimensions);

        if (!m_triggerActivated) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_triggerOffset + (Vector2)transform.position, m_triggerDimensions);


    }

    Vector3 OriginPosition()
    {
        Vector3 newPos = new Vector3();
        if (m_wallDimensions.x % 2 == 0)
        {
            newPos = new Vector3(m_wallDimensions.x - ((m_wallDimensions.x / 2) + .5f), newPos.y, 0);
        }
        else
        {
            newPos = new Vector3(m_wallDimensions.x / 2, newPos.y, 0);
        }

        if (m_wallDimensions.y % 2 == 0)
        {
            newPos = new Vector3(newPos.x, (m_wallDimensions.y) - ((m_wallDimensions.y / 2) + .5f), 0);
        }
        else
        {
            newPos = new Vector3(newPos.x, m_wallDimensions.y / 2, 0);
        }
        return newPos;
    }


    private void Awake()
    {
        m_forceFieldParent = transform.GetChild(0);
        m_collider = GetComponent<BoxCollider2D>();
        m_collider.offset = OriginPosition();
        m_collider.size = (Vector2Int)m_wallDimensions;
        m_displayGizmos = false;
    }

    private void OnEnable()
    {
        m_activated = false;
        if (!m_startActive)
        {
            BarrierActive(false);
        }
    }

    private void Start()
    {
        for (int x = 0; x < m_wallDimensions.x; x++)
        {
            for (int y = 0; y < m_wallDimensions.y; y++)
            {
                GameObject field = ObjectPooler.instance.NewObject(m_forceFieldObject, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity);
                field.transform.parent = m_forceFieldParent;
                if (!m_rotateZ)
                {
                    if (x == 0 || x == m_wallDimensions.x - 1)
                    {
                        GameObject emitterPiece = null;
                        if (y == 0)
                        {
                            emitterPiece = ObjectPooler.instance.NewObject(m_bottomEmitter, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity);
                        }
                        else if (y == m_wallDimensions.y - 1)
                        {
                            emitterPiece = ObjectPooler.instance.NewObject(m_topEmitter, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity);
                        }
                        else
                        {
                            emitterPiece = ObjectPooler.instance.NewObject(m_centerEmitter, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity);
                        }
                        emitterPiece.transform.localScale = new Vector3((x == 0) ? 1 : -1, 1, 1);
                        emitterPiece.transform.GetChild(0).transform.parent = m_forceFieldParent;
                        emitterPiece.transform.parent = this.transform;
                    }
                }
                else
                {
                    field.transform.eulerAngles = new Vector3(0, 0, 90);
                    if (y == 0 || y == m_wallDimensions.x - 1)
                    {
                        GameObject emitterPiece = null;
                        if (x == 0)
                        {
                            emitterPiece = ObjectPooler.instance.NewObject(m_bottomEmitter, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity);
                        }
                        else if (x == m_wallDimensions.x - 1)
                        {
                            emitterPiece = ObjectPooler.instance.NewObject(m_topEmitter, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity);
                        }
                        else
                        {
                            emitterPiece = ObjectPooler.instance.NewObject(m_centerEmitter, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity);
                        }
                        emitterPiece.transform.eulerAngles = new Vector3(0f, 0f, -90);
                        emitterPiece.transform.localScale = new Vector3((y == 0) ? -1 : 1, 1, 1);

                        
                        emitterPiece.transform.GetChild(0).transform.parent = m_forceFieldParent;
                        emitterPiece.transform.parent = this.transform;
                    }
                }
            }
        }
    }




    private void Update()
    {
        if (m_triggerActivated && !m_startActive && !m_activated)
        {
            Collider2D playerCollided = Physics2D.OverlapBox(m_triggerOffset + (Vector2)transform.position, m_triggerDimensions, 0f, m_playerLayers);
            if (playerCollided != null)
            {
                print("Activate");
                m_activated = true;
                BarrierActive(true);
            }
        }
    }


    void BarrierActive(bool p_activate)
    {
        m_forceFieldParent.gameObject.SetActive(p_activate);
        m_collider.enabled = p_activate;
    }

    public void ActiveState(bool p_active)
    {
        BarrierActive(p_active);
    }

    public void ResetMe()
    {
        m_activated = false;
        if (!m_startActive)
        {
            BarrierActive(false);
        }
        else
        {
            BarrierActive(true);
        }
    }
}
