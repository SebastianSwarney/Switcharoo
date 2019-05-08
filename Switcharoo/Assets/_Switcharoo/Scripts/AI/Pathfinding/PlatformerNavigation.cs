using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlatformerNavigation : MonoBehaviour
{
    public bool displayGizmos = false, displayPaths = false;

    public Vector2 m_gridWorldSize;
    public float m_nodeRadius;
    Node[,] m_nodeGrid;
    float m_nodeDiameter;
    Vector2Int m_gridSize;
    public LayerMask m_terrainLayer;
    public List<NodeDisplay> m_nodeDisplayInspector = new List<NodeDisplay>();

    public int m_maxPlatformGap = 3;
    void Start()
    {
        CreateGrid();
    }
    public int MaxSize
    {
        get
        {
            return m_gridSize.x * m_gridSize.y;
        }
    }
    private void Update()
    {
        if(!displayPaths)return;
        foreach (Node p_newNode in m_nodeGrid)
        {
            foreach (Node.NodeConnection connect in p_newNode.m_connectedTo)
            {
                Debug.DrawLine(p_newNode.m_worldPos, connect.m_connectedTo.m_worldPos, Color.green);
            }
        }
    }
    public void CreateGrid()
    {
        m_nodeDiameter = m_nodeRadius * 2;
        m_gridSize = new Vector2Int(Mathf.RoundToInt(m_gridWorldSize.x / m_nodeDiameter),Mathf.RoundToInt(m_gridWorldSize.y / m_nodeDiameter));
        m_nodeGrid = new Node[m_gridSize.x, m_gridSize.y];
        Vector2 p_worldBottomLeft = transform.position - Vector3.right * m_gridWorldSize.x / 2 - Vector3.up * m_gridWorldSize.y / 2;
        int count = 0;
        for (int y = 0; y < m_gridSize.y; y++)
        {
            for (int x = 0; x < m_gridSize.x; x++)
            {
                count++;
                //print("Node: " + count);

                if (m_nodeGrid[x, y] == null)
                {


                    m_nodeGrid[x, y] = CreateNode(x, y, p_worldBottomLeft);
                    //print("Node World Pos: " + m_nodeGrid[x, y].m_worldPos);

                }




            }
        }


        //Create the ground paths
        for (int y = 0; y < m_gridSize.y; y++)
        {
            for (int x = 0; x < m_gridSize.x; x++)
            {

                if (m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Walkable || m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Platform)
                {
                    CreateFlatPaths(m_nodeGrid[x, y]);
                }
                m_nodeDisplayInspector.Add(new NodeDisplay(m_nodeGrid[x, y]));
            }
        }
    }

    Node CreateNode(int p_xPos, int p_yPos, Vector3 p_worldBottomLeft)
    {

        Node newNode = new Node();
        //Get the world point of the current p_node, using the bottom left
        Vector3 worldPoint = p_worldBottomLeft + Vector3.right * (p_xPos * m_nodeDiameter + m_nodeRadius) + Vector3.up * (p_yPos * m_nodeDiameter + m_nodeRadius);
        bool isWalkable = !(Physics2D.OverlapCircle(worldPoint, .1f, m_terrainLayer));



        //If the currentBlock is terrain
        if (!isWalkable)
        {

            if ((p_yPos + 1) < m_gridSize.y)
            {

                //Find the p_node above
                worldPoint = p_worldBottomLeft + Vector3.right * (p_xPos * m_nodeDiameter + m_nodeRadius) + Vector3.up * ((p_yPos + 1) * m_nodeDiameter + m_nodeRadius);
                isWalkable = !(Physics2D.OverlapCircle(worldPoint, .1f, m_terrainLayer));


                //If the p_node above is walkable
                if (isWalkable)
                {


                    worldPoint = p_worldBottomLeft + Vector3.right * (p_xPos * m_nodeDiameter + m_nodeRadius) + Vector3.up * ((p_yPos) * m_nodeDiameter + m_nodeRadius);
                    newNode.NodeInitiate(false, worldPoint, p_xPos, p_yPos, Node.NodeType.Blocked, 0);


                    #region Above Node Initiation

                    Node aboveNode = new Node();
                    int freeSpace = 0;

                    //Finding the space above the current p_node, to determine how much room there is
                    for (int y = p_yPos + 1; y < m_gridSize.y; y++)
                    {
                        //print("Freespace y check: " + y);
                        worldPoint = p_worldBottomLeft + Vector3.right * (p_xPos * m_nodeDiameter + m_nodeRadius) + Vector3.up * ((y) * m_nodeDiameter + m_nodeRadius);
                        isWalkable = !(Physics2D.OverlapCircle(worldPoint, .1f, m_terrainLayer));
                        if (isWalkable)
                        {
                            freeSpace++;
                        }
                        else
                        {
                            break;
                        }
                    }


                    worldPoint = p_worldBottomLeft + Vector3.right * (p_xPos * m_nodeDiameter + m_nodeRadius) + Vector3.up * ((p_yPos + 1) * m_nodeDiameter + m_nodeRadius);
                    aboveNode.NodeInitiate(true, worldPoint, p_xPos, p_yPos + 1, Node.NodeType.Walkable, freeSpace);




                    //Check left and right
                    for (int x = p_xPos - 1; x < p_xPos + 2; x++)
                    {

                        if (x == p_xPos || x == -1 || x > m_gridSize.x - 1) continue;


                        worldPoint = p_worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.up * (p_yPos * m_nodeDiameter + m_nodeRadius);
                        isWalkable = !(Physics2D.OverlapCircle(worldPoint, .1f, m_terrainLayer));

                        if (isWalkable)
                        {
                            worldPoint = p_worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.up * (p_yPos + 1 * m_nodeDiameter + m_nodeRadius);
                            isWalkable = !(Physics2D.OverlapCircle(worldPoint, .1f, m_terrainLayer));
                            if (isWalkable)
                            {


                                aboveNode.m_currentNodeType = Node.NodeType.Platform;

                                //If the empty block is to the left
                                if (x < p_xPos)
                                {
                                    if (aboveNode.m_currentPlatformType == Node.PlatformType.Right)
                                    {
                                        aboveNode.m_currentPlatformType = Node.PlatformType.Both;
                                    }
                                    else
                                    {
                                        aboveNode.m_currentPlatformType = Node.PlatformType.Left;
                                    }



                                }


                                //If the empty block is to the right
                                else if (x > p_xPos)
                                {

                                    if (aboveNode.m_currentPlatformType == Node.PlatformType.Left)
                                    {
                                        aboveNode.m_currentPlatformType = Node.PlatformType.Both;
                                    }
                                    else
                                    {
                                        aboveNode.m_currentPlatformType = Node.PlatformType.Right;
                                    }


                                }
                            }
                        }
                    }

                    m_nodeGrid[aboveNode.m_gridPos.x, aboveNode.m_gridPos.y] = aboveNode;
                    #endregion
                }

                //If the p_node above is not walkable
                else
                {

                    worldPoint = p_worldBottomLeft + Vector3.right * (p_xPos * m_nodeDiameter + m_nodeRadius) + Vector3.up * ((p_yPos) * m_nodeDiameter + m_nodeRadius);
                    newNode.NodeInitiate(false, worldPoint, p_xPos, p_yPos, Node.NodeType.Blocked, 0);
                }

            }
            else
            {

                newNode.NodeInitiate(false, worldPoint, p_xPos, p_yPos, Node.NodeType.Blocked, 0);
            }
        }
        else
        {
            newNode.NodeInitiate(true, worldPoint, p_xPos, p_yPos, Node.NodeType.Empty, 0);
        }

        return newNode;
    }

    void CreateFlatPaths(Node p_currentNode)
    {
        for (int x = p_currentNode.m_gridPos.x - 1; x < p_currentNode.m_gridPos.x + 2; x++)
        {
            if (x < 0 || x > m_gridSize.x - 1 || x == p_currentNode.m_gridPos.x) continue;
            if (m_nodeGrid[x, p_currentNode.m_gridPos.y].m_currentNodeType == Node.NodeType.Walkable || m_nodeGrid[x, p_currentNode.m_gridPos.y].m_currentNodeType == Node.NodeType.Platform)
            {
                p_currentNode.m_connectedTo.Add(new Node.NodeConnection(m_nodeGrid[x, p_currentNode.m_gridPos.y], (x < p_currentNode.m_gridPos.x) ? Node.NodeConnection.ConnectionType.Left : Node.NodeConnection.ConnectionType.Right));
            }
        }

        if (p_currentNode.m_currentNodeType == Node.NodeType.Platform)
        {
            Vector2Int xPos = new Vector2Int();
            if (p_currentNode.m_currentPlatformType == Node.PlatformType.Left || p_currentNode.m_currentPlatformType == Node.PlatformType.Both)
            {
                xPos = new Vector2Int(-m_maxPlatformGap + p_currentNode.m_gridPos.x, 0 + p_currentNode.m_gridPos.x);
                bool skipOther = false;
                for (int x = xPos.y; x > xPos.x - 1; x--)
                {
                    if (x == p_currentNode.m_gridPos.x || x >= m_gridSize.x || x < 0) continue;
                    //Check the m_nodeGrid underneath, to assign a platform connection
                    for (int y = p_currentNode.m_gridPos.y + 1; y > 0; y--)
                    {
                        if(skipOther && y == p_currentNode.m_gridPos.y && m_nodeGrid[x,y].m_currentNodeType == Node.NodeType.Walkable ||
                        skipOther && y == p_currentNode.m_gridPos.y && m_nodeGrid[x,y].m_currentNodeType == Node.NodeType.Platform){
                            p_currentNode.m_connectedTo.Add(new Node.NodeConnection(m_nodeGrid[x, y], (x < p_currentNode.m_gridPos.x) ? Node.NodeConnection.ConnectionType.Left : Node.NodeConnection.ConnectionType.Right));
                            continue;
                        }
                        if (y >= m_gridSize.y || skipOther) continue;
                        if (m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Walkable || m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Platform)
                        {
                            if (y == p_currentNode.m_gridPos.y)
                            {
                                p_currentNode.m_connectedTo.Add(new Node.NodeConnection(m_nodeGrid[x, y], (x < p_currentNode.m_gridPos.x) ? Node.NodeConnection.ConnectionType.Left : Node.NodeConnection.ConnectionType.Right));
                            }
                            else
                            {
                                m_nodeGrid[x, y].m_connectedTo.Add(new Node.NodeConnection(p_currentNode, Node.NodeConnection.ConnectionType.Above));
                                p_currentNode.m_connectedTo.Add(new Node.NodeConnection(m_nodeGrid[x, y], Node.NodeConnection.ConnectionType.Below));
                            }
                            if (x == p_currentNode.m_gridPos.x - 1 && y == p_currentNode.m_gridPos.y - 1 || x == p_currentNode.m_gridPos.x - 1 && y == p_currentNode.m_gridPos.y - 2 || x == p_currentNode.m_gridPos.x - 2 && y == p_currentNode.m_gridPos.y - 3 || x == p_currentNode.m_gridPos.x - 2 && y == p_currentNode.m_gridPos.y - 2)
                            {
                                skipOther = true;
                                break;
                            }
                            break;

                        }

                    }
                }
            }
            if (p_currentNode.m_currentPlatformType == Node.PlatformType.Right || p_currentNode.m_currentPlatformType == Node.PlatformType.Both)
            {
                xPos = new Vector2Int(1 + p_currentNode.m_gridPos.x, m_maxPlatformGap + 1 + p_currentNode.m_gridPos.x);
                bool skipOther = false;
                for (int x = xPos.x; x < xPos.y; x++)
                {
                    if (x == p_currentNode.m_gridPos.x || x >= m_gridSize.x || x < 0) continue;
                    //Check the m_nodeGrid underneath, to assign a platform connection
                    for (int y = p_currentNode.m_gridPos.y + 1; y > 0; y--)
                    {
                        if (y >= m_gridSize.y || skipOther) continue;
                        if (m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Walkable || m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Platform)
                        {
                            if (y == p_currentNode.m_gridPos.y)
                            {
                                p_currentNode.m_connectedTo.Add(new Node.NodeConnection(m_nodeGrid[x, y], (x < p_currentNode.m_gridPos.x) ? Node.NodeConnection.ConnectionType.Left : Node.NodeConnection.ConnectionType.Right));
                            }
                            else
                            {
                                m_nodeGrid[x, y].m_connectedTo.Add(new Node.NodeConnection(p_currentNode, Node.NodeConnection.ConnectionType.Above));
                                p_currentNode.m_connectedTo.Add(new Node.NodeConnection(m_nodeGrid[x, y], Node.NodeConnection.ConnectionType.Below));
                            }
                            if (x == p_currentNode.m_gridPos.x + 1 && y == p_currentNode.m_gridPos.y - 1 || x == p_currentNode.m_gridPos.x + 1 && y == p_currentNode.m_gridPos.y - 2 || x == p_currentNode.m_gridPos.x + 2 && y == p_currentNode.m_gridPos.y - 3 || x == p_currentNode.m_gridPos.x + 2 && y == p_currentNode.m_gridPos.y - 2)
                            {
                                skipOther = true;
                                break;
                            }
                            break;

                        }

                    }
                }
            }


        }



    }

    public Node NodeFromWorldPoint(Vector3 p_worldPos)
    {
        Node returnNode;
        //the m_nodeGrid starts at 0, so you have to account for that
        //IE, if the point was at -15, and the gridsize was 15, the point is 0
        float percentX = (p_worldPos.x-transform.position.x + m_gridWorldSize.x / 2) / m_gridWorldSize.x;
        float percentY = (p_worldPos.y-transform.position.y + m_gridWorldSize.y / 2) / m_gridWorldSize.y;

        //Create the percentage of the current position on the m_nodeGrid
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Calculate the actual positon, into an int
        int x = Mathf.RoundToInt((m_gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSize.y - 1) * percentY);
        returnNode = m_nodeGrid[x, y];

        if (m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Empty || m_nodeGrid[x, y].m_currentNodeType == Node.NodeType.Blocked)
        {

            float closestDistance = Mathf.Infinity;
            foreach (Node currentNode in m_nodeGrid)
            {

                if (currentNode.m_currentNodeType == Node.NodeType.Walkable || currentNode.m_currentNodeType == Node.NodeType.Platform)
                {

                    float currentDistance = Vector2.Distance(p_worldPos, currentNode.m_worldPos);
                    if (currentDistance < closestDistance)
                    {
                        if (!Physics2D.Linecast(p_worldPos, currentNode.m_worldPos, m_terrainLayer))
                        {
                            
                            closestDistance = currentDistance;
                            
                            returnNode = currentNode;
                        }
                    }

                }

            }

        }
        return returnNode;
    }
    public Vector3 NodeToWorldPoint(Node p_navNode)
    {
        for (int x = 0; x < m_gridSize.x; x++)
        {
            for (int y = 0; y < m_gridSize.y; y++)
            {
                if (m_nodeGrid[x, y] == p_navNode)
                {
                    float p_xPos = (x * m_nodeDiameter) - m_gridWorldSize.x / 2;
                    float p_yPos = (y * m_nodeDiameter) - m_gridWorldSize.y / 2;
                    return new Vector3(p_xPos, 0f, p_yPos);
                }
            }
        }
        Debug.Log("Node does not exist in current m_nodeGrid. Defaulting to origin");
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(m_gridWorldSize.x, m_gridWorldSize.y, 0));


        if (m_nodeGrid != null)
        {
            foreach (Node n in m_nodeGrid)
            {

                if (n.m_currentNodeType == Node.NodeType.Blocked)
                {
                    Gizmos.color = Color.red;
                }
                else if (n.m_currentNodeType == Node.NodeType.Empty)
                {
                    Gizmos.color = Color.cyan;
                    
                }
                else if (n.m_currentNodeType == Node.NodeType.Platform)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (n.m_currentNodeType == Node.NodeType.Walkable)
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(n.m_worldPos, Vector3.one * (m_nodeDiameter - .1f));
            }
        }


    }


    public List<Node> GetNeighbours(Node p_node)
    {
        List<Node> neighbours = new List<Node>();
        foreach (Node.NodeConnection nodeConnect in p_node.m_connectedTo)
        {
            neighbours.Add(nodeConnect.m_connectedTo);
        }
        return neighbours;
    }

    [System.Serializable]
    public struct NodeDisplay
    {
        public Vector2 m_gridPos;
        public Vector3 m_worldPos;
        public Node.NodeType m_currentNodeType;
        public List<Node.NodeConnection> m_nodeConnections;
        public Node.PlatformType m_currentPlatformType;

        public int space;
        public NodeDisplay(Node p_newNode)
        {
            m_gridPos = new Vector2(p_newNode.m_gridPos.x, p_newNode.m_gridPos.y);
            m_worldPos = p_newNode.m_worldPos;
            m_currentNodeType = p_newNode.m_currentNodeType;
            m_nodeConnections = p_newNode.m_connectedTo;
            m_currentPlatformType = p_newNode.m_currentPlatformType;
            space = p_newNode.m_nodeSpace;
        }
    }
}


