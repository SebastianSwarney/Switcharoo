﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_PlayerSpecificTerrain : MonoBehaviour
{
    bool m_displayGizmos = true;
    public PlayerController.PlayerType m_playerTarget;
    public Vector3Int m_numOfPlatforms = new Vector3Int(1,1,0);
    

    [Header("Pre-set Variables")]
    public string m_humanPlayerLayer;
    public string m_alienPlayerLayer;
    public Color m_humanColor, m_alienColor;

    public GameObject m_humanPlatform, m_alienPlatform;

    private void OnDrawGizmos()
    {
        if (!m_displayGizmos) return;
        switch (m_playerTarget)
        {
            case (PlayerController.PlayerType.Type0) :
                Gizmos.color = m_humanColor;
                break;

            case (PlayerController.PlayerType.Type1) :
                Gizmos.color = m_alienColor;
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
            case (PlayerController.PlayerType.Type0):
                currentLayer = m_humanPlayerLayer;
                platformType = m_humanPlatform;
                break;
            case (PlayerController.PlayerType.Type1):
                currentLayer = m_alienPlayerLayer;
                platformType = m_alienPlatform;
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
}