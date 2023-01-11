using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MarcoHelpers;
using EventSystem = MarcoHelpers.EventSystem;
using System.Linq;

public enum Direction
{
    None = 0,
    Left = 1,
    Up = 2,
    Right = 3,
    Down = 4,
}

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public enum TileType { Water, Grass}

    public TileType type;

    public Vector3Int position => transform.position.ToVector3Int();

    [HideInInspector] public List<Tile> neighbourList => neighbourDictionary.Keys.ToList();
    public Dictionary<Tile, Direction> neighbourDictionary = new Dictionary<Tile, Direction>();

    public void StoreNeighbours(Dictionary<Vector3Int, Tile> _grid)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == z || x == -z || (x == 0 && z == 0)) continue;

                Vector3Int newPos = position + x * Vector3Int.right + z * Vector3Int.forward;
                if (_grid.ContainsKey(newPos))
                {
                    Tile neighbour = _grid[newPos];
                    if (neighbour != null && neighbour.type == TileType.Water)
                    {
                        if (x == 1) neighbourDictionary.Add(neighbour, Direction.Right);
                        else if (x == -1) neighbourDictionary.Add(neighbour, Direction.Left);
                        else if (z == 1) neighbourDictionary.Add(neighbour, Direction.Up);
                        else if (z == -1) neighbourDictionary.Add(neighbour, Direction.Down);
                    }
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData _eventData)
    {
        Debug.Log(transform.position);
        foreach (Tile tile in neighbourDictionary.Keys)
        {
            Debug.Log("Neighbour: " + tile.position);
        }

        EventSystem.RaiseEvent(EventName.TILE_CLICKED, position);
    }

    public bool isDeadEnd()
    {
        int adjacentWaterTiles = 0;

        foreach (Tile tile in neighbourDictionary.Keys)
        {
            if (tile.type == TileType.Water) adjacentWaterTiles++;
        }

        return adjacentWaterTiles <= 1;
    }
}