public class Node : IHeapItem<Node>
{
    #region Platformer Specific Varialbes
    public enum NodeType { Blocked, Walkable, Empty, Platform }
    public enum PlatformType { Left, Right, Both, Neither };
    public NodeType m_currentNodeType;

    public PlatformType m_currentPlatformType;
    public int m_nodeSpace;

    public List<NodeConnection> m_connectedTo = new List<NodeConnection>();
    #endregion

    #region a* variables
    public bool m_walkable;
    public Vector3 m_worldPos;
    public Vector2Int m_gridPos;
    public int gCost, hCost;
    public Node parent;
    int heapIndex;

    #endregion



    public void NodeInitiate(bool p_walkable, Vector3 p_worldPos, int p_gridX, int p_gridY, NodeType p_nodeType, int p_nodeSpace)
    {
        m_walkable = p_walkable;
        m_worldPos = p_worldPos;
        m_gridPos = new Vector2Int(p_gridX, p_gridY);
        m_currentNodeType = p_nodeType;
        m_nodeSpace = p_nodeSpace;
        m_currentPlatformType = PlatformType.Neither;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }


    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node compareNode)
    {
        int compare = fCost.CompareTo(compareNode.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(compareNode.hCost);
        }

        return -compare;

    }

    [System.Serializable]
    public struct NodeConnection
    {
        public enum ConnectionType { Above, Below, Left, Right }
        public ConnectionType m_connectType;
        public Node m_connectedTo;
        public Vector2 m_connectId;
        public NodeConnection(Node p_connectedTo, ConnectionType p_connectType)
        {
            m_connectType = p_connectType;
            m_connectedTo = p_connectedTo;
            m_connectId = new Vector2(m_connectedTo.m_gridPos.x, m_connectedTo.m_gridPos.y);
        }

    }
}

