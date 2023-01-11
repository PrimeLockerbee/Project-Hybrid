using UnityEngine;

public class GridCell
{
    public static int gridSize = 50;
    public Vector3Int cellCords;

    public GridCell(Vector3Int _cellCords)
    {
        cellCords= _cellCords;
    }

    public static Vector3Int WorldPosToCellCord(Vector3Int _worldPos)
    {
        return _worldPos / gridSize;
    }

    public Vector3Int ToWorldPos()
    {
        return cellCords * gridSize;
    }
}
