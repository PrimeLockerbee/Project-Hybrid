using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MarcoHelpers;
using EventSystem = MarcoHelpers.EventSystem;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public enum TileType { Water, Grass}

    public TileType type;

    public Vector3Int position => transform.position.ToVector3Int();

    [HideInInspector] public List<Tile> neighbours = new List<Tile>();

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
                        neighbours.Add(neighbour);
                    }
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData _eventData)
    {
        Debug.Log(transform.position);
        foreach (Tile tile in neighbours)
        {
            Debug.Log("Neighbour: " + tile.position);
        }

        EventSystem.RaiseEvent(EventName.TILE_CLICKED, position);
    }

    public bool isDeadEnd()
    {
        int adjacentWaterTiles = 0;

        foreach (Tile tile in neighbours)
        {
            if (tile.type == TileType.Water) adjacentWaterTiles++;
        }

        return adjacentWaterTiles <= 1;
    }
}