#region Heap
public class Heap<T> where T : IHeapItem<T>
{

    T[] m_items;
    int m_currentItemCount;
    public Heap(int p_maxHeapSize)
    {
        m_items = new T[p_maxHeapSize];

    }

    public void Add(T p_item)
    {
        p_item.HeapIndex = m_currentItemCount;
        m_items[m_currentItemCount] = p_item;
        SortUp(p_item);
        m_currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = m_items[0];
        m_currentItemCount--;
        m_items[0] = m_items[m_currentItemCount];
        m_items[0].HeapIndex = 0;

        SortDown(m_items[0]);
        return firstItem;
    }

    public void UpdateItem(T p_item)
    {
        SortUp(p_item);

    }

    public int Count
    {
        get
        {
            return m_currentItemCount;
        }
    }

    public bool Contains(T p_item)
    {
        return Equals(m_items[p_item.HeapIndex], p_item);
    }


    void SortDown(T p_item)
    {
        while (true)
        {
            int childIndexLeft = p_item.HeapIndex * 2 + 1;
            int childIndexRight = p_item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < m_currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < m_currentItemCount)
                {
                    if (m_items[childIndexLeft].CompareTo(m_items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (p_item.CompareTo(m_items[swapIndex]) < 0)
                {
                    SwapItem(p_item, m_items[swapIndex]);
                }
                else
                {
                    return;
                }

                //The parent has no children
            }
            else
            {
                return;
            }
        }
    }

    void SortUp(T p_item)
    {
        int parentIndex = (p_item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = m_items[parentIndex];
            if (p_item.CompareTo(parentItem) > 0)
            {
                SwapItem(p_item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (p_item.HeapIndex - 1) / 2;
        }
    }

    void SwapItem(T p_itemA, T p_itemB)
    {
        m_items[p_itemA.HeapIndex] = p_itemB;
        m_items[p_itemB.HeapIndex] = p_itemA;

        int tempHeapIndex = p_itemA.HeapIndex;
        p_itemA.HeapIndex = p_itemB.HeapIndex;
        p_itemB.HeapIndex = tempHeapIndex;
    }
}


public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
#endregion
