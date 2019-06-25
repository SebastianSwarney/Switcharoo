using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_PlayerSpecificTerrain : MonoBehaviour, IActivatable
{
    bool m_displayGizmos = true;
    public PlayerController.PlayerType m_playerTarget;
    public Vector3Int m_numOfPlatforms = new Vector3Int(1, 1, 0);



    [Header("Pre-set Variables")]
    public string m_humanPlayerLayer;
    public string m_alienPlayerLayer;

    public LayerMask m_robotLayerMask, m_alienLayerMask;
    public Color m_humanColor, m_alienColor;

    public GameObject m_humanPlatform, m_alienPlatform;

    private void Update()
    {
        Collider2D hit = Physics2D.OverlapBox(OriginPosition() + transform.position, new Vector3(m_numOfPlatforms.x - .1f, m_numOfPlatforms.y - .1f, 0), 0, (m_playerTarget == PlayerController.PlayerType.Alien) ? m_alienLayerMask : m_robotLayerMask);
        if (hit != null)
        {
            gameObject.layer = LayerMask.NameToLayer((m_playerTarget == PlayerController.PlayerType.Alien) ? m_alienPlayerLayer : m_humanPlayerLayer);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer((m_playerTarget == PlayerController.PlayerType.Alien) ? m_humanPlayerLayer : m_alienPlayerLayer);
        }

    }

    private void OnDrawGizmos()
    {
        if (!m_displayGizmos) return;
        switch (m_playerTarget)
        {
            case (PlayerController.PlayerType.Alien):
                Gizmos.color = m_alienColor;
                break;

            case (PlayerController.PlayerType.Robot):
                Gizmos.color = m_humanColor;
                break;
        }
        Vector3 newPos = OriginPosition() + transform.position;

        Gizmos.DrawCube(newPos, m_numOfPlatforms);

        

    }

    Vector3 OriginPosition()
    {
        Vector3 newPos = new Vector3();
        if (m_numOfPlatforms.x % 2 == 0)
        {
            newPos = new Vector3(m_numOfPlatforms.x - ((m_numOfPlatforms.x / 2) + .5f), newPos.y, 0);
        }
        else
        {
            newPos = new Vector3(m_numOfPlatforms.x / 2, newPos.y, 0);
        }

        if (m_numOfPlatforms.y % 2 == 0)
        {
            newPos = new Vector3(newPos.x, (m_numOfPlatforms.y) - ((m_numOfPlatforms.y / 2) + .5f), 0);
        }
        else
        {
            newPos = new Vector3(newPos.x, m_numOfPlatforms.y / 2, 0);
        }
        return newPos;
    }

    private void Start()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.offset = OriginPosition();
        col.size = (Vector2Int)m_numOfPlatforms;
        m_displayGizmos = false;
        string currentLayer = "Default";
        GameObject platformType = m_alienPlatform;
        switch (m_playerTarget)
        {
            case (PlayerController.PlayerType.Alien):
                currentLayer = m_humanPlayerLayer;
                platformType = m_alienPlatform;
                break;
            case (PlayerController.PlayerType.Robot):
                currentLayer = m_alienPlayerLayer;
                
                platformType = m_humanPlatform;
                break;
        }


        gameObject.layer = LayerMask.NameToLayer(currentLayer);


        for (int x = 0; x < m_numOfPlatforms.x; x++)
        {
            for (int y = 0; y < m_numOfPlatforms.y; y++)
            {
                ObjectPooler.instance.NewObject(platformType, new Vector3(transform.position.x + x, transform.position.y + y, 0f), Quaternion.identity).transform.parent = this.transform;

            }
        }

    }

    public void ActiveState(bool p_active)
    {
        gameObject.SetActive(p_active);
    }

    public void ResetMe()
    {
        gameObject.SetActive(true);
    }
}
