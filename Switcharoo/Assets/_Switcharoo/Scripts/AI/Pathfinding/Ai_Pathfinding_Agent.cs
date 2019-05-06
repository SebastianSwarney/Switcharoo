﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Pathfinding_Agent : MonoBehaviour
{
    public PlatformerNavigation m_navGrid;
    public Node m_currentNode;
    Node m_targetNode;
    List<Node> m_tracedPath;
    public bool m_drawPath;


    [Space(10)]

    public float m_stoppingDistance;
    bool m_isJumping;
    Rigidbody2D m_rb;
    public float m_gravityValue;
    public LayerMask m_terrainLayer;


    Node m_playernode;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_tracedPath = new List<Node>();
        if(m_navGrid == null) Debug.Log("Nav grid missing from: " + gameObject.name);
    }

    public void MoveToNode(float p_speed, float p_jumpHeight, bool p_isGrounded)
    {
        if (m_tracedPath.Count > 0)
        {
            m_currentNode = m_tracedPath[0];
            if (!IsCloseToPosition(m_currentNode.m_worldPos, p_jumpHeight, p_isGrounded))
            {
                if (m_tracedPath.Count > 0)
                {


                    if (!Physics2D.Linecast(transform.position, m_currentNode.m_worldPos, m_terrainLayer))
                    {

                        if (!p_isGrounded)
                        {
                            if (Mathf.Abs(m_currentNode.m_worldPos.x - transform.position.x) > m_stoppingDistance * 2)
                            {
                                m_rb.velocity = new Vector3(Mathf.Sign(m_currentNode.m_worldPos.x - transform.position.x) * p_speed, m_rb.velocity.y);
                            }else{
                                m_rb.velocity = new Vector3(0, m_rb.velocity.y,0);
                            }
                        }
                        else
                        {
                            m_rb.velocity = new Vector3(Mathf.Sign(m_currentNode.m_worldPos.x - transform.position.x) * p_speed, m_rb.velocity.y);
                        }


                    }
                }
                else
                {
                    return;
                }


            }
        }
        else
        {
            print("Pathfinding postion reached");
        }
    }

    void Jump(float p_jumpHeight, bool p_isGrounded)
    {
        if (p_isGrounded)
        {
            float jumpForce = Mathf.Sqrt(2f * m_gravityValue * p_jumpHeight);
            m_rb.velocity = new Vector3(m_rb.velocity.x, jumpForce, 0);

        }
    }
    bool IsCloseToPosition(Vector3 p_position, float p_jumpHeight, bool p_isGrounded)
    {

        if (Mathf.Abs(p_position.x - transform.position.x) < m_stoppingDistance)
        {
            if (p_isGrounded)
            {
                m_tracedPath.RemoveAt(0);
            }

            if (ShouldIJump())
            {
                Jump(p_jumpHeight, p_isGrounded);
            }
            


            return true;
        }
        return false;
    }

    public bool TargetPositionReached(Vector3 p_targetPos)
    {
        if (m_tracedPath.Count > 1)
        {
            return false;
        }

        
        float dis = Mathf.Abs(p_targetPos.x - transform.position.x);
        
        return (dis < m_stoppingDistance) ? true : false;
    }
    bool ShouldIJump()
    {


        if (m_navGrid.NodeFromWorldPoint(transform.position).m_gridPos.y < m_tracedPath[0].m_gridPos.y)
        {
            return true;
        }
        return false;
    }

    #region  Path Tracing

    ///<Summary>
    ///Calculates the path towards the m_target
    public void CreatePath(Vector3 p_startPoint, Vector3 p_targetPoint, float p_jumpHeight)
    {
        m_tracedPath.Clear();
        //Gets both positions, in terms of nodes
        Node p_startNode = m_navGrid.NodeFromWorldPoint(transform.position);
        Node p_endNode = m_navGrid.NodeFromWorldPoint(p_targetPoint);
        m_targetNode = p_endNode;

        Heap<Node> openNodes = new Heap<Node>(m_navGrid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();
        openNodes.Add(p_startNode);


        while (openNodes.Count > 0)
        {
            Node p_currentNode = openNodes.RemoveFirst();
            closedNodes.Add(p_currentNode);

            //If its the m_target node, stop calculating
            if (p_currentNode == p_endNode)
            {
                m_tracedPath = RetracePath(p_startNode, p_endNode);
                
                m_currentNode = m_tracedPath[0];
                return;
            }


            foreach (Node neighbour in m_navGrid.GetNeighbours(p_currentNode))
            {

                if (!neighbour.m_walkable || closedNodes.Contains(neighbour) || neighbour.m_gridPos.y > p_currentNode.m_gridPos.y && neighbour.m_gridPos.y - p_currentNode.m_gridPos.y > p_jumpHeight)
                {
                    continue;
                }


                int newMoveCostToNeighbour = p_currentNode.gCost + GetDistance(p_currentNode, neighbour);
                if (newMoveCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.gCost = newMoveCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, p_endNode);
                    neighbour.parent = p_currentNode;

                    if (!openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }
        Debug.Log("Path could not be calculated!");
    }

    ///<Summary>
    ///Returns the path to the m_target
    List<Node> RetracePath(Node p_startNode, Node p_endNode)
    {
        List<Node> path = new List<Node>();

        Node m_currentNode = p_endNode;

        while (m_currentNode != p_startNode)
        {
            path.Add(m_currentNode);
            m_currentNode = m_currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    ///<Summary>
    ///Used when finding the costs for the nodes
    int GetDistance(Node p_nodeA, Node p_nodeB)
    {
        int distX = Mathf.Abs(p_nodeA.m_gridPos.x - p_nodeB.m_gridPos.x);
        int distY = Mathf.Abs(p_nodeA.m_gridPos.y - p_nodeB.m_gridPos.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
    #endregion


    #region modular enemies
    public bool TargetMoved(Vector3 p_targetPosition)
    {
        Node testNode = m_navGrid.NodeFromWorldPoint(p_targetPosition);
        if (m_targetNode.m_worldPos != testNode.m_worldPos)
        {
            return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        if (!m_drawPath) return;

        if (m_tracedPath != null)
        {
            foreach (Node n in m_tracedPath)
            {

                Gizmos.color = Color.cyan;
                if (n == m_targetNode)
                {
                    Gizmos.color = Color.magenta;
                }
                if (n == m_playernode)
                {
                    Gizmos.color = Color.white;

                }
                Gizmos.DrawCube(n.m_worldPos, Vector3.one * (m_navGrid.m_nodeRadius * 2 - .1f));

            }
        }
    }
}