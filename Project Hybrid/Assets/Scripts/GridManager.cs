using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Dictionary<Vector3Int, Tile> grid = new Dictionary<Vector3Int, Tile>();

    public void Awake()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in tiles)
        {
            grid.Add(tile.position, tile);
        }
    }

    private void Start()
    {
        foreach(Tile tile in grid.Values)
        {
            tile.StoreNeighbours(grid);
        }
    }

    public Tile GetTile(Vector3Int _pos)
    {
        if (grid.ContainsKey(_pos)) return grid[_pos];
        return null;
    }

    /// <summary>
    /// Uses A* to calculate a path from a starting point to a target position
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_endPos"></param>
    /// <returns></returns>
    public List<Vector3Int> CalculatePath(Vector3Int _startPos, Vector3Int _endPos)
    {
        List<Vector3Int> path = new List<Vector3Int>();

        Node startNode = new Node(_position: _startPos,
                                    _previous: null,
                                    _GScore: 0,
                                    _HScore: CalculateHScore(_startPos, _endPos));

        List<Node> openNodes = new List<Node> { startNode };
        HashSet<Vector3Int> visitedPositions = new HashSet<Vector3Int> { startNode.position };

        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.OrderBy(n => n.FScore).FirstOrDefault();
            openNodes.Remove(currentNode);

            // Return the path if endPosition is found
            if (currentNode.position == _endPos)
            {
                path.Add(_endPos);

                while (currentNode.previous != null)
                {
                    currentNode = currentNode.previous;
                    path.Add(currentNode.position);
                }

                path.Reverse();
                return path;
            }

            Tile currentTile = grid[currentNode.position];
            List<Tile> neighbours = currentTile.neighbours;

            foreach (Tile neighbourTile in neighbours)
            {
                if (visitedPositions.Contains(neighbourTile.position))
                {
                    // Get node, Calculate new score, Update if necassary
                    Node neighbourNode = GetNode(neighbourTile.position, openNodes);
                    if (!openNodes.Contains(neighbourNode)) continue;   // Make sure the node is still open

                    float newGScore = neighbourNode.previous.GScore + 1;
                    if (neighbourNode.GScore > newGScore)
                    {
                        neighbourNode.GScore = newGScore;
                        neighbourNode.previous = currentNode;
                    }
                }
                else
                {
                    // Create new node, add to openNodes
                    Node neighbourNode = new Node(_position: neighbourTile.position,
                                                    _previous: currentNode,
                                                    _GScore: (int)currentNode.GScore + 1,
                                                    _HScore: CalculateHScore(neighbourTile.position, _endPos));

                    openNodes.Add(neighbourNode);
                    visitedPositions.Add(neighbourTile.position);
                }
            }
        }

        return null;
    }

    private Node GetNode(Vector3Int _position, IEnumerable<Node> _nodeList)
    {
        foreach (Node node in _nodeList)
        {
            if (node.position == _position)
            {
                return node;
            }
        }

        return null;
    }

    private int CalculateHScore(Vector3Int gridPosition, Vector3Int endPos)
    {
        Vector3Int difference = endPos - gridPosition;
        int score = Mathf.Abs(difference.x) + Mathf.Abs(difference.z);
        return score;
    }

    private class Node
    {
        public Vector3Int position; //Position on the grid
        public Node previous;

        public float FScore => GScore + HScore;

        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector3Int _position, Node _previous, int _GScore, int _HScore)
        {
            position = _position;
            previous = _previous;
            GScore = _GScore;
            HScore = _HScore;
        }
    }
}


