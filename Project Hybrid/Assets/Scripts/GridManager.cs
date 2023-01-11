using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public int numOfHighTiles = 10;
    public GameObject playerPrefab;
    public Dictionary<Vector3Int, Tile> grid = new Dictionary<Vector3Int, Tile>();
    public List<Tile> tiles => grid.Values.ToList();

    private List<WaterTile> highTiles = new List<WaterTile>();

    public void Awake()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in tiles)
        {
            grid.Add(tile.position, tile);
        }

        foreach (Tile tile in tiles)
        {
            tile.StoreNeighbours(grid);
        }
    }

    private void Start()
    {
        DivideWaterLevels();
        SpawnBoat();
    }

    private void OnEnable()
    {
        foreach (Tile tile in tiles)
        {
            if (!(tile is DamTile)) continue;
            DamTile dam = (DamTile)tile;

            dam.OnOpen += OnDamOpen;
            dam.OnClose += OnDamClose;
        }
    }

    private void OnDisable()
    {
        foreach (Tile tile in tiles)
        {
            if (!(tile is DamTile)) continue;
            DamTile dam = (DamTile)tile;

            dam.OnOpen -= OnDamOpen;
            dam.OnClose -= OnDamClose;
        }
    }

    private void DivideWaterLevels()
    {
        DistributeHighTiles(highTiles);

        foreach (WaterTile tile in highTiles)
        {
            // Geef alle neighbours een waterlevel van eigen -1 in. Herhaal voor iedereens neighbours.
            DecideNeighbourWaterlevel(tile);
        }
    }

    private void DistributeHighTiles(List<WaterTile> _highTiles)
    {
        for (int i = 0; i < numOfHighTiles; i++)
        {
            WaterTile randomTile = tiles[Random.Range(0, tiles.Count)] as WaterTile;
            if (randomTile == null || randomTile is DamTile) { i--; continue; }
            randomTile.waterLevel = 5;
            _highTiles.Add(randomTile);
        }
    }

    private void OnDamOpen(DamTile _dam)
    {
        foreach (WaterTile tile in highTiles)
        {
            tile.waterLevel = 5;
            // Geef alle neighbours een waterlevel van eigen -1 in. Herhaal voor iedereens neighbours.
            DecideNeighbourWaterlevel(tile);
        }
    }

    private void OnDamClose(DamTile _dam)
    {
        ResetAllWaterTiles();
        _dam.waterLevel = 0;
        foreach (WaterTile tile in highTiles)
        {
            tile.waterLevel = 5;
            // Geef alle neighbours een waterlevel van eigen -1 in. Herhaal voor iedereens neighbours.
            DecideNeighbourWaterlevel(tile);
        }
    }

    private void DecideNeighbourWaterlevel(WaterTile _tile)
    {
        // Base Case:
        if (_tile == null || _tile.waterLevel <= 1) return;

        foreach (WaterTile neighbour in _tile.neighbourList)
        {
            // No water should flow through dams
            if (neighbour is DamTile && !(neighbour as DamTile).isOpen) continue;

            // Here 0 means that the water level is not defined
            if (neighbour.waterLevel == 0 || neighbour.waterLevel <= _tile.waterLevel - 1)
            {
                neighbour.waterLevel = _tile.waterLevel - 1;
                DecideNeighbourWaterlevel(neighbour);
            }
            else if (neighbour.waterLevel == _tile.waterLevel - 1) { 
                neighbour.waterLevel = _tile.waterLevel;
                DecideNeighbourWaterlevel(neighbour);
            }
        }
    }

    private void ResetAllWaterTiles()
    {
        foreach(Tile tile in tiles)
        {
            if (!(tile is WaterTile)) continue;
            
            (tile as WaterTile).waterLevel = 0;
        }
    }

    // Move actual spawning to gameManager
    private void SpawnBoat()
    {
        float highestWaterLevel = 5;
        List<Tile> highestTiles = new List<Tile>();
        /*        for (int i = 0; i < 5; i++)
                {
                    highestTiles = grid.Select(pair => pair.Value)
                        .Where(tile => tile is WaterTile && (tile as WaterTile).waterLevel == highestWaterLevel - i)
                        .ToList();

                    if (highestTiles != null || highestTiles.Count != 0)
                    {
                        break;
                    }
                }*/

        highestTiles = grid.Select(pair => pair.Value)
        .Where(tile => tile is WaterTile && (tile as WaterTile).waterLevel == highestWaterLevel)
        .ToList();

        Tile randomTile = highestTiles[Random.Range(0, highestTiles.Count)];
        Vector3Int randomPos = randomTile.position;

        Instantiate(playerPrefab, randomPos, Quaternion.identity);
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
            List<Tile> neighbours = currentTile.neighbourList;

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


